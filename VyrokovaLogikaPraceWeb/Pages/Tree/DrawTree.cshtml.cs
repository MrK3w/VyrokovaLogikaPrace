﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VyrokovaLogikaPrace;
using VyrokovaLogikaPraceWeb.Helpers;

namespace VyrokovaLogikaPraceWeb.Pages
{
    public class DrawTreeModel : PageModel
    {
        public bool Valid { get; private set; } = true;

        private readonly List<string> htmlTree = new();

        private string selectFromSelectList;

        public string ErrorMessage;
        public string Formula;
        public List<string> Errors { get; private set; } = new();
        public string ConvertedTree { get; set; }

        public List<SelectListItem> ListItems { get; set; } = new List<SelectListItem>();
        readonly IWebHostEnvironment mEnv;

        public string YourFormula { get; set; } = "";
        public string Input { get; set; } = "";

        public DrawTreeModel(IWebHostEnvironment env)
        {
            mEnv = env;
            PrepareList();
        }

        private void PrepareList()
        {
            ListItems = FormulaHelper.InitializeAllFormulas(mEnv);
        }

        public IActionResult OnPostDrawTree()
        {
            //get formula from inputs
            Formula = GetFormula()!;
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
            if (engine.CreateTree())
            {
                PrintTree(engine.pSyntaxTree);
                string div = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
                ConvertedTree = div + string.Join("", htmlTree.ToArray()) + "</div>";
            }
            //prepare tree for css library treeflex
            else
            {
                Errors = engine.Errors;
                Valid = false;
            }
            return Page();
        }

        public IActionResult OnPostCreateTree()
        {
            return Page();
        }

        private void PrintTree(Node tree)
        {
            htmlTree.Add("<li id='node_" + tree.id + "'>");
            string op = string.Empty;

            op = TreeHelper.GetOP(tree);
            int position;
            if(tree is NegationOperatorNode || tree is DoubleNegationOperatorNode || tree is ValueNode)
              position = tree.Value.IndexOf(op);
            else
            {

                if (tree.Value[0] == '(')
                {
                    position = tree.Left.Value.Length + 2;
                    var firstPart = tree.Value.Substring(0, position);
                    var secondPart = tree.Value.Substring(position+1);
                    var temp = firstPart + op + secondPart;
                    if (temp != tree.Value) position = position-2;
                }

                else position = tree.Left.Value.Length;
            }
            //we store tree value and tree op to be able to switch between full form and syntax form
            htmlTree.Add("<span class='tf-nc' onclick='toggleNode(" + tree.id + ", \"" + tree.Value + "\", \"" + op + "\", " + position + ")'>" + op + "</span>");


            if (tree.Left != null)
            {
                htmlTree.Add("<ul>");
                PrintTree(tree.Left);

                if (tree.Right != null)
                {
                    PrintTree(tree.Right);
                }

                htmlTree.Add("</ul>");
            }

            htmlTree.Add("</li>");
        }

        

        public string? GetFormula()
        {
            selectFromSelectList = Request.Form["formula"];
            //if user didn't use any of inputs invalidate request and throw errorMessage that user didn't choose formula
            if (selectFromSelectList == "")
            {
                Valid = false;
                ErrorMessage = "Nevybral jsi žádnou formuli!";
                return null;
            }
            //if user used formula from listItem
            else if (selectFromSelectList != "")
            {
                foreach (var item in ListItems)
                {
                    item.Selected = false;
                    if (selectFromSelectList == item.Value)
                    {
                        item.Selected = true;
                    }
                }
                return selectFromSelectList;
            }
            return null;
        }
    }
}
