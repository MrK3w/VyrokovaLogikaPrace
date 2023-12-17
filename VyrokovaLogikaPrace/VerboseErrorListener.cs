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
        public int ErrorCount { get; private set; } = 0;
        public List<string> Errors { get; private set; } = new List<string>();

        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            // Check if the error is related to an unsupported operator
            if (offendingSymbol != null)
            {
                Console.WriteLine($"Unsupported operator found at line {line}, position {charPositionInLine}: {offendingSymbol.Text}");
                ErrorCount++;
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
                var myMessage = $"Nerozpoznan symbol {msg[msg.Length - 2]} na pozici {charPositionInLine}";
                Console.WriteLine(myMessage);
                Errors.Add(myMessage);
                ErrorCount++;
            }
            else
            {
                // Handle other types of errors if needed
                var myMessage = $"Syntax error at line {line}, position {charPositionInLine}: {msg}";
                Console.WriteLine(myMessage);
                Errors.Add(myMessage);
                ErrorCount++;
            }
        }

        
    }
}
