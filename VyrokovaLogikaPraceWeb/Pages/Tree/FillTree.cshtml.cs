using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VyrokovaLogikaPrace;
using VyrokovaLogikaPraceWeb.Helpers;

namespace VyrokovaLogikaPraceWeb.Pages
{
    public class FillTreeModel : PageModel
    {
        public bool Valid { get; private set; } = true;

        private readonly List<string> htmlTree = new();
        public string ErrorMessage;
        public string Formula;
        public string ConvertedTree { get; set; }

        public List<SelectListItem> ListItems { get; set; } = new List<SelectListItem>();
        readonly IWebHostEnvironment mEnv;

        public string YourFormula { get; set; } = "";
        public string Input { get; set; } = "";
        public List<string> Errors { get; set; } = new List<string>();

        public FillTreeModel(IWebHostEnvironment env)
        {
            mEnv = env;
            PrepareList();
        }

        private void PrepareList()
        {
            ListItems = FormulaHelper.InitializeAllFormulas(mEnv);
        }

        public IActionResult OnPostDrawTree([FromBody] string formula)
        {
            //get formula from inputs
            Formula = formula;
            Converter.ConvertSentence(ref Formula);
            //if it not valid save user input to YourFormula and return page
            if (!Valid)
            {
                if (Formula != null)
                {
                    YourFormula = Formula;
                }
                return Page();
            }
            //otherwise prepare engine with sentence we got
            Engine engine = new Engine(Formula);
            if (engine.CreateTree())
            {
                PrintEmptyTree(engine.pSyntaxTree);
                string div = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
                ConvertedTree = div + string.Join("", htmlTree.ToArray()) + "</div>";
            }
            //prepare tree for css library treeflex
            else
            {
                Errors = engine.Errors;
            }
            var responseData = new
            {
                formula = Formula,
                errors = Errors,
                convertedTree = ConvertedTree
            };
            return new JsonResult(responseData);
        }

        //TODO check if there are two values in one node (contraaiction in node) and check if contradiction is not in leafs
        public IActionResult OnPostCheckTree([FromBody] string text)
        {
            TreeConstructer constructer = new TreeConstructer(text);
            var fillTree = constructer.ProcessTree(true);
            TreeVerifier verifier = new TreeVerifier(fillTree);
            fillTree = verifier.tree;
            Errors = verifier.Errors;
            string msg;
            if (Errors.Count == 0) msg = "Strom je v poøádku, chyby nenalezeny.";
            else msg = "Ve stromu jsou chyby.";
            string div = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            htmlTree.Clear();
            PrintTree(fillTree);
            var responseData = new
            {
                errors = Errors,
                message = msg,
                convertedTree = div + string.Join("", htmlTree.ToArray()) + "</div>",
            };
            return new JsonResult(responseData);
        }

        private void PrintEmptyTree(Node tree)
        {
            htmlTree.Add("<li>");

            string op = TreeHelper.GetOP(tree);

            htmlTree.Add("<span class='tf-nc'>" + op + " = #</span>");


            if (tree.Left != null)
            {
                htmlTree.Add("<ul>");
                PrintEmptyTree(tree.Left);

                if (tree.Right != null)
                {
                    PrintEmptyTree(tree.Right);
                }

                htmlTree.Add("</ul>");
            }

            htmlTree.Add("</li>");
        }

        private void PrintTree(Node tree)
        {
            htmlTree.Add("<li>");
          
            string op = TreeHelper.GetOP(tree);
            if (tree.TruthValue2 == -1)
            {
                if (tree.Red)
                {
                    htmlTree.Add("<span class='tf-nc' style = 'color: red'>" + op + " = " + tree.TruthValue + "</span>");
                }
                else
                {
                    htmlTree.Add("<span class='tf-nc'>" + op + " = " + tree.TruthValue + "</span>");
                }
            }
            else
            {
                if (tree.Red)
                {
                    htmlTree.Add("<span class='tf-nc' style = 'color: red'>" + op + " = " + tree.TruthValue + "/" + tree.TruthValue2 + "</span>");
                }
                else
                {
                    htmlTree.Add("<span class='tf-nc'>" + op + " = " + tree.TruthValue + "/" + tree.TruthValue2 + "</span>");
                }
            }

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
    }
}
