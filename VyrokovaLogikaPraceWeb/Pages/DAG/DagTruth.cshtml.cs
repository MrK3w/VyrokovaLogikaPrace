using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using VyrokovaLogikaPrace;
using VyrokovaLogikaPraceWeb.Helpers;

namespace VyrokovaLogikaPraceWeb.Pages
{
    public class DagTruthModel : PageModel
    {
        public string ErrorMessage;
        public string Formula;
        public List<string> Errors { get; private set; } = new();

        public bool Valid { get; set; } = true;

        public List<SelectListItem> ListItems { get; set; } = new List<SelectListItem>();
        readonly IWebHostEnvironment mEnv;

        public string YourFormula { get; set; } = "";
        public string Input { get; set; } = "";

        List<VisNode> visNodes = new List<VisNode>();

        public DagTruthModel(IWebHostEnvironment env)
        {
            mEnv = env;
            PrepareList();
        }

        private void PrepareList()
        {
            ListItems = FormulaHelper.InitializeAllFormulas(mEnv);
        }
        public IActionResult OnPostTruthDAG([FromBody] DrawDagRequestModel request)
        {
            Formula = request.Formula;
            bool tautology = request.Tautology;
            if (Formula == null) return Page();
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
            Engine engine = new Engine(Formula);
            Node tree = new Node(0);
            if (engine.CreateTree())
            {
                if (tautology)
                {
                    TreeProofAdvanced adv = new TreeProofAdvanced(engine.pSyntaxTree, 0);
                    tree = adv.trees[adv.trees.Count - 1];
                }
                else
                {
                    TreeProofAdvanced adv = new TreeProofAdvanced(engine.pSyntaxTree, 1);
                    tree = adv.trees[adv.trees.Count - 1];
                }
            }
            VisNodesHelper helper = new VisNodesHelper(tree, true);
            visNodes = helper.CreateVisNodes();
            var jsonString = JsonSerializer.Serialize(visNodes);
            return new JsonResult(jsonString);
        }
    }

    public class DrawDagRequestModel
    {
        public string Formula { get; set; }
        public bool Tautology { get; set; }
    }

}
