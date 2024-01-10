using System;

namespace VyrokovaLogikaPrace
{
    public static class TreeBuildHelper
    {
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

        public static Node GetNode(string item)
        {
            switch (item)
            {
                case "¬":
                    return new NegationOperatorNode();
                case "¬¬":
                    return new DoubleNegationOperatorNode();
                case "∧":
                    return new ConjunctionOperatorNode();
                case "∨":
                    return new DisjunctionOperatorNode();
                case "≡":
                    return new EqualityOperatorNode();
                case "⇒":
                    return new ImplicationOperatorNode();
            }
            return new ValueNode(item);
        }

    }
}
