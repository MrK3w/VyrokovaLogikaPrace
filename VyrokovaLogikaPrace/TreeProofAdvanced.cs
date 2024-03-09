﻿
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
        public List<Node> moreOptions = new List<Node>();


        public void ProcessTree(Node tree, int truthValue = 0)
        {
            if (tree.IsLeaf)
            {

                return;
            }
            if (tree.IsRoot)
            {
                tree.TruthValue = truthValue;
            }
            if (tree.Left != null)
            {
                var values = TreeHelper.GetValuesOfBothSides(truthValue, tree);
                var value = values[0];
                if(tree.UsedCombinations != null && tree.UsedCombinations.Count > 0)
                {
                        value = tree.UsedCombinations[tree.UsedCombinations.Count - 1];
                        tree.UsedCombinations.RemoveAt(tree.UsedCombinations.Count - 1);
                    if (tree.UsedCombinations.Count > 1)
                    {
                        Node tr = new Node(0);

                        tr = Node.DeepCopy(GetToRoot(tree));
                        moreOptions.Add(tr);

                    }
                }
                if (values.Count > 1 && tree.UsedCombinations == null)
                {

                    Node tr = new Node(0);
                    tree.UsedCombinations = new List<(int, int)>();
                    tree.UsedCombinations = values;

                    tr = Node.DeepCopy(GetToRoot(tree));
                    moreOptions.Add(tr);
                }
                //this is for just one variant
                if (tree.Left.TruthValue == -1)
                {
                    tree.Left.TruthValue = value.Item1;
                }
                if(tree.Left.IsLeaf) nodes[tree.Left.Value] = tree.Left.TruthValue;
                ProcessTree(tree.Left, value.Item1);
                if (tree.Right != null)
                {
                    if (tree.Right.TruthValue == -1)
                        tree.Right.TruthValue = value.Item2;
                    if (tree.Right.IsLeaf) nodes[tree.Right.Value] = tree.Right.TruthValue;
                }
            }
            if (tree.IsRoot)
            {
               
                FillSameLiteralsWithSameTruthValue(tree);
                FillTruthTree(tree);
                treeIsCompleted = true;
                IsTreeCompleted(GetToRoot(tree));

                ContradictionHelper contradictionHelper = new ContradictionHelper();
                if (contradictionHelper.FindContradiction(tree))
                {
                    contradiction = true;
                }
                if (contradiction)
                {
                    IsTautology = true;
                }
                else IsTautology = false;
                DistinctNodes = contradictionHelper.DistinctNodes;
                CounterModel = contradictionHelper.CounterModel;
                if(moreOptions.Count != 0)
                {
                    nodes = new Dictionary<string, int>();
                    var xd = moreOptions[moreOptions.Count - 1];
                    moreOptions.RemoveAt(moreOptions.Count - 1);
                    ProcessTree(xd);
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
            if(tree is NegationOperatorNode)
            {
                if (tree.TruthValue != -1)
                {
                    if (tree.Left.TruthValue != -1 && (tree.TruthValue ^ 1) != tree.Left.TruthValue)
                    {
                        tree.Left.TruthValue2 = tree.TruthValue ^ 1;
                    }
                    else
                    {
                        if (tree.Left.TruthValue != -1)
                        {
                            tree.Left.TruthValue = tree.TruthValue ^ 1;
                        }


                    }
                }
            }
            if(tree is ImplicationOperatorNode)
            {
                if(tree.Left.TruthValue != -1 && tree.Right.TruthValue != -1)
                {
                    if (tree.Left.TruthValue == 1 && tree.Right.TruthValue == 0)
                    {
                        tree.TruthValue = 0;
                    }
                    else tree.TruthValue = 1;
                }
            }
            if (tree is DoubleNegationOperatorNode)
            {
                if (tree.TruthValue != -1)
                {
                    if (tree.Left.TruthValue != -1 && tree.TruthValue != tree.Left.TruthValue)
                    {
                        tree.Left.TruthValue2 = tree.TruthValue;
                    }
                    else tree.Left.TruthValue = tree.TruthValue;
                }
            }
            // Recursively traverse left and right subtrees
            if (tree.Left != null)
                    FillTruthTree(tree.Left);
                if (tree.Right != null)
                    FillTruthTree(tree.Right);

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
