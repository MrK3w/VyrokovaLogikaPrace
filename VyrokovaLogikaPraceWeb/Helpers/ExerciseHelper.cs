using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VyrokovaLogikaPraceWeb.Helpers
{
    public class LogicalFormula
    {
        public string Formula { get; set; }
    }

    public static class ExerciseHelper
    {
        private static List<LogicalFormula> formulas = new List<LogicalFormula>();

        public static List<string> Errors = new List<string>();

        public static List<string> FormulaList { get; set; } = new List<string>();

        // Get formulas from JSON

        public static List<SelectListItem> InitializeAllFormulas(IWebHostEnvironment mEnv)
        {
            GetFormulaList(mEnv);
            return FormulaList
                .Select(formula => new SelectListItem
                {
                    Value = formula,
                    Text = formula
                })
                .ToList();
        }

        public static void GetFormulaList(IWebHostEnvironment env)
        {
            // Get the path of that JSON
            var filePath = Path.Combine(env.ContentRootPath, "Helpers", "formulas.json");

            try
            {
                // Read the content of JSON
                var json = File.ReadAllText(filePath);

                // Convert JSON to our formulas
                formulas = JsonConvert.DeserializeObject<List<LogicalFormula>>(json);

                FormulaList = formulas.ConvertAll(f => f.Formula);
            }
            // If we didn't find that JSON 
            catch (Exception ex)
            {
                return;
            }
        }

        public static void SaveFormulaList(IWebHostEnvironment env, string formula)
        {
            // Get all values from JSON
            GetFormulaList(env);
            //remove all whitespaces
            formula = formula.Replace(" ", "");
            Errors = new List<string>();
            // Check if the formula already exists in the list
            if (formulas != null && formulas.Any(existingFormula => existingFormula.Formula == formula))
            {
                // Formula already exists, inform by errors
                Errors.Add("Formule " + formula + " již existuje!");
                return;
            }

            // Add new formula
            var newFormula = new LogicalFormula { Formula = formula };

            // If the list of formulas is empty, create a new list with that formula
            if (formulas == null)
                formulas = new List<LogicalFormula> { newFormula };
            // Otherwise, just add the formula to the list
            else
                formulas.Add(newFormula);

            // Write list to JSON file
            JsonWriteToFile(env);
            return;
        }

        // Write list to JSON
        private static void JsonWriteToFile(IWebHostEnvironment env)
        {
            // Options for JSON
            var options = new JsonSerializerSettings { Formatting = Formatting.Indented };

            // Serialize list to JSON
            var jsonString = JsonConvert.SerializeObject(formulas, options);

            // Get the path of JSON
            var filePath = Path.Combine(env.ContentRootPath, "Helpers", "formulas.json");

            // Write all text to JSON
            File.WriteAllText(filePath, jsonString);
        }

        // Remove formula from JSON
        internal static void RemoveFromFormulaList(IWebHostEnvironment env, string selectedValue)
        {
            // Get list from JSON
            GetFormulaList(env);

            // Trim all whitespaces in input
            selectedValue = selectedValue.Trim();

            // Find that formula in list and remove
            formulas.RemoveAll(f => f.Formula == selectedValue);

            // Write modified content to JSON
            JsonWriteToFile(env);
        }
    }
}