using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Antlr4.Runtime.Atn.SemanticContext;

namespace VyrokovaLogikaPrace
{
    public class TreeConstructer
    {
        string mHtmlTree;
        Node tree;
        string side = "left";

        public TreeConstructer(string htmlTree)
        {
            mHtmlTree = htmlTree;
        }

        //create tree from html code
        public Node ProcessTree()
        {
            var strippedTags = StripTree();
            CreateTree(strippedTags);
            return tree;
        }

        private void CreateTree(List<string> strippedTags)
        {
            //bool value if next value is item
            bool itIsItem = false;
            //if previous tag was already </ li >
            bool ThereWasLi = false;
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
                    if (tree == null)
                    {
                        tree = TreeBuildHelper.GetNode(tag);
                    }
                    else if (side == "left")
                    {
                            tree.Left = TreeBuildHelper.GetNode(tag);
                            tree.Left.Parent = tree;
                            tree = tree.Left;
                    }
                    else if (side == "right")
                    {
                        tree.Right = TreeBuildHelper.GetNode(tag);
                        tree.Right.Parent = tree;
                        tree = tree.Right;
                    }
                    itIsItem = false;
                    continue;
                }

                //if there is </item> we will just skip this
                else if (tag == "</item>") continue;
                //if <li> and there was already <\li>
                else if (tag == "<li>" && ThereWasLi)
                {
                    tree = tree.Parent;
                    side = "right";
                    ThereWasLi = false;
                }

                else if (tag == "</ul>")
                {
                    tree = tree.Parent;
                }
                else if (tag == "<ul>")
                {
                    side = "left";
                }
                else if (tag == "<item>")
                {
                    itIsItem = true;
                    continue;
                }
            }
        }

        private List<string> StripTree()
        {
            //replace all occurences of span to item
            mHtmlTree = mHtmlTree.Replace("<span class=\"tf-nc\">", "<item>").Replace("</span>", "</item>").Replace("<span class=\"tf-nc\" style=\"border-color: blue;\">", "<item>").Replace("<span class=\"tf-nc\" style=\"\">","<item>");
            //split by this delimeter
            string[] delimiters = { "<li>", "</li>", "<item>", "</item>", "<ul>", "</ul>" };

            // Split the input string by the delimiters
            return SplitWithDelimiters(mHtmlTree, delimiters);
        }

        static List<string> SplitWithDelimiters(string input, string[] delimiters)
        {
            // Replace delimiters with a unique marker
            for (int i = 0; i < delimiters.Length; i++)
            {
                input = input.Replace(delimiters[i], $"|DELIMITER{i}|");
            }

            // Split the input string by the unique marker
            var tempList = input.Split('|').ToList();
            List<string> result = new List<string>()
            {
                "<start>",
            };
            result.AddRange(tempList);
            // Replace the unique marker with the original delimiters
            for (int i = 0; i < result.Count; i++)
            {
                for (int j = 0; j < delimiters.Length; j++)
                {
                    result[i] = result[i].Replace($"DELIMITER{j}", delimiters[j]);
                }
            }
            return result.Where(s => !string.IsNullOrEmpty(s)).ToList();
        }
    }
}
