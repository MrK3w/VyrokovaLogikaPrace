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
        private bool assignedLiteral = false;

        public List<Node> trees { get; set; } = new List<Node>();

        public TreeProofAdvanced(Node tree, int truthValue = 0)
        {
            Node tempTree = new Node(0);
            tempTree = Node.DeepCopy(GetToRoot(tree));
            tempTree.TruthValue = truthValue;
            trees.Add(tempTree);
            ProcessTree(tree, truthValue);
        }

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
                if (tree.UsedCombinations != null && tree.UsedCombinations.Count > 1)
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

                if (tree.Left != null)
                {
                    if (tree.Left.TruthValue == -1)
                    {
                        tree.Left.TruthValue = value.Item1;
                    }
                    if (tree.Left.IsLeaf) nodes[tree.Left.Value] = tree.Left.TruthValue;
                }
                if (tree.Left.IsLeaf) nodes[tree.Left.Value] = tree.Left.TruthValue;

                if (tree.Right != null)
                {
                    if (tree.Right.TruthValue == -1)
                    {
                        tree.Right.TruthValue = value.Item2;
                        Node tempTree = new Node(0);
                        tempTree = Node.DeepCopy(GetToRoot(tree));
                        trees.Add(tempTree);
                    }
                    if (tree.Right.IsLeaf) nodes[tree.Right.Value] = tree.Right.TruthValue;
                }
                ProcessTree(tree.Left, value.Item1);
            }
            if (tree.IsRoot)
            {
                FillSameLiteralsWithSameTruthValue(tree);
                FillTruthTree(tree);
                treeIsCompleted = true;
                IsTreeCompleted(GetToRoot(tree));

                if (!treeIsCompleted)
                {
                    nodes = new Dictionary<string, int>();
                    FillNodes(tree);
                    FillSameLiteralsWithSameTruthValue(tree);
                    FillTruthTree(tree);
                }
                IsTreeCompleted(GetToRoot(tree));
                if (!treeIsCompleted)
                {
                    assignedLiteral = false;
                    TryToAssignValueToLiteral(tree);
                    nodes = new Dictionary<string, int>();
                    FillNodes(tree);
                    FillSameLiteralsWithSameTruthValue(tree);
                    FillTruthTree(tree);
                }
                ContradictionHelper contradictionHelper = new ContradictionHelper();
                if (contradictionHelper.FindContradiction(tree))
                {
                    contradiction = true;
                }
                if (!contradiction)
                {
                    IsTautology = false;
                }
                else IsTautology = true;

                DistinctNodes = contradictionHelper.DistinctNodes;
                CounterModel = contradictionHelper.CounterModel;
                if (!IsTautology)
                {
                    trees.Add(tree);
                    return;
                }
                else
                {
                    trees.Add(CounterModel);
                }
                if (moreOptions.Count != 0)
                {
                    nodes = new Dictionary<string, int>();
                    var xd = moreOptions[moreOptions.Count - 1];
                    moreOptions.RemoveAt(moreOptions.Count - 1);
                    contradiction = false;
                    ProcessTree(xd);
                }

            }
        }

        private void TryToAssignValueToLiteral(Node tree)
        {
            if (tree == null)
                return;

            if (tree.IsLeaf) // Leaf node
            {
                if (tree.TruthValue == -1)
                {
                    if(assignedLiteral == false)
                    {
                        tree.TruthValue = 0;
                        assignedLiteral = true;
                    }
                }
            }
            else
            {
                // Recursively traverse left and right subtrees
                if (tree.Left != null)
                    TryToAssignValueToLiteral(tree.Left);
                if (tree.Right != null)
                    TryToAssignValueToLiteral(tree.Right);
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
            if (tree is NegationOperatorNode)
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
                        else if (tree.Left.TruthValue == -1)
                        {
                            tree.Left.TruthValue = tree.TruthValue ^ 1;
                        }

                    }
                }
            }
            if (tree is ImplicationOperatorNode)
            {
                if (tree.Left.TruthValue != -1 && tree.Right.TruthValue != -1)
                {
                    if (tree.Left.TruthValue == 1 && tree.Right.TruthValue == 0)
                    {
                        tree.TruthValue = 0;
                    }
                    else tree.TruthValue = 1;
                }
                if (tree.TruthValue == 1 && tree.Left.TruthValue == 0)
                {
                    tree.Right.TruthValue = 1;
                }
                if(tree.TruthValue == -1 && tree.Left.TruthValue == 0 && tree.Right.TruthValue == 1)
                {
                    tree.TruthValue = 1;
                }
            }
            if (tree is ConjunctionOperatorNode)
            {
                if (tree.Left.TruthValue != -1 && tree.Right.TruthValue != -1)
                {
                    if (tree.Left.TruthValue == 1 && tree.Right.TruthValue == 1)
                    {
                        tree.TruthValue = 1;
                    }
                    else tree.TruthValue = 0;
                }
                if (tree.TruthValue == 1)
                {
                    if (tree.Left.TruthValue == -1) tree.Left.TruthValue = 1;
                    else if (tree.Left.TruthValue == 0) tree.Left.TruthValue2 = 1;
                    if (tree.Right.TruthValue == -1) tree.Right.TruthValue = 1;
                    else if (tree.Right.TruthValue == 0) tree.Right.TruthValue2 = 1;
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
            if (tree is DisjunctionOperatorNode)
            {
                if (tree.Left.TruthValue == 0 && tree.Right.TruthValue == 0) tree.TruthValue = 0;
                if (tree.Left.TruthValue == 1 || tree.Right.TruthValue == 1) tree.TruthValue = 1;


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
                if (tree.TruthValue == -1)
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
                if (tree.Left != null)
                    FillSameLiteralsWithSameTruthValue(tree.Left);
                if (tree.Right != null)
                    FillSameLiteralsWithSameTruthValue(tree.Right);
            }
        }

        private void FillNodes(Node tree)
        {
            if (tree.Left != null)
            {
                if (tree.Left.IsLeaf && tree.Left.TruthValue != -1) nodes[tree.Left.Value] = tree.Left.TruthValue;
                FillNodes(tree.Left);
            }
            if (tree.Right != null)
            {
                if (tree.Right.IsLeaf && tree.Right.TruthValue != -1) nodes[tree.Right.Value] = tree.Right.TruthValue;
                FillNodes(tree.Right);
            }
            return;
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
            if (tree.IsRoot == false)
            {
                return GetToRoot(tree.Parent);
            }
            return tree;
        }
    }
}