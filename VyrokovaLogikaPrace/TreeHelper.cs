using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VyrokovaLogikaPrace
{
    public static class TreeHelper
    {
        public static bool Contradiction { get; set; }
        //get node and based of class it returns corresponding operator as string
        public static string GetOP(Node tree)
        {
            switch (tree)
            {
                case NegationOperatorNode _:
                    return "¬";
                case DoubleNegationOperatorNode _:
                    return "¬¬";
                case ConjunctionOperatorNode _:
                    return "∧";
                case DisjunctionOperatorNode _:
                    return "∨";
                case EqualityOperatorNode _:
                    return "≡";
                case ImplicationOperatorNode _:
                    return "⇒";
                case ValueNode _:
                    return ((ValueNode)tree).Value;
                default:
                    return string.Empty;
            }
        }
        
        //create from string with id, new nodes
        public static Node GetNode(string item, int id)
        {
            switch (item)
            {
                case "¬":
                    return new NegationOperatorNode(id);
                case "¬¬":
                    return new DoubleNegationOperatorNode(id);
                case "∧":
                    return new ConjunctionOperatorNode(id);
                case "∨":
                    return new DisjunctionOperatorNode(id);
                case "≡":
                    return new EqualityOperatorNode(id);
                case "⇒":
                    return new ImplicationOperatorNode(id);
            }
            return new ValueNode(item,id);
        }

        public static List<(int, int)> GetValuesOfBothSides(int parentMustBe, Node tree)
        {
            List<(int, int)> valuesList = new List<(int, int)>();
            switch (tree)
            {
                case ImplicationOperatorNode _:
                    if (parentMustBe == 0)
                    {
                        valuesList.Add((1, 0));
                    }
                    else
                    {
                        valuesList.Add((0, 1));
                        valuesList.Add((1, 1));
                        valuesList.Add((0, 0));
                    }
                    break;
                case DisjunctionOperatorNode _:
                    if (parentMustBe == 0)
                    {
                        valuesList.Add((0, 0));
                    }
                    else
                    {
                        valuesList.Add((0, 1));
                        valuesList.Add((1, 0));
                        valuesList.Add((1, 1));
                    }
                    break;
                case ConjunctionOperatorNode _:
                    if (parentMustBe == 1)
                    {
                        valuesList.Add((1, 1));
                    }
                    else
                    {
                        valuesList.Add((0, 1));
                        valuesList.Add((1, 0));
                        valuesList.Add((0, 0));
                    }
                    break;
                case EqualityOperatorNode _:
                    if (parentMustBe == 1)
                    {
                        valuesList.Add((1, 1));
                        valuesList.Add((0, 0));
                    }
                    else
                    {
                        valuesList.Add((0, 1));
                        valuesList.Add((1, 0));
                    }

                    break;

                case NegationOperatorNode _:
                    if (parentMustBe == 1)
                    {
                        valuesList.Add((0, -1));
                    }
                    else
                    {
                        valuesList.Add((1, -1));
                    }

                    break;
                case DoubleNegationOperatorNode _:
                    if (parentMustBe == 1)
                    {
                        valuesList.Add((1, -1));
                    }
                    else
                    {
                        valuesList.Add((0, -1));
                    }

                    break;
            }
            return valuesList;
        }

       
    }
}
