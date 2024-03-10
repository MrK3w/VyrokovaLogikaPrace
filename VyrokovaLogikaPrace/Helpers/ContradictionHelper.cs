using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogikaPrace
{
    public class ContradictionHelper
    {
        public List<Tuple<string, int>> DistinctNodes { get; set; } = new List<Tuple<string, int>>();
        public Node CounterModel { get; set; } = new Node(0);
        bool contradictionInTree;

        public bool FindContradiction(Node tree)
        {
            if (!FindContradictionInLeafs(new List<Node> { tree}))
            {
                FindContradictionInTree(tree);
                if (!contradictionInTree) return false;
            }
            return true;
        }

        private void FindContradictionInTree(Node tree)
        {
            if (tree.IsLeaf) return;
            var sideValues = TreeHelper.GetValuesOfBothSides(tree.TruthValue, tree);
            contradictionInTree = true;
            tree.Red = true;
            foreach (var sideValue in sideValues)
            {
                if (tree.Right != null)
                {
                    if (sideValue.Item1 == tree.Left.TruthValue && sideValue.Item2 == tree.Right.TruthValue)
                    {
                        contradictionInTree = false;
                        tree.Red = false;
                    }
                }
                else
                {
                    if (sideValue.Item1 == tree.Left.TruthValue)
                    {
                        tree.Red = false;
                        contradictionInTree = false;
                    }
                }
            }
            if(tree.Red == true )
            {
                if (tree.TruthValue == 1)
                    tree.TruthValue2 = 0;
                else if (tree.TruthValue == 0)
                    tree.TruthValue2 = 1;

            }
            if (contradictionInTree) return;
            else
            {
                if (tree.Left != null)
                {
                    FindContradictionInTree(tree.Left);
                }
                if (tree.Right != null)
                {
                    FindContradictionInTree(tree.Right);
                }
            }
        }

        public bool FindContradictionInLeafs(List<Node> Trees)
        {
            foreach (var tree in Trees)
            {
                bool contradiction = false;
                List<Node> leafNodes = new List<Node>();
                GetLeafNodes(tree, ref leafNodes);
                foreach (var node in leafNodes)
                {
                    if (contradiction) break;
                    foreach (var node1 in leafNodes)
                    {
                        if (node1.Value == node.Value && node1.TruthValue != node.TruthValue)
                            contradiction = true;
                    }
                }
                if (contradiction == false)
                {
                    DistinctNodes = leafNodes
                        .Select(n => new Tuple<string, int>(n.Value, n.TruthValue))
                        .Distinct()
                        .ToList();
                    CounterModel = tree;
                    return false;

                }
            }
            CounterModel = Trees.LastOrDefault();
            List<Node> leafNodes2 = new List<Node>();
            GetLeafNodes(CounterModel, ref leafNodes2);
            DistinctNodes = leafNodes2
                        .Select(n => new Tuple<string, int>(n.Value, n.TruthValue))
                        .Distinct()
                        .ToList();
            return true;
        }

      

        public void GetLeafNodes(Node tree, ref List<Node> leafNodes)
        {
            //if node is null return
            if (tree == null) return;

            if (tree.Left == null && tree.Right == null)
            {
                // Node has no children, so it's a leaf node, we add node to list
                leafNodes.Add(tree);
                // if we are searching for literal to mark him, we will do it here 
            }
            else
            {
                // Node has children, so we recursively traverse its childrens
                if (tree.Left != null)
                {
                    GetLeafNodes(tree.Left, ref leafNodes);
                }
                GetLeafNodes(tree.Right, ref leafNodes);
            }
        }
    }
}
