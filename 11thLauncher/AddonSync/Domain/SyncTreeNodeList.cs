using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace _11thLauncher.AddonSync
{
    public class SyncTreeNodeList : ObservableCollection<SyncTreeNode>
    {
        private SyncTreeDirectory _parent;

        public SyncTreeNodeList() : base() { }

        public SyncTreeNodeList(SyncTreeDirectory parent) : base()
        {
            _parent = parent;
        }

        public SyncTreeNodeList(SyncTreeDirectory parent, IEnumerable<SyncTreeNode> collection) : base(collection)
        {
            _parent = parent;
        }

        public new void Add(SyncTreeNode item)
        {
            if (item != null)
            {
                item.Parent = _parent;
            }

            base.Add(item);
        }

        public new void Insert(int index, SyncTreeNode item)
        {
            if (item != null)
            {
                item.Parent = _parent;
            }

            base.Insert(index, item);
        }

        public new bool Remove(SyncTreeNode item)
        {
            bool result = base.Remove(item);
            if (item != null)
            {
                item.Parent = null;
            }
            return result;
        }

        public new void RemoveAt(int index)
        {
            SyncTreeNode item = base[index];
            base.RemoveAt(index);
            if (item != null)
            {
                item.Parent = null;
            }
        }

        public new void Clear()
        {
            foreach (SyncTreeNode item in Items)
            {
                if (item != null)
                {
                    item.Parent = null;
                }
            }
            base.Clear();
        }

        public new SyncTreeNode this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                SyncTreeNode oldItem = base[index];
                if (value != null)
                {
                    value.Parent = _parent;
                }
                base[index] = value;
                if (oldItem != null)
                {
                    oldItem.Parent = null;
                }
            }
        }
    }
}
