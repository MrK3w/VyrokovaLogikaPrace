using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace VyrokovaLogikaPrace
{
    class Program
    {
        //console main for testing reasons, use web app
        static void Main(string[] args)
        {
            string input = "-A>B&B|--(A&B)";
            Converter.ConvertSentence(ref input);
            Engine engine = new Engine(input);
            var tree = engine.pSyntaxTree;
            if (engine.CreateTree())
            {
                Console.WriteLine("strom správně vytvořen");
            }          
        }
    }
}
