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
        public bool Red { get; set; } = false;
        public int TruthValue { get; set; } = -1;

        public int TruthValue2 { get; set; } = -1;
        public int id { get; set; } //id of the node
        public bool IsLeaf { get; set; } // Indicates whether this node is a leaf (value) node
        public bool WillBeChanged { get; set; } = false;

        public List<(int, int)> UsedCombinations { get; set; }
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

        // Method to perform a deep copy of a tree
        public static Node DeepCopy(Node original, Dictionary<Node, Node> visitedNodes = null)
        {
            if (original == null)
            {
                return null;
            }

            // If the dictionary of visited nodes is not provided, create a new one
            if (visitedNodes == null)
            {
                visitedNodes = new Dictionary<Node, Node>();
            }

            // If the original node has already been visited, return its copy
            if (visitedNodes.ContainsKey(original))
            {
                return visitedNodes[original];
            }

            // Create a new node with the same value
            Node newNode = original.GetType()
                             .GetConstructor(new Type[] { typeof(int) })
                             .Invoke(new object[] { original.id }) as Node;
            newNode.Value = original.Value;
            newNode.TruthValue = original.TruthValue;
            newNode.isFinal = original.isFinal;
            newNode.IsLeaf = original.IsLeaf;

            // Add the original node and its copy to the dictionary of visited nodes
            visitedNodes.Add(original, newNode);

            // Parent should be set separately to avoid reference to the original tree
            newNode.Parent = null;

            if (original.Parent != null)
            {
                // If original has a parent, find the corresponding node in the copied tree
                newNode.Parent = DeepCopy(original.Parent, visitedNodes);
                newNode.Parent.id = original.Parent.id;
            }

            if (original.UsedCombinations != null)
            {
                newNode.UsedCombinations = new List<(int, int)>(original.UsedCombinations);
            }

            // Recursively copy the left subtree
            newNode.Left = DeepCopy(original.Left, visitedNodes);

            // Recursively copy the right subtree if it exists
            newNode.Right = DeepCopy(original.Right, visitedNodes);

            return newNode;
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

        public ValueNode(int id) : base(id)
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