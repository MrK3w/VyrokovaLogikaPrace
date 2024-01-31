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
                if(offendingSymbol.Text == "<EOF>")
                {
                    var myMessage = $"Formule není správně ukončena na pozici {charPositionInLine + 1} zkontroluj prosím závorky!";
                    Console.WriteLine(myMessage);
                    Errors.Add(myMessage);
                    ErrorCount++;
                }
                else
                {
                    Console.WriteLine($"Unsupported operator found at line {line}, position {charPositionInLine}: {offendingSymbol.Text}");
                    ErrorCount++;
                }
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
            if (offendingSymbol != -1)
            {
                var myMessage = $"Nerozpoznán symbol {msg[msg.Length - 2]} na pozici {charPositionInLine+1}";
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
