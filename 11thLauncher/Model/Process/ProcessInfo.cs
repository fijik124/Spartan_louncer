namespace _11thLauncher.Model.Process
{
    public class ProcessInfo
    {
        public string Name { get; set; }

        public bool Running { get; set; }

        public bool Elevated { get; set; }

        public bool Is64Bit { get; set; }

        public ProcessInfo()
        {
            Name = "";
            Running = false;
            Elevated = false;
            Is64Bit = false;
        }
    }
}
