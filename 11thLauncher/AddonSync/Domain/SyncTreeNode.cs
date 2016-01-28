using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace _11thLauncher.AddonSync
{
    public class SyncTreeNode : IComparable, INotifyPropertyChanged
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
        [XmlElement(ElementName = "syncTreeDirectory", Type = typeof(SyncTreeDirectory))]
        [XmlElement(ElementName = "syncTreeLeaf", Type = typeof(SyncTreeLeaf))]
        public SyncTreeNodeList List { get; set; }
        [XmlIgnore]
        public bool IsLeaf { get; set; }

        //View model attributes
        [XmlIgnore]
        private bool isSelected;

        public event PropertyChangedEventHandler PropertyChanged;

        public SyncTreeNode()
        {
            Updated = false;
            Deleted = false;
            List = new SyncTreeNodeList();
            IsLeaf = false;
            isSelected = false;
        }

        public SyncTreeNode(string name, SyncTreeDirectory parent) : this()
        {
            Name = name;
            Parent = parent;
        }

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }

            set
            {
                isSelected = value;
                if (List.Count != 0)
                {
                    foreach (SyncTreeNode node in List)
                    {
                        node.IsSelected = value;
                    }
                }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
                }
            }
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
    }
}
