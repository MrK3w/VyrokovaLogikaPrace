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

        public Node(Node left, Node right)
        {
            Left = left;
            Right = right;
            IsValueNode = false;
        }

        public Node(Node left)
        {
            Left = left;
            IsValueNode = false;
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
        public BinaryOperatorNode(Node left, Node right) : base(left, right)
        {
        }
    }

    public abstract class UnaryOperatorNode : Node
    {
        public UnaryOperatorNode(Node operand) : base(operand) { }
    }

    public class DoubleNegationOperatorNode : UnaryOperatorNode
    {
        public DoubleNegationOperatorNode(Node operand) : base(operand)
        {
        }
    }

    public class NegationOperatorNode : UnaryOperatorNode
    {
        public NegationOperatorNode(Node operand) : base(operand)
        {
        }
    }


    public class DisjunctionOperatorNode : BinaryOperatorNode
    {
        public DisjunctionOperatorNode(Node left, Node right) : base(left, right)
        {
        }
    }

    public class ConjunctionOperatorNode : BinaryOperatorNode
    {
        public ConjunctionOperatorNode(Node left, Node right) : base(left, right)
        {
        }
    }

    public class EqualityOperatorNode : BinaryOperatorNode
    {
        public EqualityOperatorNode(Node left, Node right) : base(left, right)
        {
        }
    }

    public class ImplicationOperatorNode : BinaryOperatorNode
    {
        public ImplicationOperatorNode(Node left, Node right) : base(left, right)
        {
        }
    }

}
