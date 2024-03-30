using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
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

        List<VisNode> visNodes = new List<VisNode>();

        public DagTruthUserModel(IWebHostEnvironment env)
        {
            mEnv = env;
            PrepareList();
        }

        private void PrepareList()
        {
            ListItems = FormulaHelper.InitializeAllFormulas(mEnv);
        }
        public IActionResult OnPostTruthDAG([FromBody] string request)
        {
            Formula = request;
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
            //otherwise prepare engine with sentence we got
            Engine engine = new Engine(Formula);

            visNodes = new List<VisNode>();
            if (engine.CreateTree())
            {
                VisNodesHelper helper = new VisNodesHelper(engine.pSyntaxTree);
                visNodes = helper.CreateVisNodes();
            }
            var typeOfFormula = "";
            TreeProofAdvanced adv = new TreeProofAdvanced(engine.pSyntaxTree, 0);
            //prepare tree for css library treeflex
            if (adv.IsTautology)
            {
                typeOfFormula = "tautology";
            }
            else
            {
                adv = new TreeProofAdvanced(engine.pSyntaxTree, 1);
                if (adv.IsTautology)
                {
                    typeOfFormula = "contradiction";
                }
                else
                {
                    typeOfFormula = "satisfiable";
                }
            }

             var responseData = new
             {
                typeOfFormula = typeOfFormula,
                visNodes = visNodes,
             };

            return new JsonResult(responseData);
        }
    }

    public class DrawDagUserRequestModel
    {
      
    }

}
