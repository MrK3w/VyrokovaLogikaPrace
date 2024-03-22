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
        public TreeVerifier(Node tree)
        {
            this.tree = tree;
            Verify(tree);
        }

        public void Verify(Node tree)
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
            if (tree.Left != null) Verify(tree.Left);
            if (tree.Right != null) Verify(tree.Right);
        }

    }
}
