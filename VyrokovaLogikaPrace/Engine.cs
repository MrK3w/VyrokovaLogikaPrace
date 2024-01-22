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

        VyrokovaLogikaParser parser;

        IParseTree tree;
        VerboseErrorListener customErrorListener;
        public bool ParseAndCheckErrors()
        {
            // Create a character stream from the input string
            ICharStream stream = new AntlrInputStream(mInput);

            // Create a lexer that processes the character stream
            VyrokovaLogikaLexer lexer = new VyrokovaLogikaLexer(stream);

            // Create a token stream from the lexer
            CommonTokenStream tokens = new CommonTokenStream(lexer);

            // Create a parser that processes the token stream
            parser = new VyrokovaLogikaParser(tokens);

            // Create a custom error listener for more detailed error handling
            customErrorListener = new VerboseErrorListener();

            // Remove the default error listeners from the parser and lexer
            parser.RemoveErrorListeners();
            lexer.RemoveErrorListeners();
            // Add custom error listeners
            lexer.AddErrorListener(customErrorListener);
            parser.AddErrorListener(customErrorListener);

            // Start parsing by invoking the top-level rule of the grammar ('prog' in this case)
            tree = parser.prog();

            // Retrieve the errors from the custom error listener
            Errors = customErrorListener.Errors;

            // Get the total number of errors encountered during parsing
            int errorCount = customErrorListener.ErrorCount;

            // Output the total number of errors to the console
            Console.WriteLine($"Total errors found: {errorCount}");

            // If there are no errors, return true; otherwise, return false
            if (errorCount == 0)
                return true;
            else
                return false;
        }

        public bool CreateTree()
        {
           
            if (ParseAndCheckErrors())
            {
                Traverse(tree, parser.RuleNames, parser.Vocabulary);
                VyrokovaLogikaVisitor visitor = new VyrokovaLogikaVisitor();
                Node syntaxTree = visitor.Visit(tree);
                pSyntaxTree = syntaxTree;
                return true;
            }
            else
            {
                Console.WriteLine("Repair your solution");
                return false;
            }
        }

        // This method traverses the parse tree and prints information about each node.
        private void Traverse(IParseTree tree, string[] ruleNames, IVocabulary vocabulary, int indent = 0)
        {
            // If the current node represents the end of the file, do nothing and return.
            if (tree.GetText() == "<EOF>")
            {
                return;
            }
            // If the current node is a terminal node (leaf node in the parse tree).
            else if (tree is TerminalNodeImpl terminalNode)
            {
                // Retrieve the symbolic name of the token type from the vocabulary, 
                // or use the actual text if no symbolic name is found.
                string tokenName = vocabulary.GetSymbolicName(terminalNode.Symbol.Type) ?? terminalNode.GetText();

                // Print information about the terminal node.
                Console.WriteLine("{0}{1}='{2}'", new string(' ', indent * 2), tokenName, terminalNode.GetText());
            }
            // If the current node is a parser rule context (non-terminal node in the parse tree).
            else if (tree is ParserRuleContext parserRuleContext)
            {
                // Print the name of the rule corresponding to the parser rule context.
                Console.WriteLine("{0}{1}", new string(' ', indent * 2), ruleNames[parserRuleContext.RuleIndex]);

                // Recursively traverse and print information about each child node.
                foreach (var child in parserRuleContext.children)
                {
                    Traverse(child, ruleNames, vocabulary, indent + 1);
                }
            }
        }
    }
}

