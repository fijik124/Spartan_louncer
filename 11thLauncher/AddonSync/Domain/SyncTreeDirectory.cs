using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace _11thLauncher.AddonSync
{
    [XmlRoot(ElementName = "syncTreeDirectory")]
    public class SyncTreeDirectory : SyncTreeNode
    {
        [XmlElement(ElementName = "syncTreeDirectory", Type = typeof(SyncTreeDirectory))]
        [XmlElement(ElementName = "syncTreeLeaf", Type = typeof(SyncTreeLeaf))]
        public SyncTreeNodeList List { get; set; }
        [XmlElement("markAsAddon")]
        public bool MarkAsAddon { get; set; }
        [XmlElement("hidden")]
        public bool Hidden { get; set; }

        public SyncTreeDirectory()
        {
            List = new SyncTreeNodeList(this);
            MarkAsAddon = false;
            Hidden = false;
        }

        //TODO ???
        public void AddTreeNode(SyncTreeNode syncTreeNode)
        {
            List.Add(syncTreeNode);

            List<SyncTreeDirectory> directories = new List<SyncTreeDirectory>();
            List<SyncTreeLeaf> leafs = new List<SyncTreeLeaf>();
            foreach (SyncTreeNode t in List)
            {
                if (t is SyncTreeDirectory)
                {
                    directories.Add((SyncTreeDirectory)t);
                }
                else
                {
                    leafs.Add((SyncTreeLeaf)t);
                }
            }
            directories.Sort();
            leafs.Sort();
            List.Clear();
            List.AddRange(directories);
            List.AddRange(leafs);
        }
    }
}
