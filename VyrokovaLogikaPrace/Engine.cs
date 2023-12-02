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
    public class Engine
    {
        string mInput;
        public Node pSyntaxTree { get; set; }
        public Engine(string input)
        {
            mInput = input;
        }

        public void CreateTree()
        {
            Converter.ConvertSentence(ref mInput);
            ICharStream stream = new AntlrInputStream(mInput);
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
                pSyntaxTree = syntaxTree;
            }
            else
            {
                Console.WriteLine("Repair your solution");
            }

        }
    }
}
