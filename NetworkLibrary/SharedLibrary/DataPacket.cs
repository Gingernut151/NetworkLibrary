using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace SharedLibrary
{
    public enum PacketType
    {
        EMPTY,
        USERNAME,
        CHATMESSAGE,
        DISCONNECT,
        CUSTOM
    }
    //-----------------------------------------------------------------------------------------
    [Serializable]
    [ProtoContract]
    [ProtoInclude(100, typeof(ChatMessagePacket))]
    [ProtoInclude(101, typeof(UsernamePacket))]
    [ProtoInclude(102, typeof(DisconnectPacket))]
    public abstract class Packet
    {
        [ProtoMember(1)]
        public PacketType type = PacketType.EMPTY;
        [ProtoMember(2)]
        public string sender = string.Empty;
    }
    //-----------------------------------------------------------------------------------------
    [Serializable]
    [ProtoContract]
    public class ChatMessagePacket : Packet
    {
        [ProtoMember(3)]
        public string message = String.Empty;
        public ChatMessagePacket() // Protobuf requires a base constructor
        {
            this.type = PacketType.CHATMESSAGE;
            this.message = "This is defualt constructed";
            this.sender = "Defualt";
        }

        public ChatMessagePacket(string message, string sender)
        {
            this.type = PacketType.CHATMESSAGE;
            this.message = message;
            this.sender = sender;
        }
    }
    //-----------------------------------------------------------------------------------------
    [Serializable]
    [ProtoContract]
    public class UsernamePacket : Packet
    {
        public UsernamePacket() // Protobuf requires a base constructor
        {
            this.type = PacketType.USERNAME;
            this.sender = "Defualt";
        }
        public UsernamePacket(string username)
        {
            this.type = PacketType.USERNAME;
            this.sender = username;
        }
    }
    //-----------------------------------------------------------------------------------------
    [Serializable]
    [ProtoContract]
    public class DisconnectPacket : Packet
    {
        public DisconnectPacket() // Protobuf requires a base constructor
        {
            this.type = PacketType.DISCONNECT;
            this.sender = "Defualt";
        }
        public DisconnectPacket(string username)
        {
            this.type = PacketType.DISCONNECT;
            this.sender = username;
        }
    }
}
