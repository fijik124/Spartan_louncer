using System.Net;

namespace _11thLauncher.Accessors.Contracts
{
    public interface INetworkAccessor
    {
        WebResponse GetWebResponse(WebRequest request);

        string DownloadString(WebClient webClient, string uri);
    }
}
