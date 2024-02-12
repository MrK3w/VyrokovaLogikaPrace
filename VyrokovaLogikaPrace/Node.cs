using System;
using System.Collections.Generic;

namespace VyrokovaLogikaPrace
{
    public class Node
    {
        public string Value { get; set; } // Value for leaf nodes
        public Node Left { get; set; } // Left child node
        public Node Right { get; set; } // Right child node
        public Node Parent { get; set; } // Parent of node

        public int ParentId { get; set; }
        public bool IsRoot => Parent == null; //if it is root value is true, otherwise it is false

        public int TruthValue { get; set; } = -1;
        public int id { get; set; } //id of the node
        public bool IsLeaf { get; set; } // Indicates whether this node is a leaf (value) node

        public List<(int,int)> UsedCombinations { get; set; }
        public bool isFinal {get;set;}
        public Node(string value, int id)
        {
            Value = value;
            this.id = id;
            IsLeaf = true;
        }

        public Node(string value,int truthValue, int id = 0)
        {
            Value = value;
            this.id = id;
            IsLeaf = true;
            TruthValue = truthValue;
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

            IsLeaf = false;
            Value = value;
        }

        public Node(Node left, int id, string value = "")
        {
            Left = left;
            this.id = id;
            Left.Parent = this;
            Left.Parent.id = this.id;
            IsLeaf = false;
            Value = value;
        }

        public Node(int id)
        {
            this.id = id;
        }
    }
    //for nodes which are literals
    public class ValueNode : Node
    {
        public ValueNode(string value, int id) : base(value, id)
        {
        }

        public ValueNode(string value,int truthValue, int id) : base(value, truthValue, id)
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