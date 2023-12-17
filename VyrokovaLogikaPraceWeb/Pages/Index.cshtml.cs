using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;
using VyrokovaLogikaPrace;

namespace VyrokovaLogikaPraceWeb.Pages
{
    public class IndexModel : PageModel
    {
        public enum ButtonType
        {
            None,
            DAG,
            SyntaxTree,
        }

        public ButtonType Button { get; set; }

        public IndexModel()
        {

        }

    }
}