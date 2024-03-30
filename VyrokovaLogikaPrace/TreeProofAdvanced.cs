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
        List<Node> tempTreeByLiteral = new List<Node>();
        public List<string> steps { get; set; } = new List<string>();
        public List<Node> trees { get; set; } = new List<Node>();
        public List<string> triedToAssignLiterals = new List<string>();
        public TreeProofAdvanced(Node tree, int truthValue = 0)
        {
            Node tempTree = new Node(0);
            tempTree = Node.DeepCopy(GetToRoot(tree));
            tempTree.TruthValue = truthValue;
            tempTree.Blue = true;
            steps.Add("Přidáme " + truthValue + " na začátek stromu protože se snažíme dokázat");
            if (truthValue == 0)
            {
                steps[steps.Count - 1] += " tautologii";
            }
            else
            {
                steps[steps.Count - 1] += " kontradikci";
            }
            trees.Add(tempTree);
            ProcessTree(tree, truthValue);
            if(steps.Count == 1)
            {
                tempTree = new Node(0);
                tempTree = Node.DeepCopy(GetToRoot(tree));
                tempTree.TruthValue = truthValue;
                tempTree.Blue = true;
                steps.Add("Strom se skládá pouze z literálu, nemůže se jednat o");
                if (truthValue == 0)
                {
                    steps[steps.Count - 1] += " tautologii";
                }
                else
                {
                    steps[steps.Count - 1] += " kontradikci";
                }
                trees.Add(tempTree);
            }
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
            if (triedToAssignLiterals.Count == 0)
            {
                if (tree.Left != null)
                {
                    bool wasSomethingChanged = false;
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
                            RemoveBlue((GetToRoot(tree)));
                            tree.Left.Blue = true;
                            wasSomethingChanged = true;
                        }
                        if (tree.Left.IsLeaf) nodes[tree.Left.Value] = tree.Left.TruthValue;
                    }
                    if (tree.Left.IsLeaf) nodes[tree.Left.Value] = tree.Left.TruthValue;

                    if (tree.Right != null)
                    {
                        if (tree.Right.TruthValue == -1)
                        {
                            tree.Right.TruthValue = value.Item2;
                            tree.Right.Blue = true;
                            wasSomethingChanged = true;
                        }
                        if (tree.Right.IsLeaf) nodes[tree.Right.Value] = tree.Right.TruthValue;
                    }
                    if (wasSomethingChanged)
                    {
                        Node tempTree = new Node(0);
                        tempTree = Node.DeepCopy(GetToRoot(tree));
                        trees.Add(tempTree);
                        if (values.Count == 1)
                        {
                            steps.Add("Přidáme na levou stranu " + tree.Left.TruthValue);
                        }
                        else
                        {
                            steps.Add("Zkusíme přidat jako jednu z možností na levou stranu " + tree.Left.TruthValue);
                        }
                        if (tree.Right != null)
                        {
                            steps[steps.Count - 1] += " a na pravou stranu " + tree.Right.TruthValue;
                        }
                        steps[steps.Count - 1] += " protože operátor " + TreeHelper.GetOperatorName(tree) + " má pravdivostní hodnotu " + tree.TruthValue;
                    }
                    ProcessTree(tree.Left, value.Item1);
                }
            }
            if (tree.IsRoot)
            {
                treeIsCompleted = false;
                while (!treeIsCompleted)
                {
                    if (triedToAssignLiterals.Count == 0)
                    {
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
                        treeIsCompleted = true;
                        IsTreeCompleted(GetToRoot(tree));
                    }
                }

                RemoveBlue(GetToRoot(tree));
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
                    steps.Add("není zde spor");
                    steps[steps.Count - 1] += " nemůže se jednat o "; 
                    if(truthValue == 0)
                    {
                        steps[steps.Count - 1] += "tautologii";
                    }
                    else
                    {
                        steps[steps.Count - 1] += "kontradikci";
                    }
                    trees.Add(tree);
                    return;
                }
                else
                {
                    if (tree.TruthValue == 0)
                    {
                        if(moreOptions.Count != 0 || triedToAssignLiterals.Count != 0)
                        steps.Add("Je zde spor, takže se může jednat o tautologii, musíme se vrátit do bodu, kde jsme zkusili dosadit hodnotu");
                        else
                        {
                            steps.Add("Je zde spor, jedná se o tautologii vyzkoušeli jsme všechny možnosti");
                        }
                    }
                    else
                    {
                        if (moreOptions.Count != 0 || triedToAssignLiterals.Count != 0)
                            steps.Add("Je zde spor, takže se může jednat o kontradikci, musíme se vrátit do bodu, kde jsme zkusili dosadit hodnotu");
                        else
                        {
                            steps.Add("Je zde spor, jedná se o kontradikci vyzkoušeli jsme všechny možnosti");
                        }
                    }
                    trees.Add(CounterModel);
                }
                if(triedToAssignLiterals.Count != 0)
                {
                    
                    contradiction = false;
                    if (tempTreeByLiteral.Count != 0)
                    {
                        var t = tempTreeByLiteral[tempTreeByLiteral.Count - 1];
                        tempTreeByLiteral.RemoveAt(tempTreeByLiteral.Count - 1);
                        Node newTree = new Node(0);
                        RemoveBlue(t);
                        newTree = Node.DeepCopy(t);
                        trees.Add(newTree);
                        nodes.Remove(triedToAssignLiterals[triedToAssignLiterals.Count - 1]);
                        steps.Add("Vratíme se do bodu kde jsme zkoušeli dosazovat do " + triedToAssignLiterals[triedToAssignLiterals.Count - 1]);
                        ProcessTree(GetToRoot(t), GetToRoot(t).TruthValue);
                        return;
                    }
                }
                if (moreOptions.Count != 0)
                {
                    nodes = new Dictionary<string, int>();
                    var xd = moreOptions[moreOptions.Count - 1];
                    moreOptions.RemoveAt(moreOptions.Count - 1);
                    contradiction = false;
                    ProcessTree(xd,xd.TruthValue);
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
                        if (triedToAssignLiterals.Count != 0)
                        {
                            if (triedToAssignLiterals[triedToAssignLiterals.Count - 1] == tree.Value)
                            {
                                tree.TruthValue = 1;
                                RemoveBlue((GetToRoot(tree)));
                                tree.Blue = true;
                                Node tempTree = new Node(0);
                                tempTree = Node.DeepCopy(GetToRoot(tree));
                                trees.Add(tempTree);

                                steps.Add("Dosadíme do uzlu " + tree.Value + " hodnotu 1");
                                assignedLiteral = true;
                                triedToAssignLiterals.Remove(tree.Value);
                            }
                            else
                            {
                                var temporary = new Node(0);
                                temporary = Node.DeepCopy(GetToRoot(tree));
                                tempTreeByLiteral.Add(temporary);

                                tree.TruthValue = 0;
                                RemoveBlue((GetToRoot(tree)));
                                tree.Blue = true;
                                Node tempTree = new Node(0);
                                tempTree = Node.DeepCopy(GetToRoot(tree));
                                trees.Add(tempTree);

                                steps.Add("Můžeme zkusit dosadit do uzlu " + tree.Value + " hodnotu 0");
                                assignedLiteral = true;
                                triedToAssignLiterals.Add(tree.Value);
                            }
                        }

                        else
                        {
                                var temporary = new Node(0);
                                temporary = Node.DeepCopy(GetToRoot(tree));
                                tempTreeByLiteral.Add(temporary);

                                tree.TruthValue = 0;
                                RemoveBlue((GetToRoot(tree)));
                                tree.Blue = true;
                                Node tempTree = new Node(0);
                                tempTree = Node.DeepCopy(GetToRoot(tree));
                                trees.Add(tempTree);

                                steps.Add("Můžeme zkusit dosadit do uzlu " + tree.Value + " hodnotu 0");
                                assignedLiteral = true;
                                triedToAssignLiterals.Add(tree.Value);
                        }
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
                        RemoveBlue((GetToRoot(tree)));
                        tree.Parent.Blue = true;
                        Node tempTree = new Node(0);
                        tempTree = Node.DeepCopy(GetToRoot(tree));
                        trees.Add(tempTree);
                        steps.Add("Můžeme dosadit do uzlu negace negovanou hodnotu literálu");
                    }
                }
            }
            if (tree is ImplicationOperatorNode)
            {
                if (tree.Left.TruthValue == 1 && tree.Right.TruthValue == 1)
                {
                    if (tree.TruthValue == -1)
                    {
                        tree.TruthValue = 1;
                        RemoveBlue((GetToRoot(tree)));
                        tree.Blue = true;
                        Node tempTree = new Node(0);
                        tempTree = Node.DeepCopy(GetToRoot(tree));
                        trees.Add(tempTree);
                        steps.Add("Pokud operátor uzlu je implikace a levý uzel je 1 a zároveň pravý uzel je 1 můžeme dosadit do uzlu 1");

                    }
                }

                if (tree.Left.TruthValue == 0 && tree.Right.TruthValue == 1)
                {
                    if (tree.TruthValue == -1)
                    {
                        tree.TruthValue = 1;
                        RemoveBlue((GetToRoot(tree)));
                        tree.Blue = true;
                        Node tempTree = new Node(0);
                        tempTree = Node.DeepCopy(GetToRoot(tree));
                        trees.Add(tempTree);
                        steps.Add("Pokud operátor uzlu je implikace a levý uzel je 0 a zároveň pravý uzel je 1 můžeme dosadit do uzlu 1");

                    }
                }

                if (tree.Left.TruthValue == 0 && tree.Right.TruthValue == 0)
                {
                    if (tree.TruthValue == -1)
                    {
                        tree.TruthValue = 1;
                        RemoveBlue((GetToRoot(tree)));
                        tree.Blue = true;
                        Node tempTree = new Node(0);
                        tempTree = Node.DeepCopy(GetToRoot(tree));
                        trees.Add(tempTree);
                        steps.Add("Pokud operátor uzlu je implikace a levý uzel je 0 a zároveň pravý uzel je 0 můžeme dosadit do uzlu 1");

                    }
                }

                if (tree.Left.TruthValue == 1 && tree.Right.TruthValue == 0)
                {
                    if (tree.TruthValue == -1)
                    {
                        tree.TruthValue = 0;
                        RemoveBlue((GetToRoot(tree)));
                        tree.Blue = true;
                        Node tempTree = new Node(0);
                        tempTree = Node.DeepCopy(GetToRoot(tree));
                        trees.Add(tempTree);
                        steps.Add("Pokud operátor uzlu je implikace a levý uzel je 1 a zároveň pravý uzel je 0 můžeme dosadit do uzlu 0");

                    }
                }


                if (tree.TruthValue == -1 && tree.Left.TruthValue == 0 && tree.Right.TruthValue == 1)
                {
                    tree.TruthValue = 1;
                    RemoveBlue((GetToRoot(tree)));
                    tree.Blue = true;
                    Node tempTree = new Node(0);
                    tempTree = Node.DeepCopy(GetToRoot(tree));
                    trees.Add(tempTree);
                    steps.Add("Pokud operátor uzlu je implikace a levý uzel je 0 a zároveň hodnota pravého uzlu je 1 můžeme do  uzlu dosadit 1");
                }
            }
            if (tree is ConjunctionOperatorNode)
            {
                if (tree.Left.TruthValue != -1 && tree.Right.TruthValue != -1)
                {
                    if (tree.Left.TruthValue == 1 && tree.Right.TruthValue == 1)
                    {
                        if (tree.TruthValue == -1)
                        {
                            tree.TruthValue = 1;
                            RemoveBlue((GetToRoot(tree)));
                            tree.Blue = true;
                            Node tempTree = new Node(0);
                            tempTree = Node.DeepCopy(GetToRoot(tree));
                            trees.Add(tempTree);
                            steps.Add("Pokud operátor uzlu je konjunkce a levý uzel je 1 a zároveň hodnota pravého uzlu je 1 můžeme do  uzlu dosadit 1");
                        }
                    }
                    else
                    {
                        if (tree.TruthValue == -1)
                        {
                            tree.TruthValue = 0;
                            RemoveBlue((GetToRoot(tree)));
                            tree.Blue = true;
                            Node tempTree = new Node(0);
                            tempTree = Node.DeepCopy(GetToRoot(tree));
                            trees.Add(tempTree);
                            steps.Add("Pokud operátor uzlu je konjunkce a levý uzel není 1 a zároveň hodnota pravého uzlu není 1 můžeme do  uzlu dosadit 0");
                        }
                    }
                }
                if (tree.TruthValue == 1)
                {
                    if (tree.Left.TruthValue == -1)
                    {
                        steps.Add("Pokud operátor uzlu je konjunkce a hodnota uzlu je 1 můžeme do levého potomka dosadit 1");
                        tree.Left.TruthValue = 1;
                        RemoveBlue((GetToRoot(tree)));
                        tree.Left.Blue = true;
                        Node tempTree = new Node(0);
                        tempTree = Node.DeepCopy(GetToRoot(tree));
                        trees.Add(tempTree);
                    }
                    if (tree.Right.TruthValue == -1)
                    {
                        tree.Right.TruthValue = 1;
                        RemoveBlue((GetToRoot(tree)));
                        tree.Right.Blue = true;
                        steps.Add("Pokud operátor uzlu je konjunkce a hodnota uzlu je 0 můžeme do pravého uzlu dosadit 1");
                        Node tempTree = new Node(0);
                        tempTree = Node.DeepCopy(GetToRoot(tree));
                        trees.Add(tempTree);
                    }
                }
            }
            if (tree is DoubleNegationOperatorNode)
            {
                if (tree.TruthValue != -1)
                {
                    if (tree.Left.TruthValue == -1)
                    {
                        steps.Add("Pokud operátor uzlu je dvojitá negace můžeme do potomka dosadit stejnou hodnotu");
                        RemoveBlue((GetToRoot(tree)));
                        tree.Left.Blue = true;
                        tree.Left.TruthValue = tree.TruthValue;
                        Node tempTree = new Node(0);
                        tempTree = Node.DeepCopy(GetToRoot(tree));
                        trees.Add(tempTree);
                    }
                }
                if(tree.TruthValue == -1)
                { 
                    if(tree.Left.TruthValue != -1)
                    {
                        tree.TruthValue = tree.Left.TruthValue;
                        steps.Add("Pokud operátor uzlu je dvojitá negace můžeme do uzlu dosadit hodnotu potomku!");
                        RemoveBlue((GetToRoot(tree)));
                        tree.Blue = true;
                        tree.TruthValue = tree.Left.TruthValue;
                        Node tempTree = new Node(0);
                        tempTree = Node.DeepCopy(GetToRoot(tree));
                        trees.Add(tempTree);
                    }
                }
            }

            if (tree is EqualityOperatorNode)
            {
                if (tree.TruthValue == -1)
                {
                    if((tree.Left.TruthValue == 0 && tree.Right.TruthValue == 1)|| (tree.Left.TruthValue == 1 && tree.Right.TruthValue == 0))
                    {
                        steps.Add("Pokud operátor uzlu je ekvivalence a uzly potomků se neshodují dosadíme 0");
                        RemoveBlue((GetToRoot(tree)));
                        tree.Blue = true;
                        tree.TruthValue = 0;
                        Node tempTree = new Node(0);
                        tempTree = Node.DeepCopy(GetToRoot(tree));
                        trees.Add(tempTree);
                    }
                    if ((tree.Left.TruthValue == 1 && tree.Right.TruthValue == 1) || (tree.Left.TruthValue == 1 && tree.Right.TruthValue == 1))
                    {
                        steps.Add("Pokud operátor uzlu je ekvivalence a uzly potomků se shodují dosadíme 1");
                        RemoveBlue((GetToRoot(tree)));
                        tree.Blue = true;
                        tree.TruthValue = 1;
                        Node tempTree = new Node(0);
                        tempTree = Node.DeepCopy(GetToRoot(tree));
                        trees.Add(tempTree);
                    }
                    if(tree.TruthValue == 1)
                    {
                        if(tree.Left.TruthValue == -1)
                        {
                            if (tree.Right.TruthValue == 1)
                            {
                                steps.Add("Pokud operátor uzlu je ekvivalence a hodnota uzlu je 1 a pravý potomek má hodnotu uzlu 1 můžeme do levého uzlu dosadit 1");
                                RemoveBlue((GetToRoot(tree)));
                                tree.Blue = true;
                                tree.Left.TruthValue = 1;
                                Node tempTree = new Node(0);
                                tempTree = Node.DeepCopy(GetToRoot(tree));
                                trees.Add(tempTree);

                            }
                        }
                        else if(tree.Right.TruthValue == -1)
                        {
                            if (tree.Left.TruthValue == 1)
                            {
                                steps.Add("Pokud operátor uzlu je ekvivalence a hodnota uzlu je 1 a levý potomek má hodnotu uzlu 1 můžeme do pravého uzlu dosadit 1");
                                RemoveBlue((GetToRoot(tree)));
                                tree.Blue = true;
                                tree.Right.TruthValue = 1;
                                Node tempTree = new Node(0);
                                tempTree = Node.DeepCopy(GetToRoot(tree));
                                trees.Add(tempTree);

                            }
                        }
                    }
                    else if(tree.TruthValue == 0)
                    {
                        if (tree.Left.TruthValue == -1)
                        {
                            if (tree.Right.TruthValue == 1)
                            {
                                steps.Add("Pokud operátor uzlu je ekvivalence a hodnota uzlu je 0 a pravý potomek má hodnotu uzlu 1 můžeme do levého uzlu dosadit 0");
                                RemoveBlue((GetToRoot(tree)));
                                tree.Blue = true;
                                tree.Left.TruthValue = 0;
                                Node tempTree = new Node(0);
                                tempTree = Node.DeepCopy(GetToRoot(tree));
                                trees.Add(tempTree);

                            }
                            else if (tree.Right.TruthValue == 0)
                            {
                                steps.Add("Pokud operátor uzlu je ekvivalence a hodnota uzlu je 0 a pravý potomek má hodnotu uzlu 0 můžeme do levého uzlu dosadit 1");
                                RemoveBlue((GetToRoot(tree)));
                                tree.Blue = true;
                                tree.Left.TruthValue = 1;
                                Node tempTree = new Node(0);
                                tempTree = Node.DeepCopy(GetToRoot(tree));
                                trees.Add(tempTree);
                            }
                        }
                        else if (tree.Right.TruthValue == -1)
                        {
                            if (tree.Left.TruthValue == 1)
                            {
                                steps.Add("Pokud operátor uzlu je ekvivalence a hodnota uzlu je 0 a levý potomek má hodnotu uzlu 1 můžeme do pravého uzlu dosadit 0");
                                RemoveBlue((GetToRoot(tree)));
                                tree.Blue = true;
                                tree.Right.TruthValue = 0;
                                Node tempTree = new Node(0);
                                tempTree = Node.DeepCopy(GetToRoot(tree));
                                trees.Add(tempTree);
                            }
                            if (tree.Left.TruthValue == 0)
                            {
                                steps.Add("Pokud operátor uzlu je ekvivalence a hodnota uzlu je 0 a levý potomek má hodnotu uzlu 0 můžeme do pravého uzlu dosadit 1");
                                RemoveBlue((GetToRoot(tree)));
                                tree.Blue = true;
                                tree.Right.TruthValue = 0;
                                Node tempTree = new Node(0);
                                tempTree = Node.DeepCopy(GetToRoot(tree));
                                trees.Add(tempTree);
                            }
                        }
                    }
                    if(tree.TruthValue == -1)
                    {
                        if((tree.Left.TruthValue == 1 && tree.Right.TruthValue == 1) || (tree.Left.TruthValue == 0 && tree.Right.TruthValue == 0))
                        {
                            tree.TruthValue = 1;

                            steps.Add("Pokud operátor uzlu je ekvivalence a levý a pravý uzel mají stejnou pravdivostní hodnotu můžeme do uzlu dosadit 1");
                            RemoveBlue((GetToRoot(tree)));
                            tree.Blue = true;
                            tree.TruthValue = 1;
                            Node tempTree = new Node(0);
                            tempTree = Node.DeepCopy(GetToRoot(tree));
                            trees.Add(tempTree);

                        }
                        else if ((tree.Left.TruthValue == 1 && tree.Right.TruthValue == 0) || (tree.Left.TruthValue == 0 && tree.Right.TruthValue == 1))
                        {
                            steps.Add("Pokud operátor uzlu je ekvivalence a levý a pravý uzel mají jimou pravdivostní hodnotu můžeme do uzlu dosadit 0");
                            RemoveBlue((GetToRoot(tree)));
                            tree.Blue = true;
                            tree.TruthValue = 0;
                            Node tempTree = new Node(0);
                            tempTree = Node.DeepCopy(GetToRoot(tree));
                            trees.Add(tempTree);
                        }
                    }
                }
            }
            if (tree is DisjunctionOperatorNode)
            {
                if (tree.Left.TruthValue == 0 && tree.Right.TruthValue == 0)
                {
                    if (tree.TruthValue == -1)
                    {
                        steps.Add("Pokud operátor uzlu je disjunkce a zároveň hodnota levého i pravého potomka je 0 můžeme do uzlu dosadit 0");
                        tree.TruthValue = 0;
                        RemoveBlue((GetToRoot(tree)));
                        tree.Blue = true;
                        Node tempTree = new Node(0);
                        tempTree = Node.DeepCopy(GetToRoot(tree));
                        trees.Add(tempTree);
                    }
                }
                if (tree.Left.TruthValue == 1 || tree.Right.TruthValue == 1)
                {
                    if (tree.TruthValue == -1)
                    {
                        steps.Add("Pokud operátor uzlu je disjunkce a zároveň hodnota levého nebo pravého potomka je 1 můžeme do uzlu dosadit 1");
                        tree.TruthValue = 1;
                        RemoveBlue((GetToRoot(tree)));
                        tree.Blue = true;
                        Node tempTree = new Node(0);
                        tempTree = Node.DeepCopy(GetToRoot(tree));
                        trees.Add(tempTree);
                    }
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
                if (tree.TruthValue == -1)
                {
                    if (nodes.ContainsKey(tree.Value))
                    {
                        tree.TruthValue = nodes[tree.Value];
                        RemoveBlue((GetToRoot(tree)));
                        tree.Blue = true;
                        Node tempTree = new Node(0);
                        tempTree = Node.DeepCopy(GetToRoot(tree));
                        steps.Add("Přidáme do uzlu " + tree.Value + " hodnotu " + tree.TruthValue + " protože ji známe");
                        trees.Add(tempTree);
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

        private void RemoveBlue(Node tree)
        {
            tree.Blue = false;
            if (tree.Left != null)
            {

                RemoveBlue(tree.Left);
            }
            if (tree.Right != null)
            {
                RemoveBlue(tree.Right);
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