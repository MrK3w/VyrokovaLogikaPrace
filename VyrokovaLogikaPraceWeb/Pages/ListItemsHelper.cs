using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using VyrokovaLogikaPrace;

namespace VyrokovaLogikaPraceWeb.Pages
{
    public static class ListItemsHelper
    {
        //list item used on page
        private static List<SelectListItem> _listItems;
        //if it is empty create new one and set this to default values otherwise return
        public static List<SelectListItem> ListItems
        {
            get
            {
                if (_listItems == null)
                {
                    _listItems = new List<SelectListItem>();
                    SetListItems();
                }
                return _listItems;
            }
        }

        public static string? ErrorMesage;

        //create new list of formulas for exercises
        public static void SetListItems(string mSentence = null)
        {
            if (_listItems.Count == 0)
            {
                SelectListItem defaultItem = new() { Text = "Vyber formuli", Value = "" };
                string mPropositionalSentence = "(p>q)≡(-q>-p)";
                Converter.ConvertSentence(ref mPropositionalSentence);

                string mPropositionalSentence1 = "((-x|b)&(x|a))";
                Converter.ConvertSentence(ref mPropositionalSentence1);

                string mPropositionalSentence2 = "(-x|b)&(x|a)";
                Converter.ConvertSentence(ref mPropositionalSentence);

                string mPropositionalSentence3 = "(P&-P)";
                Converter.ConvertSentence(ref mPropositionalSentence);
                string mPropositionalSentence4 = "(A|B)&(-A&-B)";
                Converter.ConvertSentence(ref mPropositionalSentence);
                SelectListItem item1 = new(mPropositionalSentence, mPropositionalSentence);
                SelectListItem item2 = new(mPropositionalSentence1, mPropositionalSentence1);
                SelectListItem item3 = new(mPropositionalSentence2, mPropositionalSentence2);
                SelectListItem item4 = new(mPropositionalSentence3, mPropositionalSentence3);
                SelectListItem item5 = new(mPropositionalSentence4, mPropositionalSentence4);
                _listItems.Add(defaultItem);
                _listItems.Add(item1);
                _listItems.Add(item2);
                _listItems.Add(item3);
                _listItems.Add(item4);
                _listItems.Add(item5);
            }
            else
            {
                // Check if the item already exists in the list
                if (!_listItems.Any(item => item.Text == mSentence))
                {
                    SelectListItem item = new(mSentence, mSentence);
                    _listItems.Add(item);
                }
            }
        }
    }
}