using MonoTek.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace MonoTek.Net.Server
{
    public interface IServerConnection
    {
        int Id { get; set; }
        string Username { get; set; }
        IServerTcp Tcp { get; set; }
        IServerUdp Udp { get; set; }
        bool Connected { get; }
    }
    public class ServerConnection : IServerConnection
    {
        public const int BufferSize = 4096;

        protected int _id;
        protected IServerTcp _tcp;
        protected IServerUdp _udp;
        protected string _username;

        public int Id { get => _id; set => _id = value; }
        public string Username { get => _username; set => _username = value; }
        public IServerTcp Tcp { get => _tcp; set => _tcp = value; }
        public IServerUdp Udp { get => _udp; set => _udp = value; }
        public bool Connected => _tcp.Socket != null && _tcp.Socket.Connected;

        public ServerConnection(INetServer server, int id)
        {
            _id = id;
            _username = "";
            _tcp = new ServerTcp(ref server, id);
            _udp = new ServerUdp(id);
        }
    }

    public interface IServerTcp
    {
        TcpClient Socket { get; }
        string EndPoint { get; }


        void Connect(TcpClient socket);
        void Send(IPacket packet);
        void SendCallback(IAsyncResult result);
        void ReceiveCallback(IAsyncResult result);
        bool HandleData(byte[] data);
    }
    public class ServerTcp : IServerTcp
    {
        public const int BufferSize = 4096;

        protected INetServer _server;

        protected readonly int _id;
        protected TcpClient _socket;
        protected NetworkStream _stream;
        protected IPacket _packet;
        protected byte[] _buffer;

        public TcpClient Socket => _socket;
        public string EndPoint => _socket == null ? "" : _socket.Client.RemoteEndPoint.ToString();

        public ServerTcp(ref INetServer server, int id)
        {
            _server = server;
            _id = id;
        }

        public void Connect(TcpClient socket)
        {
            _socket = socket;
            socket.ReceiveBufferSize = BufferSize;
            socket.SendBufferSize = BufferSize;
            _stream = _socket.GetStream();
            _packet = new Packet();
            _buffer = new byte[BufferSize];
            _stream.BeginRead(_buffer, 0, BufferSize, ReceiveCallback, null);
            _server.Sender.WelcomeStart(_id, "Welcome!");
        }

        public void Send(IPacket packet)
        {
            try
            {
                if (_socket == null) return;
                _stream.BeginWrite(packet.Writer.End(true), 0, packet.Size, SendCallback, null);
            }
            catch (Exception e) { NetServer.Log($"Exception Thrown while Sending to Client [{_id}] via Tcp {e}"); }
        }
        public void SendCallback(IAsyncResult result)
        {
            _stream.EndWrite(result);
        }

        public void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                int receivedAmount = _stream.EndRead(result);
                if (receivedAmount <= 0)
                {
                    //Disconnect
                    return;
                }
                var data = new byte[receivedAmount];
                Buffer.BlockCopy(_buffer, 0, data, 0, receivedAmount);
                _packet.Reset(HandleData(data));
                _buffer = new byte[BufferSize];
                _stream.BeginRead(_buffer, 0, BufferSize, ReceiveCallback, null);

            }
            catch (Exception e) { NetServer.Log($"Exception Thrown while Receiving from Client [{_id}] via Tcp {e}"); }
        }
        public bool HandleData(byte[] data)
        {
            var length = 0;
            _packet.Writer.Write(data);
            if (_packet.Reader.Unread >= 4)
            {
                _packet.Reader.Read(out length);
                if (length <= 0) return true;
            }

            while (length > 0 && length <= _packet.Reader.Unread)
            {
                _packet.Reader.Read(out byte[] packetBytes, length);
                _server.ThreadManager.ExecuteOnMainThread(() =>
                {
                    using var packet = new Packet(packetBytes);
                    packet.Reader.Read(out int id);
                    _server.PacketHandlers[id](_id, packet);
                });
                length = 0;
                if (_packet.Reader.Unread >= 4)
                {
                    _packet.Reader.Read(out length);
                    if (length <= 0) return true;
                }
            }
            if (length <= 1) return true;
            return false;
        }
    }

    public interface IServerUdp
    {
    }
    public class ServerUdp : IServerUdp
    {
        protected int _id;

        public ServerUdp(int id)
        {
            _id = id;
        }
    }
}
