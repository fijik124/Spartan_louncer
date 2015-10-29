using System.Xml.Serialization;
using _11thLauncher.Configuration;

namespace _11thLauncher.AddonSync
{
    [XmlRoot(ElementName = "SyncTreeLeaf")]
    public class SyncTreeLeaf : SyncTreeNode
    {
        [XmlElement("sha1")]
        public string Sha1 { get; set; }
        [XmlElement("localSHA1")]
        public string LocalSHA1 { get; set; }
        [XmlElement("size")]
        public long Size { get; set; }
        [XmlElement("complete")]
        public long Complete { get; set; }// file completion in %

        public SyncTreeLeaf()
        {
            List = new SyncTreeNodeList(); //Dummy children list
            IsLeaf = true;
        }

        public void ComputeLocalHash()
        {
            //Get full path 
            string path = "";
            SyncTreeNode current = Parent;
            while(current.Parent != null)
            {
                path = "\\" + current.Name + path;
                current = current.Parent;
            }
            path = Settings.Arma3Path + path + "\\" + Name;

            //Compute SHA1
            LocalSHA1 = Cryptography.computeSHA1(path);
        }
    }
}
