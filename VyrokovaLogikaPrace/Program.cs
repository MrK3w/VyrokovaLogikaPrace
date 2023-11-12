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
        static void Main(string[] args)
        {
            string input = "-(-A&B|C|(C=A))";
            Converter.ConvertSentence(ref input);
            ICharStream stream = new AntlrInputStream(input);
            VyrokovaLogikaLexer lexer = new VyrokovaLogikaLexer(stream);
            CommonTokenStream tokens = new CommonTokenStream(lexer);


            VyrokovaLogikaParser parser = new VyrokovaLogikaParser(tokens);

            var customErrorListener = new VerboseErrorListener();

            parser.RemoveErrorListeners();
            parser.AddErrorListener(customErrorListener);

            IParseTree tree = parser.prog(); // Start with the top-level rule for your grammar

            int errorCount = customErrorListener.GetErrorCount();
            Console.WriteLine($"Total errors found: {errorCount}");

            if (errorCount == 0)
            {
                Console.WriteLine(tree.ToStringTree(parser));

                VyrokovaLogikaVisitor visitor = new VyrokovaLogikaVisitor();
                Node syntaxTree = visitor.Visit(tree);
            }
            else
            {
                Console.WriteLine("Repair your solution");
            }
        }
    }
}
