using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

using ClientLibrary;
using SharedLibrary;

namespace DemoClient
{
    class ClientMain
    {
        static void Main(string[] args)
        {
            SimpleChatClient client = new SimpleChatClient();
            client.SendMessage();
        }

        class SimpleChatClient
        {
            Client client;

            public void SendMessage()
            {
                TCP_Config tcp_Config;
                tcp_Config.address = "127.0.0.1";
                tcp_Config.port = 4444;

                UDP_Config udp_Config;
                udp_Config.address = "127.0.0.1";
                udp_Config.port = 11000;

                //DotNetserialization tcpSerializer = new DotNetserialization();
                ProtoBufSerializer tcpSerializer = new ProtoBufSerializer();
                ClientListenerTCP chatListener = new ClientListenerTCP(tcp_Config);
                ClientConnectionTCP chatConnection = new ClientConnectionTCP("Chat_Connection", "PaulBoocock");
                chatConnection.AddListener(chatListener);
                chatConnection.AddSerializer(tcpSerializer);

                DotNetserialization udpSerializer = new DotNetserialization();
                ClientListenerUDP udpListener = new ClientListenerUDP(udp_Config);
                ClientConnectionUDP udpConnection = new ClientConnectionUDP("UDP_Connection", "PaulBoocock");
                udpConnection.AddListener(udpListener);
                udpConnection.AddSerializer(udpSerializer);

                client = new Client("PaulBoocock");
                client.AddConnection(chatConnection);
                client.AddConnection(udpConnection);
                client.Start();

                Thread recieveMessages = new Thread(GetMessages);
                recieveMessages.Start();

                string userInput;

                while ((userInput = Console.ReadLine()) != null)
                {
                    ChatMessagePacket message = new ChatMessagePacket(userInput, "PaulBoocock");
                    UsernamePacket username = new UsernamePacket("PaulBoocock");
                    client.SendMessage(message, "Chat_Connection");
                    client.SendMessage(username, "Chat_Connection");
                    client.SendMessage(message, "UDP_Connection");
                }
            }

            public void GetMessages()
            {
                List<Packet> tcpPacketList = new List<Packet>();
                List<Packet> udpPacketList = new List<Packet>();

                while (true)
                {
                    tcpPacketList = client.RecieveMessages("Chat_Connection");

                    if (tcpPacketList.Count() > 0)
                    {
                        Thread.Sleep(20);
                        string returnMessage = ((ChatMessagePacket)tcpPacketList[0]).message;
                        string sender = ((ChatMessagePacket)tcpPacketList[0]).sender;
                        Console.WriteLine(sender + ": " + returnMessage);

                        client.ClearMessages("Chat_Connection");
                    }

                    udpPacketList = client.RecieveMessages("UDP_Connection");

                    if (udpPacketList.Count() > 0)
                    {
                        Thread.Sleep(20);
                        string returnMessage = ((ChatMessagePacket)udpPacketList[0]).message;
                        string sender = ((ChatMessagePacket)udpPacketList[0]).sender;
                        Console.WriteLine(sender + ": " + returnMessage);

                        client.ClearMessages("UDP_Connection");
                    }
                }
            }
        }
    }
}
