using System.Net;
using QueryMaster;
using QueryMaster.GameServer;

namespace _11thLauncher.Accessors.Contracts
{
    public interface INetworkAccessor
    {
        WebResponse GetWebResponse(WebRequest request);

        string DownloadString(WebClient webClient, string uri);

        Server QueryServerInstance(EngineType type, string ip, ushort port);

        ServerInfo GetServerInfo(Server server);

        QueryMasterCollection<PlayerInfo> GetServerPlayers(Server server);
    }
}
