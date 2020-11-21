using MonoTek.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;


namespace MonoTek.Net.Server
{
    public interface IServerSender
    {
        void WelcomeStart(int client, string msg);
        void WhoEnd(int client);

        void SendTcp(int client, IPacket packet);
        void SendTcpDataToAll(IPacket packet);
        void SendTcpDataToAll(IPacket packet, int except);
    }
    public class ServerSender : IServerSender
    {
        protected INetServer _server;

        public ServerSender(INetServer server)
        {
            _server = server;
        }

        public void WelcomeStart(int client, string msg)
        {
            using Packet packet = new Packet((int)ServerPackets.WelcomeStart);
            packet.Writer.Write(client);
            packet.Writer.Write(msg);
            SendTcp(client, packet);
        }
        public void WhoEnd(int client)
        {
            var total = _server.Clients.Values.Count(x => x.Connected);
            var clients = "";
            var clientTotal = $"Total: {total}";
            if (total > 1)
            {
                var clientList = _server.Clients.Values.Where(x => x.Id != client && x.Connected).Select(x => x.Username);
                clients = string.Join(',', clientList);
                clients += $",{_server.Clients[client].Username} (You)";
            }
            else
            {
                clients += $"{_server.Clients[client].Username} (You)";
            }
            using Packet packet = new Packet((int)ServerPackets.WhoEnd);
            packet.Writer.Write(clientTotal);
            packet.Writer.Write(clients);
            SendTcp(client, packet);
        }

        public void SendTcp(int client, IPacket packet)
        {
            _server.Clients[client].Tcp.Send(packet);
        }
        public void SendTcpDataToAll(IPacket packet)
        {
            for (int i = 1; i < _server.MaxConnections; i++)
                _server.Clients[i].Tcp.Send(packet);
        }
        public void SendTcpDataToAll(IPacket packet, int except)
        {
            for (int i = 1; i < _server.MaxConnections; i++)
                if (i != except) _server.Clients[i].Tcp.Send(packet); 
        }
    }
}
