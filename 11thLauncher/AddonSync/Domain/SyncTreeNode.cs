using System;
using System.Xml.Serialization;

namespace _11thLauncher.AddonSync
{
    public class SyncTreeNode : IComparable
    {
        [XmlElement("name")]
        public string Name { get; set; }
        public SyncTreeDirectory Parent { get; set; }
        [XmlElement("destinationPath")]
        public string DestinationPath { get; set; }
        [XmlElement("updated")]
        public bool Updated { get; set; }
        [XmlElement("deleted")]
        public bool Deleted { get; set; }

        public SyncTreeNode()
        {
            Updated = false;
            Deleted = false;
        }

        public SyncTreeNode(string name, SyncTreeDirectory parent)
        {
            Updated = false;
            Deleted = false;
            Name = name;
            Parent = parent;
        }

        public int CompareTo(object other)
        {
            string name = ((SyncTreeNode)other).Name;
            int result = 1;
            if (string.Compare(name, Name, StringComparison.OrdinalIgnoreCase) > 0)
            {
                result = -1;
            }
            else if (string.Compare(name, Name, StringComparison.OrdinalIgnoreCase) == 0)
            {
                result = 0;
            }
            return result;
        }

        bool IsLeaf()
        {
            return false;
        }
    }
}
