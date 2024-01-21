using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogikaPrace
{
    public class VisNodesHelper
    {
        Node mTree;
        List<VisNode> visNodesList { get; set; }

        public VisNodesHelper(Node tree)
        {
            mTree = tree;
            
        }

        public List<VisNode> CreateVisNodes()
        {
            visNodesList = new List<VisNode>();
            TraverseTreeToCreateVisNodes(mTree);
            return visNodesList;
        }

        private void TraverseTreeToCreateVisNodes(Node node)
        {
            if (node == null)
                return;

            visNodesList.Add(new VisNode
            {
                Id = node.id,
                Label = node.Value,
                ParentId = node.Parent != null ? node.Parent.id : 0
            });

            TraverseTreeToCreateVisNodes(node.Left);
            TraverseTreeToCreateVisNodes(node.Right);
        }

    }
}
