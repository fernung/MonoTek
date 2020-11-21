using MonoTek.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace MonoTek.Net.Server
{
    public delegate void PacketHandler(int client, Packet packet);

    public interface INetServer
    {
        int Port { get; }
        int MaxConnections { get; }
        IThreadManager ThreadManager { get; }
        IServerSender Sender { get; }
        IServerHandler Handler { get; }
        Dictionary<int, IServerConnection> Clients { get; }
        Dictionary<int, PacketHandler> PacketHandlers { get; set; }

        void Start();
        void Initialize();
        void ConnectCallback(IAsyncResult result);
    }
    public class NetServer : INetServer
    {
        public const int DefaultPort = 6969;
        public const int DefaultMaxConnections = 10;

        protected static IThreadManager _thread = new ThreadManager();

        protected int _port, _maxConnections;
        protected TcpListener _tcpListener;
        protected IServerSender _sender;
        protected IServerHandler _handler;
        protected Dictionary<int, IServerConnection> _clients;
        protected Dictionary<int, PacketHandler> _packetHandlers;

        public int Port => _port;
        public int MaxConnections => _maxConnections;
        public IThreadManager ThreadManager => _thread;
        public IServerSender Sender => _sender;
        public IServerHandler Handler => _handler;
        public Dictionary<int, IServerConnection> Clients => _clients;

        public Dictionary<int, PacketHandler> PacketHandlers 
        { get => _packetHandlers; set => _packetHandlers = value; }
        
        public NetServer(int port = DefaultPort, int maxConnections = DefaultMaxConnections)
        {
            _port = port;
            _maxConnections = maxConnections;
            Initialize();
        }

        public void Start()
        {
            Log("Starting Server...");
            _tcpListener = new TcpListener(IPAddress.Any, _port);
            _tcpListener.Start();
            _tcpListener.BeginAcceptTcpClient(ConnectCallback, null);
            Log($"Server started on Port [{_port}]");
        }

        public void Initialize()
        {
            Log("Initializing Server...");
            Log("Initializing Clients...");
            _clients = new Dictionary<int, IServerConnection>();
            for (int i = 1; i < _maxConnections; i++)
                _clients.Add(i, new ServerConnection(this, i));
            Log("Initialized Clients!");
            Log("Initializing Packet Handlers...");
            PacketHandlers = new Dictionary<int, PacketHandler>()
            {
                {  (int)ClientPackets.WelcomeEnd, _handler.WelcomeEnd },
                {  (int)ClientPackets.WhoStart, _handler.WhoStart }
            };
            Log("Initialized Packet Handlers!");
            Log("Initializing Server Sender/Handler...");
            _sender = new ServerSender(this);
            _handler = new ServerHandler(this);
            Log("Initialized Server Sender/Handler!");
            Log("Initialized Server!");
        }
        public void ConnectCallback(IAsyncResult result)
        {
            var client = _tcpListener.EndAcceptTcpClient(result);
            Log($"Incoming Connection from {client.Client.RemoteEndPoint}...");
            Log("Looking for open Client to Bind the Connecting to...");
            foreach (var c in _clients.Values)
            {
                if (c.Tcp.Socket == null)
                {
                    Log("Found open Client to Bind the Connecting to!");
                    Log($"Binding Connection to Client[Id:{c.Id}]...");
                    c.Tcp.Connect(client);
                    Log($"Successfully Bound Connection to Client[Id:{c.Id}]!");
                    break;
                }
            }
            _tcpListener.BeginAcceptTcpClient(ConnectCallback, null);
        }

        
        public static void Log(string message) => Console.WriteLine(message);
        public static void Update() => _thread.UpdateMain();
    }
}
