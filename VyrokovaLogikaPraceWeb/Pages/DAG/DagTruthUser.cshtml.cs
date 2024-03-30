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
    public class DagTruthUserModel : PageModel
    {
        public string ErrorMessage;
        public string Formula;
        public List<string> Errors { get; private set; } = new();

        public bool Valid { get; set; } = true;

        public List<SelectListItem> ListItems { get; set; } = new List<SelectListItem>();
        readonly IWebHostEnvironment mEnv;

        public string YourFormula { get; set; } = "";
        public string Input { get; set; } = "";

        List<string> Steps { get; set; } = new List<string>();

        List<List<VisNode>> visNodes = new List<List<VisNode>>();

        public DagTruthUserModel(IWebHostEnvironment env)
        {
            mEnv = env;
            PrepareList();
        }

        private void PrepareList()
        {
            ListItems = FormulaHelper.InitializeAllFormulas(mEnv);
        }
        public IActionResult OnPostTruthDAG([FromBody] DrawDagUserRequestModel request)
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
            List<Node> tree = new List<Node>();
            if (engine.CreateTree())
            {
                if (tautology)
                {
                    TreeProofAdvanced adv = new TreeProofAdvanced(engine.pSyntaxTree, 0);
                    tree = adv.trees;
                    Steps = adv.steps;
                }
                else
                {
                    TreeProofAdvanced adv = new TreeProofAdvanced(engine.pSyntaxTree, 1);
                    tree = adv.trees;
                    Steps = adv.steps;
                }
            }
            foreach (var treee in tree)
            {
                VisNodesHelper helper = new VisNodesHelper(treee, true);
                visNodes.Add(helper.CreateVisNodes());
            }
            List<int> stepsToRemove = new List<int>();
            int i = 0;
            foreach (var step in Steps)
            {

                if (step.StartsWith("Pøidáme do uzlu"))
                {
                    stepsToRemove.Add(i);
                }
                i++;
            }



            // Remove items from VisNodes and Steps based on indexes stored in indexesToRemove
            foreach (int indexToRemove in stepsToRemove.OrderByDescending(x => x))
            {
                visNodes.RemoveAt(indexToRemove);
                Steps.RemoveAt(indexToRemove);
            }


            var response = new
            {
                VisNodes = visNodes,
                Steps = Steps
            };
            var jsonString = JsonSerializer.Serialize(response);
            return new JsonResult(jsonString);
        }
    }

    public class DrawDagUserRequestModel
    {
        public string Formula { get; set; }
        public bool Tautology { get; set; }
    }

}
