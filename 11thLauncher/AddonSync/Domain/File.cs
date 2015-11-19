namespace _11thLauncher.AddonSync.Domain
{
    class File
    {
        public string Path { get; set; }
        public string LocalHash { get; set; }
        public string RemoteHash { get; set; }
        public FileStatus FileStatus { get; set; }

        public string RelativePath()
        {
            //TODO
            return Path;
        }
    }
}
