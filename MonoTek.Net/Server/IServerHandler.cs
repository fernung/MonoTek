using MonoTek.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace MonoTek.Net.Server
{
    public interface IServerHandler
    {
        void WelcomeEnd(int client, IPacket packet);
        void WhoStart(int client, IPacket packet);
    }
    public class ServerHandler : IServerHandler
    {
        protected INetServer _server;

        public ServerHandler(INetServer server)
        {
            _server = server;
        }
        public void WelcomeEnd(int client, IPacket packet)
        {
            packet.Reader.Read(out int idCheck).Read(out string username);
            NetServer.Log($"{_server.Clients[client].Tcp.EndPoint} connected Successfully! [Id: {client}][Username: {username}]");
            if (client != idCheck)
            {
                NetServer.Log($"Player: [{username}, {client}] assumed the wrong Client ID {idCheck}");
                return;
            }
            _server.Clients[idCheck].Username = username; 
            //TODO: Add player to game
        }
        public void WhoStart(int client, IPacket packet)
        {
            packet.Reader.Read(out int idCheck);
            if (client != idCheck)
            {
                NetServer.Log($"Player: [{_server.Clients[client].Username}, {client}] assumed the wrong Client ID {idCheck}");
                return;
            }
            _server.Sender.WhoEnd(client); 
        }
    }
}
