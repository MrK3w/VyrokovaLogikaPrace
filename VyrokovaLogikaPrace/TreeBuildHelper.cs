using System;

namespace VyrokovaLogikaPrace
{
    public static class TreeBuildHelper
    {
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

    }
}
