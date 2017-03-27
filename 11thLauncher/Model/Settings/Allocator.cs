namespace _11thLauncher.Model.Settings
{
    public enum AllocatorType { X32, X64 }
    public class Allocator
    {
        public string Name;
        public string DisplayName;
        public AllocatorType AllocatorType;

        public Allocator(string name, string displayName, AllocatorType allocatorType)
        {
            Name = name;
            DisplayName = displayName;
            AllocatorType = allocatorType;
        }
    }
}
