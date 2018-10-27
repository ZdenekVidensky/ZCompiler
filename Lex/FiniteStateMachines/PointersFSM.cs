using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lex.FiniteStateMachines
{
    public static class PointersFSM
    {
        public static Token GetNextToken(ref string sourceCode, ref int index, string attribute = "")
        {
            index++;
            while (index != sourceCode.Length)
            {
                char c = sourceCode[index];

                if (char.IsLetterOrDigit(c) || c == '_')
                {
                    attribute += c;
                    index++;
                }
                else
                {
                    return new Token(TokenType.pointer, attribute);
                }
            }

            return new Token(TokenType.pointer, attribute);
        }
    }
}
