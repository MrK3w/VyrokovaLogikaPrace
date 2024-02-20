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

        //method to recursively traverse the tree and create Vis.js nodes
        private void TraverseTreeToCreateVisNodes(Node node)
        {
            //If the node is null, return
            if (node == null)
                return;

            // Create a new VisNode and add it to the list
            visNodesList.Add(new VisNode
            {
                Id = node.id,
                Label = node.Value,
                ParentId = node.Parent != null ? node.Parent.id : 0,
                Operator = TreeHelper.GetOP(node)
            });

            // Recursively traverse the left and right children
            TraverseTreeToCreateVisNodes(node.Left);
            TraverseTreeToCreateVisNodes(node.Right);
        }

    }
}
