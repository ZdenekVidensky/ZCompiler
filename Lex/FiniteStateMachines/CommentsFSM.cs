using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lex.FiniteStateMachines
{
    public class CommentsFSM
    {
        public static void GetNextToken(ref string sourceCode, ref int index, string attribute = "")
        {
            int state = 0;
            while (index != sourceCode.Length)
            {
                char c = sourceCode[index];
                switch (state)
                {
                    case 0:
                        if (c == '*')
                        {
                            state = 1;
                            index++;
                        }
                        else
                        {
                            index++;
                        }
                    break;
                    case 1:
                        if(c == '/')
                        {
                            return;
                        }
                        else
                        {
                            state = 0;
                        }
                    break;
                }
            }
        }
    }
}
