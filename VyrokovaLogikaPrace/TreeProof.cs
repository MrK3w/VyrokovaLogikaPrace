using System.Collections.Generic;
using System;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace VyrokovaLogikaPrace
{
    public class TreeProof
    {
        public List<Tuple<string, int>> DistinctNodes { get; set; } = new List<Tuple<string, int>>();
        public Node CounterModel { get; set; } = new Node(0);
        public TreeProof()
        {
        }

        public List<Node> ProcessTree(Node tree, int truthValue = 0)
        {
            List<Node> combinedTrees = new List<Node>();

            //if leaf return value of leaf
            if (tree.IsLeaf)
            {
                return GetLeave(tree, truthValue);
            }
            var sideValues = TreeHelper.GetValuesOfBothSides(truthValue, tree);

            //we must iterate for each available option which can tree childs evaluate to get parent value
            foreach (var truthValues in sideValues)
            {
                //create new list for left and right side
                List<Node> currentTreeListFromLeftSide = new List<Node>();
                List<Node> currentTreeListFromRightSide = new List<Node>();
                //if tree has childNodeLeft or childNodeRight do recursion for this tree
                if (tree.Left != null)
                {
                    currentTreeListFromLeftSide = ProcessTree(tree.Left, truthValues.Item1);
                }
                if (tree.Right != null)
                {
                    currentTreeListFromRightSide = ProcessTree(tree.Right, truthValues.Item2);
                }
                //connect right and left trees this dont iterate if we dont have any of this lists
                for (int i = 0; i < currentTreeListFromLeftSide.Count; i++)
                {
                    for (int j = 0; j < currentTreeListFromRightSide.Count; j++)
                    {
                        //create new tempTree where we store trees
                        var tempTree = TreeHelper.GetNode(TreeHelper.GetOP(tree), tree.id);
                        //if tree is root take value from parameter
                        tempTree.TruthValue = truthValue;
                        //add to tempTree his tree childs
                        tempTree.Left = currentTreeListFromLeftSide[i];
                        tempTree.Right = currentTreeListFromRightSide[j];
                        //add this childs to combinedTree list
                        combinedTrees.Add(tempTree);
                    }
                }
                //if tree don't have right side to the same 
                if (currentTreeListFromRightSide.Count == 0)
                {
                    for (int i = 0; i < currentTreeListFromLeftSide.Count; i++)
                    {
                        var tempTree = TreeHelper.GetNode(TreeHelper.GetOP(tree), tree.id);
                        tempTree.TruthValue = truthValue;
                        tempTree.Left = currentTreeListFromLeftSide[i];
                        combinedTrees.Add(tempTree);
                    }
                }
                //if tree don't have left side to the same 
            }
            return combinedTrees;
        }

        //get leaf in tree
        private static List<Node> GetLeave(Node tree, int truthValue)
        {
            Node leafTree = new ValueNode(tree.Value, truthValue, 1);
            List<Node> treeees = new List<Node>();
            return new List<Node> { leafTree };
        }

        public bool FindContradiction(List<Node> Trees)
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
                       .GroupBy(node => new { node.Value, node.TruthValue })
                       .Select(group => group.First())
                       .Select(node => new Tuple<string, int>(node.Value, node.TruthValue))
                       .ToList();
                    CounterModel = tree;
                    return false;

                }
            }
            CounterModel = Trees.LastOrDefault();
            List<Node> leafNodes2 = new List<Node>();
            GetLeafNodes(CounterModel, ref leafNodes2);
            DistinctNodes = leafNodes2
                .GroupBy(node => new { node.Value, node.TruthValue })
                .Select(group => group.First())
                .Select(node => new Tuple<string, int>(node.Value, node.TruthValue))
                .ToList();
        return true;
        }

        void GetLeafNodes(Node tree, ref List<Node> leafNodes)
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

