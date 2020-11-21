namespace MonoTek.Net.Client
{
    public interface IClientHandler
    {
        void WelcomeStart(IPacket packet);
        void WhoEnd(IPacket packet);
    }

    public class ClientHandler : IClientHandler
    {
        protected INetClient _client;

        public ClientHandler(INetClient client)
        {
            _client = client;
        }
        public void WelcomeStart(IPacket packet)
        {
            packet.Reader.Read(out int id);
            packet.Reader.Read(out string message);
            _client.Messages.Add($"[Server]: [WelcomeStart][Id: {id}] {message}");
            _client.Id = id;
            _client.Sender.WelcomeEnd();
        }

        public void WhoEnd(IPacket packet)
        {
            packet.Reader.Read(out string total).Read(out string users);
            _client.Messages.Add($"[Server]: [WhoEnd]");
            _client.Messages.Add($"{total}");
            _client.Messages.Add($"{users}");
        }
    }
}
