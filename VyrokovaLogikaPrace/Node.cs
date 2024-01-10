using System.Security.Policy;

namespace VyrokovaLogikaPrace
{
    public class Node
    {
        public string Value { get; set; } // Value for leaf nodes (e.g., integer values)
        public Node Left { get; set; } // Left child node
        public Node Right { get; set; } // Right child node
        public Node Parent { get; set; } //parent of node
        public bool IsRoot => Parent == null;

        public bool IsValueNode { get; set; } // Indicates whether this node is a leaf (value) node

        public Node(string value)
        {
            Value = value;
            IsValueNode = true;
        }

        public Node(Node left, Node right, string value = "")
        {
            Left = left;
            Right = right;
            Left.Parent = this;
            Right.Parent = this;
            IsValueNode = false;
            Value = value;
        }

        public Node(Node left, string value = "")
        {
            Left = left;
            Left.Parent = this;
            IsValueNode = false;
            Value = value;
        }

        public Node()
        {
           
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

    public class DoubleNegationOperatorNode : Node
    {
        public DoubleNegationOperatorNode(Node operand, string value) : base(operand,value)
        {
        }

        public DoubleNegationOperatorNode() : base()
        {
        }
    }

    public class NegationOperatorNode : Node
    {
        public NegationOperatorNode(Node operand, string value) : base(operand, value)
        {
        }

        public NegationOperatorNode() : base()
        {
        }
    }


    public class DisjunctionOperatorNode : Node
    {
        public DisjunctionOperatorNode(Node left, Node right, string value) : base(left, right, value)
        {
        }

        public DisjunctionOperatorNode() : base ()
        {

        }
    }

    public class ConjunctionOperatorNode : Node
    {
        public ConjunctionOperatorNode(Node left, Node right, string value) : base(left, right, value)
        {
        }

        public ConjunctionOperatorNode() : base()
        {
        }
    }

    public class EqualityOperatorNode : Node
    {
        public EqualityOperatorNode(Node left, Node right, string value) : base(left, right, value)
        {
        }

        public EqualityOperatorNode() : base()
        {
        }
    }

    public class ImplicationOperatorNode : Node
    {
        public ImplicationOperatorNode(Node left, Node right, string value) : base(left, right, value)
        {
        }

        public ImplicationOperatorNode() : base()
        {
        }
    }

}
