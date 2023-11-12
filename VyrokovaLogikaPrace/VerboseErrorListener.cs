using Antlr4.Runtime.Misc;
using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogikaPrace
{
    public class VerboseErrorListener : BaseErrorListener
    {
        private int errorCount = 0;
        public int GetErrorCount()
        {
            return errorCount;
        }

        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            // Check if the error is related to an unsupported operator
            if (offendingSymbol != null && offendingSymbol.Type == VyrokovaLogikaLexer.UNSUPPORTED_OPERATOR)
            {
                Console.WriteLine($"Unsupported operator found at line {line}, position {charPositionInLine}: {offendingSymbol.Text}");
                errorCount++;
            }
            else
            {
                // Handle other types of errors if needed
                Console.WriteLine($"Syntax error at line {line}, position {charPositionInLine}: {msg}");
            }
        }
    }
}
