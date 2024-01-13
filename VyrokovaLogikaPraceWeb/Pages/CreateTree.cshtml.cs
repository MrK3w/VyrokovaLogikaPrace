using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Diagnostics;
using VyrokovaLogikaPrace;

namespace VyrokovaLogikaPraceWeb.Pages
{
    public class CreateTreeModel : PageModel
    {
        public bool Valid { get; private set; } = true;

        public string ErrorMessage;
        public List<string> Errors { get; private set; } = new();
        public List<SelectListItem> ListItems { get; set; } = new List<SelectListItem>();


        public string YourFormula { get; set; } = "";
        public string Input { get; set; } = "";


        public IActionResult OnPostCreateFormula([FromBody] string text)
        {
            TreeConstructer constructer = new TreeConstructer(text);
            constructer.ProcessTree();
            string div = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            
            var responseData = new
            {
                message = "Formula is " + constructer.Formula,
                convertedTree = div + string.Join("", text.ToArray()) + "</div>",
                formula = "Formula is " + constructer.Formula
            };
            return new JsonResult(responseData);
        }

        public IActionResult OnPostCreateTree()
        {
            return Page();
        }
    }
}
