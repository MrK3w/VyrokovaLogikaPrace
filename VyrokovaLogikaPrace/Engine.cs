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

        public List<string> Errors { get; private set; } = new List<string>();
        public Engine(string input)
        {
            mInput = input;
        }

        public bool CreateTree()
        {
            Converter.ConvertSentence(ref mInput);
            ICharStream stream = new AntlrInputStream(mInput);
            VyrokovaLogikaLexer lexer = new VyrokovaLogikaLexer(stream);
            CommonTokenStream tokens = new CommonTokenStream(lexer);


            VyrokovaLogikaParser parser = new VyrokovaLogikaParser(tokens);

            var customErrorListener = new VerboseErrorListener();

            parser.RemoveErrorListeners();
            lexer.AddErrorListener(customErrorListener);
            parser.AddErrorListener(customErrorListener);

            IParseTree tree = parser.prog(); // Start with the top-level rule for your grammar

            int errorCount = customErrorListener.ErrorCount;
            Console.WriteLine($"Total errors found: {errorCount}");

            if (errorCount == 0)
            {
                //Console.WriteLine(tree.ToStringTree(parser));
                Traverse(tree, parser.RuleNames, parser.Vocabulary);
                VyrokovaLogikaVisitor visitor = new VyrokovaLogikaVisitor();
                Node syntaxTree = visitor.Visit(tree);
                pSyntaxTree = syntaxTree;
                return true;
            }
            else
            {
                Errors = customErrorListener.Errors;
                Console.WriteLine("Repair your solution");
                return false;
            }

        }

        private void Traverse(IParseTree tree, string[] ruleNames, IVocabulary vocabulary, int indent = 0)
        {
            if (tree.GetText() == "<EOF>")
            {
                return;
            }
            else if (tree is TerminalNodeImpl terminalNode)
            {
                string tokenName = vocabulary.GetSymbolicName(terminalNode.Symbol.Type) ?? terminalNode.GetText();
                Console.WriteLine("{0}{1}='{2}'", new string(' ', indent * 2), tokenName, terminalNode.GetText());
            }
            else if (tree is ParserRuleContext parserRuleContext)
            {
                Console.WriteLine("{0}{1}", new string(' ', indent * 2), ruleNames[parserRuleContext.RuleIndex]);
                foreach (var child in parserRuleContext.children)
                {
                    Traverse(child, ruleNames, vocabulary, indent + 1);
                }
            }
        }
    }
}

