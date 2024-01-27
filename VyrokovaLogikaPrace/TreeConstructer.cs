﻿using System.Collections.Generic;
using System.Linq;
using VyrokovaLogikaPrace.Enums;

namespace VyrokovaLogikaPrace
{

    
    //class for building tree from page
    public class TreeConstructer
    {
        //tree in html string
        string mHtmlTree;
        //new istance of tree
        Node tree;
        //set default side to left
        Side side = Side.left;
        //counter for id of nodes
        private int globalIdCounter = 1;
        public string Formula => tree.Value;

        private int GetNextId()
        {
            return globalIdCounter++;
        }

        public TreeConstructer(string htmlTree)
        {
            mHtmlTree = htmlTree;
        }

        //create tree from html code
        public Node ProcessTree()
        {
            //get list of tags in tree
            var strippedTags = StripTree();
            //create tree from this list of tags
            CreateTree(strippedTags);
            //get full formula logic
            FillFormula(tree);
            return tree;
        }        

        //modifed version of traverse in tree to get full formula
        private void FillFormula(Node tree)
        {
            if(tree.Left !=  null)
                FillFormula(tree.Left);
            if (tree.Right != null)
                FillFormula(tree.Right);
            if(tree.Left != null && tree.Right != null)
            {
                tree.Value = '(' +  tree.Left.Value + TreeBuildHelper.GetOP(tree) + tree.Right.Value + ')';
            }
            else if(tree.Left != null)
            {
                if(tree.Left is ValueNode)
                tree.Value = TreeBuildHelper.GetOP(tree) + tree.Left.Value;
                else
                {
                    tree.Value = TreeBuildHelper.GetOP(tree) + '(' + tree.Left.Value + ')';
                }
            }
            else
            {
                
            }
            return;
        }

        private void CreateTree(List<string> strippedTags)
        {
            //if next item in list is item(value of node) set to true
            bool itIsItem = false;
            //if we have inside <ul> already <li> set to true, to let it now that it will be right side
            bool ThereWasLi = false;
            //iteration of each tag in list
            foreach (string tag in strippedTags)
            {
                if (tag == "</li>")
                {
                    ThereWasLi = true;
                    continue;
                }
                //if it item we need to get this values
                if (itIsItem)
                {
                    //if we din't have tree, we will create new one
                    if (tree == null)
                    {
                        tree = TreeBuildHelper.GetNode(tag, GetNextId());
                    }
                    //if it will be left side getNode from tag value
                    else if (side == Side.left)
                    {
                            //we will create tree left node 
                            tree.Left = TreeBuildHelper.GetNode(tag, GetNextId());
                            //set his parent to current tree
                            tree.Left.Parent = tree;
                            //and move to left node
                            tree = tree.Left;
                    }
                    //if it will be right side similar as above
                    else if (side == Side.right)
                    {
                        tree.Right = TreeBuildHelper.GetNode(tag, GetNextId());
                        tree.Right.Parent = tree;
                        tree = tree.Right;
                    }
                    itIsItem = false;
                    continue;
                }

                //if there is </item> we will just skip this(information that items ends here)
                else if (tag == "</item>") continue;
                //if <li> and it second time inside <ul>
                else if (tag == "<li>" && ThereWasLi)
                {
                    tree = tree.Parent;
                    side = Side.right;
                    ThereWasLi = false;
                }
                //finish on child nodes, we need to return to parent
                else if (tag == "</ul>")
                {
                    tree = tree.Parent;
                }
                //this let us know that there will be some child nodes, so we will add to left side of tree
                else if (tag == "<ul>")
                {
                    side = Side.left;
                }
                //let know that there will be a item
                else if (tag == "<item>")
                {
                    itIsItem = true;
                    continue;
                }
            }
        }

        private List<string> StripTree()
        {
            //replace all occurences of spans with item (easier to iterate it in CreateTree method 
            mHtmlTree = mHtmlTree.Replace("<span class=\"tf-nc\">", "<item>").Replace("</span>", "</item>").Replace("<span class=\"tf-nc\" style=\"border-color: blue;\">", "<item>").Replace("<span class=\"tf-nc\" style=\"\">","<item>");
            //split by this delimeter
            //we will strip tree with this delimetrs to create list
            string[] delimiters = { "<li>", "</li>", "<item>", "</item>", "<ul>", "</ul>" };

            // Split the input string by the delimiters
            return SplitWithDelimiters(mHtmlTree, delimiters);
        }

        static List<string> SplitWithDelimiters(string input, string[] delimiters)
        {
            // Replace delimiters with a unique marker
            for (int i = 0; i < delimiters.Length; i++)
            {
                //we will add to each tag | to be able to strip into separate tags
                input = delimiters[i] != "</item>"
                    ? input.Replace(delimiters[i], delimiters[i] + "|")
                    : input.Replace(delimiters[i], "|"+delimiters[i] + "|");
            }

            // Split the input string by the unique marker
            return input.Split('|').ToList();
            
        }
    }
}
