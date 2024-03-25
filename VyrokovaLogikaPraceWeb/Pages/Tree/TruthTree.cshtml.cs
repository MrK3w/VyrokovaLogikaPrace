using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VyrokovaLogikaPrace;
using VyrokovaLogikaPraceWeb.Helpers;

namespace VyrokovaLogikaPraceWeb.Pages
{
    public class TruthTreeModel : PageModel
    {
        //If we have tree then it is valid
        public bool Valid { get; private set; } = true;
        //tree in html string
        private  List<string> htmlTree = new();
        //formula selected from select list
        private string selectFromSelectList;
        //string for showing error message, why tree did not compile
        public string ErrorMessage;
        //Formula
        public string Formula;
        public List<string> Errors { get; private set; } = new();
        public string ConvertedTree { get; set; }

        public List<string> ConvertedTrees { get; set; } = new List<string>();
        public  string Message { get; set; }
        public List<string> Steps { get; set; } = new List<string>();
        public List<Tuple<string,int>> DistinctNodes { get; set; }
        public List<SelectListItem> ListItems { get; set; } = new List<SelectListItem>();
        readonly IWebHostEnvironment mEnv;
        public bool Advanced { get; set; } = false;
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

        public IActionResult OnPostDrawTruthTreeTautologyAdvanced()
        {
            Advanced = true;
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
                TreeProofAdvanced adv = new TreeProofAdvanced(engine.pSyntaxTree,0);
                //prepare tree for css library treeflex
                if (adv.IsTautology)
                {
                    Message = "Zvolená formule je tautologií";

                }
                else
                {
                    Message = "Zvolená formule není tautologií";
                }
                DistinctNodes = adv.DistinctNodes;
                Steps = adv.steps;
                foreach (var trx in adv.trees)
                {
                    htmlTree = new();
                    PrintTree(trx);
                    string div =  "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
                    ConvertedTree = div + string.Join("", htmlTree.ToArray()) + "</div>";
                    ConvertedTrees.Add(ConvertedTree);
                }
            }

            else
            {
                Errors = engine.Errors;
                Valid = false;
            }
            return Page();
        }


        public IActionResult OnPostDrawTruthTreeContradictionAdvanced()
        {
            Advanced = true;
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
                TreeProofAdvanced adv = new TreeProofAdvanced(engine.pSyntaxTree,1);
                //prepare tree for css library treeflex
                if (adv.IsTautology)
                {
                    Message = "Zvolená formule je kontradikcí";
                }
                else
                {
                    Message = "Zvolená formule není kontradikcí";
                }
                DistinctNodes = adv.DistinctNodes;
                Steps = adv.steps;
                foreach (var trx in adv.trees)
                {
                    htmlTree = new();
                    PrintTree(trx);
                    string div = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
                    ConvertedTree = div + string.Join("", htmlTree.ToArray()) + "</div>";
                    ConvertedTrees.Add(ConvertedTree);
                }
            }
            else
            {
                Errors = engine.Errors;
                Valid = false;
            }
            return Page();
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
                var trees = treeProof.ProcessTree(engine.pSyntaxTree, 0);
                ContradictionHelper contradictionHelper = new ContradictionHelper();
                if (contradictionHelper.FindContradictionInLeafs(trees))
                {
                    Message = "Zvolená formule je tautologií";

                }
                else
                {
                    Message = "Zvolená formule není tautologií";
                }
                DistinctNodes = contradictionHelper.DistinctNodes;
                PrintTree(contradictionHelper.CounterModel);
                string div = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
                ConvertedTree = div + string.Join("", htmlTree.ToArray()) + "</div>";
                ConvertedTrees.Add(ConvertedTree);
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
                var trees = treeProof.ProcessTree(engine.pSyntaxTree, 1);
                ContradictionHelper contradictionHelper = new ContradictionHelper();
                if(contradictionHelper.FindContradictionInLeafs(trees))
                {
                    Message = "Zvolená formule je kontradikcí";

                }
                else
                {
                    Message = "Zvolená formule není kontradikcí";
                }
                DistinctNodes = contradictionHelper.DistinctNodes;
                PrintTree(contradictionHelper.CounterModel);
                string div = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
                ConvertedTree = div + string.Join("", htmlTree.ToArray()) + "</div>";
                ConvertedTrees.Add(ConvertedTree);
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
            string op = TreeHelper.GetOP(tree);
            //we store tree value and tree op to be able to switch between full form and syntax form

            if (tree.Red)
            {
                if (tree.TruthValue == -1)
                {
                    if (tree.TruthValue2 == -1)
                    {
                        htmlTree.Add("<span class='tf-nc' style='color: red'>" + op + "</span>");
                    }
                    else
                    {
                        htmlTree.Add("<span class='tf-nc' style='color: red'>" + op + "/" + "</span>");
                    }
                }
                else
                {
                    if (tree.TruthValue2 == -1)
                    {
                        htmlTree.Add("<span class='tf-nc' style='color: red'>" + op + "= " + tree.TruthValue + "</span>");
                    }
                    else
                    {
                        htmlTree.Add("<span class='tf-nc' style='color: red'>" + op + "= " + tree.TruthValue + "/" + tree.TruthValue2 + "</span>");
                    }
                }
            }

            else if (tree.Blue)
            {
                if (tree.TruthValue == -1)
                {
                    if (tree.TruthValue2 == -1)
                    {
                        htmlTree.Add("<span class='tf-nc' style='color: blue'>" + op + "</span>");
                    }
                    else
                    {
                        htmlTree.Add("<span class='tf-nc' style='color: blue'>" + op + "/" + "</span>");
                    }
                }
                else
                {
                    if (tree.TruthValue2 == -1)
                    {
                        htmlTree.Add("<span class='tf-nc' style='color: blue'>" + op + "= " + tree.TruthValue + "</span>");
                    }
                    else
                    {
                        htmlTree.Add("<span class='tf-nc' style='color: blue'>" + op + "= " + tree.TruthValue + "/" + tree.TruthValue2 + "</span>");
                    }
                }
            }

           
           
            else
            {
                if (tree.TruthValue == -1)
                {
                    if (tree.TruthValue2 == -1)
                    {
                        htmlTree.Add("<span class='tf-nc' style='color: black'>" + op + "</span>");
                    }
                    else
                    {
                        htmlTree.Add("<span class='tf-nc' style='color: black'>" + op + "/" + "</span>");
                    }
                }
                else
                {
                    if (tree.TruthValue2 == -1)
                    {
                        htmlTree.Add("<span class='tf-nc' style='color: black'>" + op + "= " + tree.TruthValue + "</span>");
                    }
                    else
                    {
                        htmlTree.Add("<span class='tf-nc' style='color: black'>" + op + "= " + tree.TruthValue + "/" + tree.TruthValue2 + "</span>");
                    }
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

        public string? GetFormula()
        {
            selectFromSelectList = Request.Form["formula"];
            //if user didn't use any of inputs invalidate request and throw errorMessage that user didn't choose formula
            if (selectFromSelectList == "")
            {
                Valid = false;
                ErrorMessage = "Nevybral jsi žádnou formuli!";
                return null;
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
