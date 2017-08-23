using _11thLauncher.Models;

namespace _11thLauncher.Messages
{
    public class ServerQueryFinished
    {
        public Server Server;

        public ServerQueryFinished(Server server)
        {
            Server = server;
        }
    }
}
