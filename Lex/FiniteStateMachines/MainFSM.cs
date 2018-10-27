using Lex.FiniteStateMachines;
using SourceCodeImport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lex
{
    public static class MainFSM
    {
        public static int Index = 0;
        public static string SourceCode;
        public static int RowCounter = 0;

        public static Token GetNextToken()
        {
            Token result = null;

            while(Index != SourceCode.Length)
            {
                char c = SourceCode[Index];

                if (char.IsLetter(c))
                {
                    result =  KeywordsFSM.GetNextToken(ref SourceCode, ref Index);
                }
                else if(char.IsLetter(c) || c == '_')
                {
                    Index++;
                    result = VariablesFSM.GetNextToken(ref SourceCode, ref Index, c.ToString());
                }
                else if(char.IsNumber(c))
                {
                    result = NumbersFSM.GetNextToken(ref SourceCode, ref Index);
                }
                else if (char.IsWhiteSpace(c))
                {
                    if(c == '\n')
                    {
                        RowCounter++;
                    }

                    Index++;
                    continue;
                }
                else
                {
                    result = OperatorsFSM.GetNextToken(ref SourceCode, ref Index);
                }

                if (result.Type == TokenType.commentType)
                {
                    continue;
                }
                else 
                {
                    return result;
                }
            }
            return new Token(TokenType.eofType);
        }

        public static Token PeekNextToken()
        {
            int prevIndex = Index;
            var token = GetNextToken();
            Index = prevIndex;

            return token;
        }

        public static Token PeekNextToken(int count)
        {
            int prevIndex = Index;
            Token token = null;

            for(int i = 0; i < count; i++)
            {
                token = MainFSM.GetNextToken();
            }

            Index = prevIndex;
            return token;
        }
    }
}
