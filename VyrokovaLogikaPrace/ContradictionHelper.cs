﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogikaPrace
{
    public class ContradictionHelper
    {
        public List<Tuple<string, int>> DistinctNodes { get; set; } = new List<Tuple<string, int>>();
        public Node CounterModel { get; set; } = new Node(0);
        public bool FindContradiction(List<Node> Trees)
        {
            foreach (var tree in Trees)
            {
                bool contradiction = false;
                List<Node> leafNodes = new List<Node>();
                GetLeafNodes(tree, ref leafNodes);
                foreach (var node in leafNodes)
                {
                    if (contradiction) break;
                    foreach (var node1 in leafNodes)
                    {
                        if (node1.Value == node.Value && node1.TruthValue != node.TruthValue)
                            contradiction = true;
                    }
                }
                if (contradiction == false)
                {
                    DistinctNodes = leafNodes
                       .GroupBy(node => new { node.Value, node.TruthValue })
                       .Select(group => group.First())
                       .Select(node => new Tuple<string, int>(node.Value, node.TruthValue))
                       .ToList();
                    CounterModel = tree;
                    return false;

                }
            }
            CounterModel = Trees.LastOrDefault();
            List<Node> leafNodes2 = new List<Node>();
            GetLeafNodes(CounterModel, ref leafNodes2);
            DistinctNodes = leafNodes2
                .GroupBy(node => new { node.Value, node.TruthValue })
                .Select(group => group.First())
                .Select(node => new Tuple<string, int>(node.Value, node.TruthValue))
                .ToList();
            return true;
        }

        public bool FindContradiction(Node tree)
        {
            bool contradiction = false;
            List<Node> leafNodes = new List<Node>();
            GetLeafNodes(tree, ref leafNodes);
            foreach (var node in leafNodes)
            {
                if (contradiction) break;
                foreach (var node1 in leafNodes)
                {
                    if (node1.Value == node.Value && node1.TruthValue != node.TruthValue)
                        contradiction = true;
                }
            }
            if (contradiction == false)
            {
                DistinctNodes = leafNodes
                    .GroupBy(node => new { node.Value, node.TruthValue })
                    .Select(group => group.First())
                    .Select(node => new Tuple<string, int>(node.Value, node.TruthValue))
                    .ToList();
                CounterModel = tree;
                return false;

            }
            CounterModel = tree;
            List<Node> leafNodes2 = new List<Node>();
            GetLeafNodes(CounterModel, ref leafNodes2);
            DistinctNodes = leafNodes2
                .GroupBy(node => new { node.Value, node.TruthValue })
                .Select(group => group.First())
                .Select(node => new Tuple<string, int>(node.Value, node.TruthValue))
                .ToList();
            return true;
        }

        public void GetLeafNodes(Node tree, ref List<Node> leafNodes)
        {
            //if node is null return
            if (tree == null) return;

            if (tree.Left == null && tree.Right == null)
            {
                // Node has no children, so it's a leaf node, we add node to list
                leafNodes.Add(tree);
                // if we are searching for literal to mark him, we will do it here 
            }
            else
            {
                // Node has children, so we recursively traverse its childrens
                if (tree.Left != null)
                {
                    GetLeafNodes(tree.Left, ref leafNodes);
                }
                GetLeafNodes(tree.Right, ref leafNodes);
            }
        }
    }
}
