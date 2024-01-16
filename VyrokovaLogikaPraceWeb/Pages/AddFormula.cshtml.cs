using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VyrokovaLogikaPraceWeb.Helpers;
using VyrokovaLogikaPraceWeb.Pages;

namespace VyrokovaLogikaPraceWeb
{
    public class AddFormulaModel : PageModel
    {
        readonly IWebHostEnvironment mEnv;
        public List<SelectListItem> AllFormulas { get; set; }

        public AddFormulaModel(IWebHostEnvironment env)
        {
            mEnv = env;
            // Move the initialization of AllFormulas to a separate method
            AllFormulas = ExerciseHelper.InitializeAllFormulas(mEnv);
        }

        public IActionResult OnPostAddNewFormula()
        {
            string formula = Request.Form["FormulaInput"];
            ExerciseHelper.SaveFormulaList(mEnv, formula);
            AllFormulas = ExerciseHelper.InitializeAllFormulas(mEnv);
            return Page();
        }

        public IActionResult OnPostRemoveFormula()
        {
            string selectedValue = Request.Form["MyFormulas"];
            ExerciseHelper.RemoveFromFormulaList(mEnv, selectedValue);
            AllFormulas = ExerciseHelper.InitializeAllFormulas(mEnv);
            return Page();
        }
    }
}
