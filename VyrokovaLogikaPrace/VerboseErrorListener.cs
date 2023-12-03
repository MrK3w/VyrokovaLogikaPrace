using Antlr4.Runtime.Misc;
using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogikaPrace
{
    public class VerboseErrorListener : BaseErrorListener, IAntlrErrorListener<int>
    {
        private int errorCount = 0;
        public int GetErrorCount()
        {
            return errorCount;
        }

        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            // Check if the error is related to an unsupported operator
            if (offendingSymbol != null)
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

        public void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] int offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
        {
            // Check if the error is related to an unsupported operator
            if (offendingSymbol != null)
            {
                Console.WriteLine($"Nerozpoznan symbol {msg[msg.Length-2]} na pozici {charPositionInLine}");
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
