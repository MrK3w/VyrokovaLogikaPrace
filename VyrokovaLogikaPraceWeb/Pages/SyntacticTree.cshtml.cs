using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VyrokovaLogikaPrace;

namespace VyrokovaLogikaPraceWeb.Pages
{
    public class SyntacticTreeModel : PageModel
    {
        public bool Valid { get; private set; } = true;

        private readonly List<string> htmlTree = new();

        private string vl;
        private string vl1;
        public string ErrorMessage;
        public List<string> Errors { get; private set; } = new();
        public string ConvertedTree { get; set; }

        public enum ButtonType
        {
            None,
            DAG,
            SyntaxTree,
        }

        public ButtonType Button { get; set; }

        public List<SelectListItem> ListItems { get; set; } = new List<SelectListItem>();


        public string YourFormula { get; set; } = "";

        public SyntacticTreeModel()
        {
            PrepareList();
        }

        private void PrepareList()
        {
            ListItems = ListItemsHelper.ListItems;
        }

        public IActionResult OnPostCreateTree()
        {
            Button = ButtonType.SyntaxTree;
            //get formula from inputs
            string mSentence = GetFormula();
            //if it not valid save user input to YourFormula and return page
            if (!Valid)
            {
                if (mSentence != null)
                {
                    YourFormula = mSentence;
                }
                return Page();
            }
            //otherwise prepare engine with sentence we got
            Engine engine = new Engine(mSentence);
            if(engine.CreateTree())
            {
                PrintTree(engine.pSyntaxTree);
                string div = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
                ConvertedTree = div + string.Join("", htmlTree.ToArray()) + "</div>";
            }
            //prepare tree for css library treeflex
            else
            {
                Errors = engine.Errors;
                Valid = false;
            }
            return Page();
        }

        private void PrintTree(Node tree)
        {
            htmlTree.Add("<li>");
            string op = string.Empty;
            op = GetOP(tree);
            htmlTree.Add("<span class=tf-nc>" + op + "</span>");
            //if tree has childNodeLeft we will use recursion 
            if (tree.Left != null)
            {
                htmlTree.Add("<ul>");
                PrintTree(tree.Left);
                if (tree.Right != null)
                {
                    PrintTree(tree.Right);
                }
                htmlTree.Add("</ul>");
            }
            htmlTree.Add("</li>");
        }

        private string GetOP(Node tree)
        {
            switch (tree)
            {
                case NegationOperatorNode:
                    return "¬";
                case DoubleNegationOperatorNode:
                    return "¬¬";
                case ConjunctionOperatorNode:
                    return "∧";
                case DisjunctionOperatorNode:
                    return "∨";
                case EqualityOperatorNode:
                    return "≡";
                case ImplicationOperatorNode:
                    return "⇒";
                case ValueNode:
                    return tree.Value;
            }
            return String.Empty;
        }

        public string? GetFormula()
        {
            vl = Request.Form["formula"];
            vl1 = Request.Form["UserInput"];
            //if user didn't use any of inputs invalidate request and throw errorMessage that user didn't choose formula
            if (vl == "" && vl1 == "")
            {
                Valid = false;
                ErrorMessage = "Nevybral jsi žádnou formuli!";
                return null;
            }
            //if user user userInput
            if (vl1 != "")
            {
                ////validate his formula otherwise throw error message and save that formula so user can change it later
                //if (!Validator.ValidateSentence(ref vl1))
                //{
                //    ErrorMessage = Validator.ErrorMessage;
                //    Valid = false;
                //    YourFormula = vl1;
                //    return null;
                //}
                //convert logical operators in case they are not in right format
                Converter.ConvertSentence(ref vl1);
                //add formula to listItem
                ListItemsHelper.SetListItems(vl1);
                //select that formula in itemList
                var selected = ListItems.Where(x => x.Value == vl1).First();
                selected.Selected = true;
                return vl1;
            }
            //if user used formula from listItem
            else if (vl != "")
            {
                //validate this sentece
                //if (!Validator.ValidateSentence(ref vl))
                //{
                //    Valid = false;
                //    ErrorMessage = Validator.ErrorMessage;
                //    return null;
                //}
                //select that formula in listItem
                foreach (var item in ListItems)
                {
                    item.Selected = false;
                    if (vl == item.Value)
                    {
                        item.Selected = true;
                    }
                }
                return vl;
            }
            return null;
        }
    }
}