using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VyrokovaLogikaPrace;
using VyrokovaLogikaPraceWeb.Helpers;

namespace VyrokovaLogikaPraceWeb.Pages
{
    public class TruthTreeModel : PageModel
    {
        public bool Valid { get; private set; } = true;

        private readonly List<string> htmlTree = new();

        private string selectFromSelectList;
        private string selectFromInput;
        public string ErrorMessage;
        public string Formula;
        public List<string> Errors { get; private set; } = new();
        public string ConvertedTree { get; set; }
        public  string Message { get; set; }

        public List<Tuple<string,int>> DistinctNodes { get; set; }
        public List<SelectListItem> ListItems { get; set; } = new List<SelectListItem>();
        readonly IWebHostEnvironment mEnv;

        public string YourFormula { get; set; } = "";
        public string Input { get; set; } = "";

        public TruthTreeModel(IWebHostEnvironment env)
        {
            mEnv = env;
            PrepareList();
        }

        private void PrepareList()
        {
            ListItems = FormulaHelper.InitializeAllFormulas(mEnv);
        }

        public IActionResult OnPostDrawTruthTreeTautology()
        {
            //get formula from inputs
            Formula = GetFormula()!;
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
                TreeProof treeProof = new TreeProof();
                if(treeProof.FindContradiction(treeProof.ProcessTree(engine.pSyntaxTree)))
                {
                    Message = "Zvolená formule je tautologií";
 
                }
                else
                {
                    Message = "Zvolená formule není tautologií";
                }
                DistinctNodes = treeProof.DistinctNodes;
                PrintTree(treeProof.CounterModel);
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

        public IActionResult OnPostDrawTruthTreeContradiction()
        {
            //get formula from inputs
            Formula = GetFormula()!;
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
                TreeProof treeProof = new TreeProof();
                if (treeProof.FindContradiction(treeProof.ProcessTree(engine.pSyntaxTree,1)))
                {
                    Message = "Zvolená formule je kontradikcí";

                }
                else
                {
                    Message = "Zvolená formule není kontradikcí";
                }
                DistinctNodes = treeProof.DistinctNodes;
                PrintTree(treeProof.CounterModel);
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
            htmlTree.Add("<li id='node_" + tree.id + "'>");
            string op = string.Empty;
            op = TreeHelper.GetOP(tree);
            //we store tree value and tree op to be able to switch between full form and syntax form
            htmlTree.Add("<span class='tf-nc' onclick='toggleNode(" + tree.id + ", \"" + tree.Value + "\", \"" + op + "\",\"" + tree.TruthValue + "\",)'>" + op + "= " + tree.TruthValue + "</span>");

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

        public string? GetFormula()
        {
            selectFromSelectList = Request.Form["formula"];
            selectFromInput = Request.Form["UserInput"];
            //if user didn't use any of inputs invalidate request and throw errorMessage that user didn't choose formula
            if (selectFromSelectList == "" && selectFromInput == "")
            {
                Valid = false;
                ErrorMessage = "Nevybral jsi žádnou formuli!";
                return null;
            }
            //if user used userInput
            if (selectFromInput != "")
            {
                Converter.ConvertSentence(ref selectFromInput);
                ListItems.Add(new SelectListItem(selectFromInput, selectFromInput));
                var selected = ListItems.Where(x => x.Value == selectFromInput).First();
                selected.Selected = true;
                return selectFromInput;
            }
            //if user used formula from listItem
            else if (selectFromSelectList != "")
            {
                foreach (var item in ListItems)
                {
                    item.Selected = false;
                    if (selectFromSelectList == item.Value)
                    {
                        item.Selected = true;
                    }
                }
                return selectFromSelectList;
            }
            return null;
        }
    }
}
