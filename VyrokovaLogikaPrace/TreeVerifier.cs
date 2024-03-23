using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogikaPrace
{
    public class TreeVerifier
    {
        public Node tree { get; set; }
        public List<string> Errors = new List<string>();
        public Dictionary<string,int> literalsTruthValues = new Dictionary<string,int>();
        public List<string> wronglyAssignedLiterals= new List<string>();

        public TreeVerifier(Node tree)
        {
            this.tree = tree;
            VerifyCombinations(tree);
            VerifyLiteralsValues(tree);
            if(wronglyAssignedLiterals.Count > 0)
            {
                MarkWrongLiterals(tree);
            }
        }

        private void MarkWrongLiterals(Node tree)
        {
            if (tree.IsLeaf == true)
            {
               foreach(var wronglyAssignedLiteral in wronglyAssignedLiterals)
               {
                    if (wronglyAssignedLiteral == tree.Value) tree.Red = true;
               }
            }
            if (tree.Left != null) MarkWrongLiterals(tree.Left);
            if (tree.Right != null) MarkWrongLiterals(tree.Right);
        }
        private void VerifyLiteralsValues(Node tree)
        {
            if (tree.IsLeaf == true)
            {
                if (!literalsTruthValues.ContainsKey(tree.Value))
                {
                    literalsTruthValues[tree.Value] = tree.TruthValue;
                }
                else
                {
                    if (literalsTruthValues[tree.Value] != tree.TruthValue)
                    {
                        if (!tree.Contradiction)
                        {
                            Errors.Add("Literál " + tree.Value + " není označen jako spor, a přesto se neshodují pravdivostní hodnoty literálu");
                            if (!wronglyAssignedLiterals.Contains(tree.Value)) wronglyAssignedLiterals.Add(tree.Value);
                        }
                    }
                }
            }
            if (tree.Left != null) VerifyLiteralsValues(tree.Left);
            if (tree.Right != null) VerifyLiteralsValues(tree.Right);
        }

        private void VerifyCombinations(Node tree)
        {
            if (tree.IsLeaf == true) return;
            if(tree.Left != null && tree.Right != null)
            {
               if(!TreeHelper.GoodCombination(tree,tree.Left.TruthValue, tree.Right.TruthValue))
               {
                    tree.Red = true;
                    Errors.Add("Pokud máme operátor " + TreeHelper.GetOperatorName(tree) + " ,který má pravdivostní hodnotu " + tree.TruthValue + " tak potomci nemohou mít uzly nemohou mít hodnotu " + tree.Left.TruthValue + " a " + tree.Right.TruthValue + ".");
               }
            }
            else if (tree.Left != null)
            {
                if(!TreeHelper.GoodCombination(tree,tree.Left.TruthValue))
                {
                    tree.Red = true;
                    Errors.Add("Pokud máme operátor " + TreeHelper.GetOperatorName(tree) + " ,který má pravdivostní hodnotu " + tree.TruthValue + " tak potomek nemůže mít hodnotu " + tree.Left.TruthValue);
                }
            }
            if (tree.Left != null) VerifyCombinations(tree.Left);
            if (tree.Right != null) VerifyCombinations(tree.Right);
        }

    }
}
