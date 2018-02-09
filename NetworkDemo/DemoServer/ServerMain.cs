using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using ServerLibrary;
using SharedLibrary;

namespace DemoServer
{
    class ServerMain
    {
        static void Main(string[] args)
        {
            TCP_Config tcp_Config;
            tcp_Config.address = "127.0.0.1";
            tcp_Config.port = 4444;

            UDP_Config udp_Config;
            udp_Config.address = "127.0.0.1";
            udp_Config.port = 11000;

            ServerListenerTCP chatListener = new ServerListenerTCP(tcp_Config);
            ProtoBufSerializer chatSerialize = new ProtoBufSerializer();
            //DotNetserialization chatSerialize = new DotNetserialization();
            ServerConnectionTCP chatConnection = new ServerConnectionTCP("Chat_Connection");
            chatConnection.AddListener(chatListener);
            chatConnection.AddSerializer(chatSerialize);

            ServerListenerUDP udpListener = new ServerListenerUDP(udp_Config);
            DotNetserialization UdpSerialize = new DotNetserialization();
            ServerConnectionUDP udpConnection = new ServerConnectionUDP("UDP_Connection");
            udpConnection.AddListener(udpListener);
            udpConnection.AddSerializer(UdpSerialize);

            Server server = new Server();
            server.AddConnection(chatConnection);
            server.AddConnection(udpConnection);
            server.Start();

            server.AllowTcpConnection("Chat_Connection");

            List<Packet> TcpPacketList;
            List<Packet> UdpPacketList;
            string returnMessage;

            while (true)
            {
                TcpPacketList = server.RecieveMessages("Chat_Connection");

                if (TcpPacketList.Count() > 0)
                {
                    Thread.Sleep(20);

                    if (TcpPacketList[0].type == PacketType.CHATMESSAGE)
                    {
                        returnMessage = ((ChatMessagePacket)TcpPacketList[0]).message;
                        string sender = ((ChatMessagePacket)TcpPacketList[0]).sender;
                        Console.WriteLine("Tcp - " + sender + ": " + returnMessage);

                        server.SendPacketToAll(TcpPacketList[0], "Chat_Connection");
                        server.ClearMessages("Chat_Connection");
                    }
                    else if (TcpPacketList[0].type == PacketType.DISCONNECT)
                    {
                        string sender = ((DisconnectPacket)TcpPacketList[0]).sender;
                        Console.WriteLine("Tcp Disconnect - " + sender + "!!!");
                    }

                    List<ServerClient> tcpList = server.GetConnectionClientList("Chat_Connection");

                    for (int i = 0; i < tcpList.Count; i++)
                    {
                        Console.WriteLine("tcpList: " + tcpList[i]._name);
                    }
                }

                UdpPacketList = server.RecieveMessages("UDP_Connection");

                if (UdpPacketList.Count() > 0)
                {
                    Thread.Sleep(20);

                    if (UdpPacketList[0].type == PacketType.CHATMESSAGE)
                    {
                        returnMessage = ((ChatMessagePacket)UdpPacketList[0]).message;
                        string sender = ((ChatMessagePacket)UdpPacketList[0]).sender;
                        Console.WriteLine("Udp - " + sender + ": " + returnMessage);

                        server.SendPacketToAll(UdpPacketList[0], "UDP_Connection");
                        server.ClearMessages("UDP_Connection");
                    }
                    else if (UdpPacketList[0].type == PacketType.DISCONNECT)
                    {
                        string sender = ((DisconnectPacket)UdpPacketList[0]).sender;
                        Console.WriteLine("UDP Disconnect - " + sender + "!!!");
                    }

                    List<ServerClient> udpList = server.GetConnectionClientList("UDP_Connection");

                    for (int i = 0; i < udpList.Count; i++)
                    {
                        Console.WriteLine("udpList: " + udpList[i]._name);
                    }
                }
            }
        }
    }
}
