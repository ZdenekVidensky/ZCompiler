using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lex.FiniteStateMachines
{
    public class VariablesFSM
    {
        public static Token GetNextToken(ref string sourceCode, ref int index, string attribute = "")
        {
            while (index != sourceCode.Length)
            {
                char c = sourceCode[index];

                if(char.IsLetterOrDigit(c) || c == '_')
                {
                    attribute += c;
                    index++;
                }
                else
                {
                    return new Token(TokenType.idType, attribute);
                }
            }

            return new Token(TokenType.idType, attribute);
        }
    }
}
