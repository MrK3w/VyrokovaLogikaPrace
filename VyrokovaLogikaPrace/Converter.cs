﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogikaPrace
{
    public static class Converter
    {
        //Convert formula input to usable characters
        public static void ConvertSentence(ref string input)
        {
            input = input.Replace("&", "∧")
                 .Replace("|", "∨")
                 .Replace("=", "≡")
                 .Replace("-", "¬")
                 .Replace(">", "⇒")
                 .Replace("--", "¬¬")
                 .Replace("→", "⇒");
        }
    }
}
