using _11thLauncher.Models;

namespace _11thLauncher.Messages
{
    public class FillServerInfoMessage
    {
        public Server Server;

        public FillServerInfoMessage(Server server)
        {
            Server = server;
        }
    }
}
