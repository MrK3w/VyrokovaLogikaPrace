using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VyrokovaLogikaPraceWeb.Pages
{
    public class CreateTreeModel : PageModel
    {
        public bool Valid { get; private set; } = true;

        private readonly List<string> htmlTree = new();

        private string vl;
        private string vl1;
        public string ErrorMessage;
        public List<string> Errors { get; private set; } = new();
        public string ConvertedTree { get; set; }

        public enum ButtonType
        {
            None,
            DrawSyntaxTree,
            CreateSyntaxTree,
        }

        public ButtonType Button { get; set; }

        public List<SelectListItem> ListItems { get; set; } = new List<SelectListItem>();


        public string YourFormula { get; set; } = "";
        public string Input { get; set; } = "";


        public CreateTreeModel()
        {
        }

        public IActionResult OnPostCreateTree()
        {
            Button = ButtonType.CreateSyntaxTree;
            return Page();
        }
    }
}
