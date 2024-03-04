
using System;
using System.Collections.Generic;

namespace VyrokovaLogikaPrace
{
   
    public class TreeProofAdvanced
    {
        public bool IsTautology { get; set; }
        public Node CounterModel { get; set; }
        public List<Tuple<string, int>> DistinctNodes { get; set; } = new List<Tuple<string, int>>();

        bool treeIsCompleted = true;
        Dictionary<string, int> nodes = new Dictionary<string, int>();
        bool contradiction;
        public List<Node> treees = new List<Node>();
        public void ProcessTree(Node tree, int truthValue = 0)
        {
            if (tree.IsLeaf)
            {
               
                return;
            }
            if (tree.Left != null)
            {
                var values = TreeHelper.GetValuesOfBothSides(truthValue, tree);
                //this is for just one variant
                tree.Left.TruthValue = values[0].Item1;
                if(tree.Left.IsLeaf) nodes[tree.Left.Value] = tree.Left.TruthValue;
                ProcessTree(tree.Left, values[0].Item1);
                if (tree.Right != null)
                {
                    tree.Right.TruthValue = values[0].Item2;
                    if (tree.Right.IsLeaf) nodes[tree.Right.Value] = tree.Right.TruthValue;
                }
            }
            if (tree.IsRoot)
            {
                tree.TruthValue = truthValue;
                FillSameLiteralsWithSameTruthValue(tree);
                FillTruthTree(tree);
                treeIsCompleted = true;
                IsTreeCompleted(GetToRoot(tree));
                if (treeIsCompleted)
                {
                    ContradictionHelper contradictionHelper = new ContradictionHelper();
                    if (contradictionHelper.FindContradiction(tree))
                    {
                        contradiction = true;
                    }
                    if (contradiction) IsTautology = true;
                    else IsTautology = false;
                    DistinctNodes = contradictionHelper.DistinctNodes;
                    CounterModel = contradictionHelper.CounterModel;
                }
            }
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

        private void FillSameLiteralsWithSameTruthValue(Node tree)
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
                FillSameLiteralsWithSameTruthValue(tree.Left);
                if(tree.Right != null)
                FillSameLiteralsWithSameTruthValue(tree.Right);
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
