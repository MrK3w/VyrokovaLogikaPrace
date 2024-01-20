using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using VyrokovaLogikaPrace;
using VyrokovaLogikaPraceWeb.Helpers;

namespace VyrokovaLogikaPraceWeb.Pages
{
    public class CreateTreeModel : PageModel
    {
        public bool Valid { get; private set; } = true;
        public List<string> Errors { get; private set; } = new();
        public List<SelectListItem> ListItems { get; set; } = new List<SelectListItem>();

        IWebHostEnvironment mEnv;
        public string YourFormula { get; set; } = "";
        public string Input { get; set; } = "";

        public string Message { get; set; } = "";

        public CreateTreeModel(IWebHostEnvironment env)
        {
            mEnv = env;
        }

        public IActionResult OnPostCreateFormula([FromBody] string text)
        {
            TreeConstructer constructer = new TreeConstructer(text);
            constructer.ProcessTree();
            string div = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            
            var responseData = new
            {
                message = "Formula is " + constructer.Formula,
                convertedTree = div + string.Join("", text.ToArray()) + "</div>",
                formula = constructer.Formula
            };
            return new JsonResult(responseData);
        }

        public IActionResult OnPostSaveFormula([FromBody] string formula)
        {
            Errors = new List<string>();
            Engine engine = new Engine(formula);
            //check if there are some errors in the formula
            if (engine.ParseAndCheckErrors())
            {
                //save formula to JSON
                ExerciseHelper.SaveFormulaList(mEnv, formula);
                //get updated list of formula;
                Errors = ExerciseHelper.Errors;
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
