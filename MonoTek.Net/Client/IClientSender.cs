namespace MonoTek.Net.Client
{
    public interface IClientSender
    {
        void SendTcp(IPacket packet);
        void WelcomeEnd();
        void WhoStart();
    }
    public class ClientSender : IClientSender
    {
        protected INetClient _client;

        public ClientSender(INetClient client)
        {
            _client = client;
        }

        public void SendTcp(IPacket packet)
        {
            _client.Connection.Tcp.Send(packet);
        }
        public void WelcomeEnd()
        {
            using Packet packet = new Packet((int)ClientPackets.WelcomeEnd);
            packet.Writer.Write(_client.Id);
            packet.Writer.Write(_client.Username);
            SendTcp(packet);
            _client.Messages.Add($"Sending to server: [WelcomeEnd][Id: {_client.Id}][Username: {_client.Username}]");
        }
        public void WhoStart()
        {
            using Packet packet = new Packet((int)ClientPackets.WhoStart);
            packet.Writer.Write(_client.Id);
            SendTcp(packet);
            _client.Messages.Add($"Sending to server: [WhoStart][Id: {_client.Id}]");
        }
    }
}
