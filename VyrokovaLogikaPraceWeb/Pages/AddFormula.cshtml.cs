using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;
using VyrokovaLogikaPrace;
using VyrokovaLogikaPraceWeb.Helpers;

namespace VyrokovaLogikaPraceWeb
{
    public class AddFormulaModel : PageModel
    {
        readonly IWebHostEnvironment mEnv;
        public List<SelectListItem> AllFormulas { get; set; }

        public List<string> Errors { get; set; } = new List<string>();

        public string Formula { get; set; } = "";

        public List<int> ErrorsIndex { get; set; } = new List<int>();

        public bool FormulaAdded { get; set; } = false;


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
            Regex regex = new Regex(@"[a-zA-Z]+(¬|¬¬)");
            if (regex.IsMatch(formula))
            {
                Errors.Add("Pøed negací nemùže být literál!");
                var matches = regex.Matches(formula);
                foreach (Match match in matches)
                {
                    // Update ErrorsIndex list with the index positions of letters preceding the negation
                    int startIndex = match.Index;
                    int endIndex = match.Index + match.Length - 1;
                    for (int i = startIndex; i <= endIndex; i++)
                    {
                        if (!ErrorsIndex.Contains(i))
                        {
                            ErrorsIndex.Add(i);
                        }
                    }
                }
                Formula = formula;
                return Page();
            }
            Engine engine = new Engine(formula);
            //check if there are some errors in the formula
            if (engine.ParseAndCheckErrors())
            {
                //save formula to JSON
                FormulaHelper.SaveFormulaList(mEnv, formula);
                //get updated list of formula
                AllFormulas = FormulaHelper.InitializeAllFormulas(mEnv);
                Errors = FormulaHelper.Errors;
                if (Errors.Count == 0) FormulaAdded = true;
                Formula = formula;
            }
            else
            {
                Errors = engine.Errors;
                Formula = formula;
                ErrorsIndex = engine.ErrorsIndex;
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
