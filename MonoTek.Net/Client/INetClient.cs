using MonoTek.Net.Server;
using System.Collections.Generic;

namespace MonoTek.Net.Client
{
    public delegate void PacketHandler(IPacket packet);
    public interface INetClient
    {
        int Id { get; set; }
        int Port { get; }
        string Host { get; }
        string Username { get; set; }
        bool Connected { get; }
        IClientConnection Connection { get; }
        IClientSender Sender { get; }
        IClientHandler Handler { get; }
        IThreadManager ThreadManager { get; }
        IList<string> Messages { get; }
        Dictionary<int, PacketHandler> PacketHandlers { get; set; }
    }
    public class NetClient : INetClient
    {
        protected static IThreadManager _thread = new ThreadManager();

        public const string DefaultHost = "127.0.0.1";
        public const int DefaultPort = 6969;
        public const int BufferSize = 4096;

        protected int _port, _id;
        protected string _host, _username;
        protected IClientConnection _connection;
        protected IClientSender _sender;
        protected IClientHandler _handler;
        protected Dictionary<int, PacketHandler> _packetHandler;

        protected List<string> _messages;

        public int Id { get => _id; set => _id = value; }
        public int Port => _port;
        public string Host => _host;
        public string Username { get => _username; set => _username = value; }
        public bool Connected => _connection.Connected;
        public IClientConnection Connection => _connection;
        public IClientSender Sender => _sender;
        public IClientHandler Handler => _handler;
        public IThreadManager ThreadManager => _thread;
        public Dictionary<int, PacketHandler> PacketHandlers { get => _packetHandler; set => _packetHandler = value; }
        public IList<string> Messages => _messages;

        public NetClient(string host = DefaultHost, int port = DefaultPort)
        {
            _id = 0;
            _username = "";
            _host = host;
            _port = port;
            _connection = new ClientConnection(this);
            _sender = new ClientSender(this);
            _handler = new ClientHandler(this);
            _messages = new List<string>();
        }

        public void Connect()
        {
            Initialize();
            _connection.Tcp.Connect();
        }

        protected void Initialize()
        {
            _packetHandler = new Dictionary<int, PacketHandler>()
            {
                { (int)ServerPackets.WelcomeStart, _handler.WelcomeStart },
                { (int)ServerPackets.WhoEnd, _handler.WhoEnd }
            };
        }

        public static void Update() => _thread.UpdateMain();
    }
}
