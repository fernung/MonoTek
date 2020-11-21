using System;
using System.Net.Sockets;

namespace MonoTek.Net.Client
{
    public interface IClientConnection
    {
        IClientTcp Tcp { get; set; }
        IClientUdp Udp { get; set; }
        bool Connected { get; }
    }

    public class ClientConnection : IClientConnection
    {
        protected INetClient _client;
        protected IClientTcp _tcp;
        protected IClientUdp _udp;

        public IClientTcp Tcp { get => _tcp; set => _tcp = value; }
        public IClientUdp Udp { get => _udp; set => _udp = value; }
        public bool Connected => _tcp.Socket != null && _tcp.Socket.Connected;

        public ClientConnection(INetClient client)
        {
            _client = client;
            _tcp = new ClientTcp(client);
            _udp = new ClientUdp();

        }
    }

    public interface IClientTcp
    {
        TcpClient Socket { get; set; }

        void Connect();
        void Send(IPacket packet);

        void ConnectCallback(IAsyncResult result);
        void ReceiveCallback(IAsyncResult result);
        bool HandleData(byte[] data);
    }
    public class ClientTcp : IClientTcp
    {
        public const int BufferSize = 4096;

        protected INetClient _client;
        protected TcpClient _socket;
        protected NetworkStream _stream;
        protected IPacket _packet;
        protected byte[] _buffer;

        public TcpClient Socket { get => _socket; set => _socket = value; }

        public ClientTcp(INetClient client)
        {
            _client = client;
        }

        public void Connect()
        {
            _socket = new TcpClient
            {
                ReceiveBufferSize = BufferSize,
                SendBufferSize = BufferSize
            };
            _buffer = new byte[BufferSize];
            _socket.BeginConnect(_client.Host, _client.Port, ConnectCallback, _socket);
            _client.Messages.Add($"Connecting to server: {_client.Host}:{_client.Port}...");
        }

        public void ConnectCallback(IAsyncResult result)
        {
            _socket.EndConnect(result);
            if (!_socket.Connected) return;
            _client.Messages.Add($"Successfully connected to {_client.Host}:{_client.Port}!");
            _stream = _socket.GetStream();
            _packet = new Packet();
            _buffer = new byte[BufferSize];
            _stream.BeginRead(_buffer, 0, BufferSize, ReceiveCallback, null);
        }

        public void Send(IPacket packet)
        {
            try
            {
                if (_socket == null) return;
                _stream.BeginWrite(packet.Writer.End(true), 0, packet.Size, SendCallback, null);
                _client.Messages.Add($"Sending Packet to server: [bytes: {packet.Size}]");
            }
            catch (Exception e) { Console.WriteLine($"Exception Thrown while Sending to Server via Tcp {e}"); }
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
                _client.Messages.Add($"Receiving Packet from server: [bytes: {_packet.Size}]");
                _packet.Reset(HandleData(data));
                _buffer = new byte[BufferSize];
                _stream.BeginRead(_buffer, 0, BufferSize, ReceiveCallback, null);

            }
            catch (Exception e) { Console.WriteLine($"Exception Thrown while Receiving from Client [{_client.Id}] via Tcp {e}"); }
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
                _client.ThreadManager.ExecuteOnMainThread(() =>
                {
                    using var packet = new Packet(packetBytes);
                    packet.Reader.Read(out int id);
                    _client.PacketHandlers[id](packet);
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

    public interface IClientUdp
    {

    }
    public class ClientUdp : IClientUdp
    {

    }
}
