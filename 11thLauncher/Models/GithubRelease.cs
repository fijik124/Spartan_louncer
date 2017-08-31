namespace _11thLauncher.Models
{
    public class GithubRelease
    {
        public string tag_name;
        public string name;
        public Asset[] assets;
    }

    public class Asset
    {
        public string name;
        public string browser_download_url;
    }
}
