using System.Collections.Generic;
using System;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace VyrokovaLogikaPrace
{
    public class TreeProof
    {
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
            return new List<Node> { new ValueNode(tree.Value, truthValue, 1) };
        }

    }
}

