using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Net;

using SharedLibrary;

namespace ClientLibrary
{
    public abstract class ClientListener
    {
        public abstract void Start();
        public abstract void Stop();
    }
    //-----------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------
    public class ClientListenerTCP : ClientListener
    {
        private TCP_Config _config;
        private TcpClient _tcpClient;
        //-----------------------------------------------------------------------------------------
        public ClientListenerTCP(TCP_Config config)
        {
            _config = config;

            _tcpClient = new TcpClient();
        }
        //-----------------------------------------------------------------------------------------
        public override void Start()
        {
            _tcpClient.Connect(_config.address, _config.port);
        }
        //-----------------------------------------------------------------------------------------
        public override void Stop()
        {

        }
        //-----------------------------------------------------------------------------------------
        public NetworkStream GetStream()
        {
            return _tcpClient.GetStream();
        }
        //-----------------------------------------------------------------------------------------
    }
    //-----------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------
    public class ClientListenerUDP : ClientListener
    {
        private UDP_Config _config;
        private UdpClient _udpClient;
        //-----------------------------------------------------------------------------------------
        public ClientListenerUDP(UDP_Config config)
        {
            _config = config;
            _udpClient = new UdpClient();
        }
        //-----------------------------------------------------------------------------------------
        public override void Start()
        {
            _udpClient.Connect(new IPEndPoint(IPAddress.Parse(_config.address), _config.port));
        }
        //-----------------------------------------------------------------------------------------
        public override void Stop()
        {
            _udpClient.Close();
        }
        //-----------------------------------------------------------------------------------------
        public void SendUdpPacket(byte[] data)
        {
            _udpClient.Send(data, data.Length);
        }
        //-----------------------------------------------------------------------------------------
        public byte[] ReceiveUdpPackets()
        {
            IPEndPoint sender = new IPEndPoint(IPAddress.Parse(_config.address), _config.port);

            return _udpClient.Receive(ref sender);
        }
        //-----------------------------------------------------------------------------------------
    }
}
