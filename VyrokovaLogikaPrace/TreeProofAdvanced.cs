
using System;
using System.Collections.Generic;

namespace VyrokovaLogikaPrace
{
   
    public class TreeProofAdvanced
    {
        public bool IsTautology { get; set; }
        public Node CounterModel { get; set; }
        public List<Tuple<string, int>> DistinctNodes { get; set; } = new List<Tuple<string, int>>();
        Node tempTree = new Node(0);
        bool treeIsCompleted = true;
        Dictionary<string, int> nodes;
        bool contradiction;
        public void ProcessTree(Node tree, int truthValue = 0)
        {
            //if tree is leaf we will try to fill all other leaf if already there is same value node
            if(tree.IsLeaf)
            {
                List<Node> leafNodes = new List<Node>();
                nodes = new Dictionary<string, int>();
                ContradictionHelper contradiction = new ContradictionHelper();
                contradiction.GetLeafNodes(GetToRoot(tree), ref leafNodes);
                foreach (var leafNode in leafNodes)
                {
                    if (leafNode.TruthValue != -1) nodes.Add(leafNode.Value, leafNode.TruthValue);
                }
                FillSameTruthWithSameValues(GetToRoot(tree));
                return;
            }
            //if tree is root we will assign truth value to root
            if (tree.IsRoot) tree.TruthValue = truthValue;
            //if we did not created combinations for node we will do it now
            if(tree.UsedCombinations == null)
            {
                tree.UsedCombinations = TreeHelper.GetValuesOfBothSides(truthValue, tree);
            }
            //if there is left only one combination we will mark it as final
            if(tree.UsedCombinations.Count == 1)
            {
                tree.isFinal = true;
            }
            //we will try one of options and remove it from lsit
            var truthValues = tree.UsedCombinations[tree.UsedCombinations.Count - 1];
            tree.UsedCombinations.RemoveAt(tree.UsedCombinations.Count - 1);
            //if tree is not final we will save this true for later use
            if (!tree.isFinal)
            {
                tempTree = Node.DeepCopy(GetToRoot(tree));
            }
            //if tree has left or right side we will assign truth value for that
            if (tree.Left != null) tree.Left.TruthValue = truthValues.Item1;
            if(tree.Right != null) tree.Right.TruthValue = truthValues.Item2;
            //has tree complete all sides
            IsTreeCompleted(GetToRoot(tree));
           
            //if tree is not completed we will try to go left side
            if (tree.Left != null)
            {
                ProcessTree(tree.Left, tree.Left.TruthValue);
            }
          
           if (tree.IsRoot)
           {
                FillTruthTree(tree);
                treeIsCompleted = true;
                IsTreeCompleted(GetToRoot(tree));
                if(treeIsCompleted)
                {
                    ContradictionHelper contradictionHelper = new ContradictionHelper();
                    if(contradictionHelper.FindContradiction(tree))
                    {
                        contradiction = true;
                    }
                    if (tree.isFinal)
                    {
                        if (contradiction) IsTautology = true;
                        else IsTautology = false;
                        DistinctNodes = contradictionHelper.DistinctNodes;
                        CounterModel = contradictionHelper.CounterModel;
                    }
                    else
                    {
                        ProcessTree(tempTree, 0);
                    }
                   
                }    
           }
            return;
        }

        private void FillTruthTree(Node tree)
        {
            if (tree == null)
                return;

            if (tree.IsLeaf && tree.TruthValue != -1) // Leaf node
            {
                if (tree.Parent.TruthValue == -1)
                {
                    if (tree.Parent is NegationOperatorNode)
                    {
                        tree.Parent.TruthValue = tree.TruthValue ^ 1;
                    }
                }
            }
            else
            {
                // Recursively traverse left and right subtrees
                if (tree.Left != null)
                    FillTruthTree(tree.Left);
                if (tree.Right != null)
                    FillTruthTree(tree.Right);
            }
        }

        private void FillSameTruthWithSameValues(Node tree)
        {
            if (tree == null)
                return;

            if (tree.IsLeaf) // Leaf node
            {
                if(tree.TruthValue == -1)
                {
                    if (nodes.ContainsKey(tree.Value))
                    {
                        tree.TruthValue = nodes[tree.Value];
                    }
                }
            }
            else
            {
                // Recursively traverse left and right subtrees
                if(tree.Left != null)
                FillSameTruthWithSameValues(tree.Left);
                if(tree.Right != null)
                FillSameTruthWithSameValues(tree.Right);
            }
        }


        private void IsTreeCompleted(Node tree)
        {
            if (tree.TruthValue == -1)
                treeIsCompleted = false;
            if (tree.Left != null)
            {
                IsTreeCompleted(tree.Left);
            }
            if (tree.Right != null)
            {
                IsTreeCompleted(tree.Right);
            }
        }

        private Node GetToRoot(Node tree)
        {
            if(tree.IsRoot == false)
            {
                return GetToRoot(tree.Parent);
            }
            return tree;
        }
    }
}
