using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VyrokovaLogikaPrace;
using VyrokovaLogikaPraceWeb.Helpers;

namespace VyrokovaLogikaPraceWeb
{
    public class AddFormulaModel : PageModel
    {
        readonly IWebHostEnvironment mEnv;
        public List<SelectListItem> AllFormulas { get; set; }

        public List<string> Errors { get; set; } = new List<string>();

        public string Formula = "";

        public AddFormulaModel(IWebHostEnvironment env)
        {
            mEnv = env;
            // Move the initialization of AllFormulas to a separate method
            AllFormulas = FormulaHelper.InitializeAllFormulas(mEnv);
        }

        public IActionResult OnPostAddNewFormula()
        {
            //get value from FormulaInput
            string formula = Request.Form["FormulaInput"];
            Engine engine = new Engine(formula);
            //check if there are some errors in the formula
            if (engine.ParseAndCheckErrors())
            {
                //save formula to JSON
                FormulaHelper.SaveFormulaList(mEnv, formula);
                //get updated list of formula
                AllFormulas = FormulaHelper.InitializeAllFormulas(mEnv);
                Errors = FormulaHelper.Errors;
            }
            else
            {
                Errors = engine.Errors;
                Formula = formula;
            }
            return Page();
        }

        public IActionResult OnPostRemoveFormula()
        {
            string selectedValue = Request.Form["MyFormulas"];
            FormulaHelper.RemoveFromFormulaList(mEnv, selectedValue);
            AllFormulas = FormulaHelper.InitializeAllFormulas(mEnv);
            return Page();
        }
    }
}
