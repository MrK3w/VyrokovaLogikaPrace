namespace VyrokovaLogikaPrace
{
    public class Node
    {
        public string Value { get; set; } // Value for leaf nodes
        public Node Left { get; set; } // Left child node
        public Node Right { get; set; } // Right child node
        public Node Parent { get; set; } // Parent of node

        public int ParentId { get; set; }
        public bool IsRoot => Parent == null;

        public int id { get; set; }
        public bool IsValueNode { get; set; } // Indicates whether this node is a leaf (value) node

        public Node(string value, int id)
        {
            Value = value;
            this.id = id;
            IsValueNode = true;
        }

        public Node(Node left, Node right, int id, string value = "")
        {
            Left = left;
            this.id = id;
            Right = right;

            // Set parent and parent ID for left child
            Left.Parent = this;
            Left.ParentId = this.id;

            // Set parent and parent ID for right child
            Right.Parent = this;
            Right.ParentId = this.id;

            IsValueNode = false;
            Value = value;
        }

        public Node(Node left, int id, string value = "")
        {
            Left = left;
            this.id = id;
            Left.Parent = this;
            Left.Parent.id = this.id;
            IsValueNode = false;
            Value = value;
        }

        public Node(int id)
        {
            this.id = id;
        }
    }

    public class ValueNode : Node
    {
        public ValueNode(string value, int id) : base(value, id)
        {
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class DoubleNegationOperatorNode : Node
    {
        public DoubleNegationOperatorNode(Node operand, int id, string value) : base(operand, id, value)
        {
        }

        public DoubleNegationOperatorNode(int id) : base(id)
        {
        }
    }

    public class NegationOperatorNode : Node
    {
        public NegationOperatorNode(Node operand, int id, string value) : base(operand, id, value)
        {
        }

        public NegationOperatorNode(int id) : base(id)
        {
        }
    }

    public class DisjunctionOperatorNode : Node
    {
        public DisjunctionOperatorNode(Node left, Node right, int id, string value) : base(left, right, id, value)
        {
        }

        public DisjunctionOperatorNode(int id) : base(id)
        {

        }
    }

    public class ConjunctionOperatorNode : Node
    {
        public ConjunctionOperatorNode(Node left, Node right, int id, string value) : base(left, right, id, value)
        {
        }

        public ConjunctionOperatorNode(int id) : base(id)
        {
        }
    }

    public class EqualityOperatorNode : Node
    {
        public EqualityOperatorNode(Node left,Node right, int id, string value) : base(left, right, id, value)
        {
        }

        public EqualityOperatorNode(int id) : base(id)
        {
        }
    }

    public class ImplicationOperatorNode : Node
    {
        public ImplicationOperatorNode(Node left, Node right, int id, string value) : base(left, right, id, value)
        {
        }

        public ImplicationOperatorNode(int id) : base(id)
        {
        }
    }
}