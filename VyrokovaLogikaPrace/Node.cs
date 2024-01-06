namespace VyrokovaLogikaPrace
{
    public class Node
    {
        public string Value { get; set; } // Value for leaf nodes (e.g., integer values)
        public Node Left { get; set; } // Left child node
        public Node Right { get; set; } // Right child node

        public bool IsValueNode { get; set; } // Indicates whether this node is a leaf (value) node

        public Node(string value)
        {
            Value = value;
            IsValueNode = true;
        }

        public Node(Node left, Node right, string value)
        {
            Left = left;
            Right = right;
            IsValueNode = false;
            Value = value;
        }

        public Node(Node left, string value)
        {
            Left = left;
            IsValueNode = false;
            Value = value;
        }
    }

    public class ValueNode : Node
    {
        public ValueNode(string value) : base(value)
        {
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class BinaryOperatorNode : Node
    {
        public BinaryOperatorNode(Node left, Node right, string value) : base(left, right, value)
        {
        }
    }

    public abstract class UnaryOperatorNode : Node
    {
        public UnaryOperatorNode(Node operand, string value) : base(operand, value) { }
    }

    public class DoubleNegationOperatorNode : UnaryOperatorNode
    {
        public DoubleNegationOperatorNode(Node operand, string value) : base(operand,value)
        {
        }
    }

    public class NegationOperatorNode : UnaryOperatorNode
    {
        public NegationOperatorNode(Node operand, string value) : base(operand, value)
        {
        }
    }


    public class DisjunctionOperatorNode : BinaryOperatorNode
    {
        public DisjunctionOperatorNode(Node left, Node right, string value) : base(left, right, value)
        {
        }
    }

    public class ConjunctionOperatorNode : BinaryOperatorNode
    {
        public ConjunctionOperatorNode(Node left, Node right, string value) : base(left, right, value)
        {
        }
    }

    public class EqualityOperatorNode : BinaryOperatorNode
    {
        public EqualityOperatorNode(Node left, Node right, string value) : base(left, right, value)
        {
        }
    }

    public class ImplicationOperatorNode : BinaryOperatorNode
    {
        public ImplicationOperatorNode(Node left, Node right, string value) : base(left, right, value)
        {
        }
    }

}
