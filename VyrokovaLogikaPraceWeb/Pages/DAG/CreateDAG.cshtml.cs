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
    public class CreateDAGModel : PageModel
    {
        public string ErrorMessage;
        public string Formula;
        public List<string> Errors { get; private set; } = new();

        public bool Valid { get; set; } = true;

        public List<SelectListItem> ListItems { get; set; } = new List<SelectListItem>();
        readonly IWebHostEnvironment mEnv;

        public string YourFormula { get; set; } = "";
        public string Input { get; set; } = "";

        public CreateDAGModel(IWebHostEnvironment env)
        {
            mEnv = env;
        }

        public IActionResult OnPostSaveFormula([FromBody] string formula)
        {
            Errors = new List<string>();
            Engine engine = new Engine(formula);
            //check if there are some errors in the formula
            if (engine.ParseAndCheckErrors())
            {
                //save formula to JSON
                FormulaHelper.SaveFormulaList(mEnv, formula);
                //get updated list of formula;
                Errors = FormulaHelper.Errors;
            }
            else
            {
                Errors = engine.Errors;
            }

            var responseData = new
            {
                errors = Errors,
            };
            return new JsonResult(responseData);
        }
    }
}
