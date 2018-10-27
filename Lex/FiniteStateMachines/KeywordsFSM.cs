using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lex.FiniteStateMachines
{
    public static class KeywordsFSM
    {
        public static Token GetNextToken(ref string sourceCode, ref int index, string attribute = "")
        {
            int state = 0;

            while (index != sourceCode.Length)
            {
                char c = sourceCode[index];

                switch (state)
                {
                    #region State 0 - start characters
                    case 0:
                        if (c == 'i' || c == 'I')
                        {
                            state = 1;
                        }
                        else if (c == 'c' || c == 'C')
                        {
                            state = 2;
                        }
                        else if (c == 'd' || c == 'D')
                        {
                            state = 3;
                        }
                        else if (c == 'e' || c == 'E')
                        {
                            state = 4;
                        }
                        else if (c == 'f' || c == 'F')
                        {
                            state = 5;
                        }
                        else if(c == 'b' || c == 'B')
                        {
                            state = 6;
                        }
                        else if(c == 'l' || c == 'L')
                        {
                            state = 7;
                        }
                        else if(c == 'r' || c == 'R'){
                            state = 8;
                        }
                        else if(c == 's' || c == 'S')
                        {
                            state = 9;
                        }
                        else if(c == 'w' || c == 'W')
                        {
                            state = 10;
                        }
                        else
                        {
                            //Prejdu do stavu, kde se preda rizeni automatu pro rozpoznavani promennych
                            state = 11;
                        }
                        break;
                    #endregion
                    #region State 1 - IF
                    case 1:
                        if(sourceCode.Substring(index).Length >= 2 && sourceCode.Substring(index, 2).ToLower() == "if")
                        {
                            index += 2;

                            if (index != sourceCode.Length && (char.IsLetterOrDigit(sourceCode[index]) || sourceCode[index] == '_'))
                            {
                                attribute += "if";
                                state = 11;
                            }
                            else
                            {
                                return new Token(TokenType.ifType);
                            }
                        }
                        else
                        {
                            state = 11;
                        }
                        break;
                    #endregion
                    #region State 2 - CONTINUE
                    case 2:
                        if(sourceCode.Substring(index).Length >= 8 && sourceCode.Substring(index, 8).ToLower() == "continue")
                        {
                            index += 8;


                            if (index != sourceCode.Length && (char.IsLetterOrDigit(sourceCode[index]) || sourceCode[index] == '_'))
                            {
                                attribute += "continue";
                                state = 11;
                            }
                            else
                            {
                                return new Token(TokenType.continueType);
                            }
                        }
                        else
                        {
                            state = 11;
                        }
                        break;
                    #endregion
                    #region State 3 - DO
                    case 3:
                        if (sourceCode.Substring(index).Length >= 2 && sourceCode.Substring(index, 2).ToLower() == "do")
                        {
                            index += 2;

                            if (index != sourceCode.Length && (char.IsLetterOrDigit(sourceCode[index]) || sourceCode[index] == '_'))
                            {
                                attribute += "do";
                                state = 11;
                            }
                            else
                            {
                                return new Token(TokenType.doType);
                            }
                        }
                        else
                        {
                            state = 11;
                        }
                        break;
                    #endregion
                    #region State 4 - ELSE
                    case 4:
                        if (sourceCode.Substring(index).Length >= 4 && sourceCode.Substring(index, 4).ToLower() == "else")
                        {
                            index += 4;

                            if (index != sourceCode.Length && (char.IsLetterOrDigit(sourceCode[index]) || sourceCode[index] == '_'))
                            {
                                attribute += "else";
                                state = 11;
                            }
                            else
                            {
                                return new Token(TokenType.elseType);
                            }
                        }
                        else
                        {
                            state = 11;
                        }
                        break;
                    #endregion
                    #region State 5 - FOR
                    case 5:
                        if (sourceCode.Substring(index).Length >= 3 && sourceCode.Substring(index, 3).ToLower() == "for")
                        {
                            index += 3;

                            if (index != sourceCode.Length && (char.IsLetterOrDigit(sourceCode[index]) || sourceCode[index] == '_'))
                            {
                                attribute += "for";
                                state = 11;
                            }
                            else
                            {
                                return new Token(TokenType.forType);
                            }

                        }
                        else
                        {
                            state = 11;
                        }
                        break;
                    #endregion
                    #region State 6 - BREAK
                    case 6:
                        if (sourceCode.Substring(index).Length >= 5 && sourceCode.Substring(index, 5).ToLower() == "break")
                        {
                            index += 5;

                            if (index != sourceCode.Length && (char.IsLetterOrDigit(sourceCode[index]) || sourceCode[index] == '_'))
                            {
                                attribute += "break";
                                state = 11;
                            }
                            else
                            {
                                return new Token(TokenType.breakType);
                            }
                        }
                        else
                        {
                            state = 11;
                        }
                        break;
                    #endregion
                    #region State 7 - LONG
                    case 7:
                        if (sourceCode.Substring(index).Length >= 4 && sourceCode.Substring(index, 4).ToLower() == "long")
                        {
                            index += 4;

                            if (index != sourceCode.Length && (char.IsLetterOrDigit(sourceCode[index]) || sourceCode[index] == '_'))
                            {
                                attribute += "long";
                                state = 11;
                            }
                            else
                            {
                                return new Token(TokenType.longType);
                            }
                        }
                        else
                        {
                            state = 11;
                        }
                        break;
                    #endregion
                    #region State 8 - RETURN
                    case 8:
                        if (sourceCode.Substring(index).Length >= 6 && sourceCode.Substring(index, 6).ToLower() == "return")
                        {
                            index += 6;

                            if (index != sourceCode.Length && (char.IsLetterOrDigit(sourceCode[index]) || sourceCode[index] == '_'))
                            {
                                attribute += "return";
                                state = 11;
                            }
                            else
                            {
                                return new Token(TokenType.returnType);
                            }
                        }
                        else
                        {
                            state = 11;
                        }
                        break;
                    #endregion
                    #region State 9 - SIZEOF
                    case 9:
                        if (sourceCode.Substring(index).Length >= 6 && sourceCode.Substring(index, 6).ToLower() == "sizeof")
                        {
                            index += 6;

                            if (index != sourceCode.Length && (char.IsLetterOrDigit(sourceCode[index]) || sourceCode[index] == '_'))
                            {
                                attribute += "sizeof";
                                state = 11;
                            }
                            else
                            {
                                return new Token(TokenType.sizeofType);
                            }
                        }
                        else
                        {
                            state = 11;
                        }
                        break;
                    #endregion
                    #region State 10 - WHILE
                    case 10:
                        if (sourceCode.Substring(index).Length >= 5 && sourceCode.Substring(index, 5).ToLower() == "while")
                        {
                            index += 5;

                            if (index != sourceCode.Length && (char.IsLetterOrDigit(sourceCode[index]) || sourceCode[index] == '_'))
                            {
                                attribute += "while";
                                state = 11;
                            }
                            else
                            {
                                return new Token(TokenType.whileType);
                            }
                        }
                        else
                        {
                            state = 11;
                        }
                        break;
                    #endregion
                    #region State 11 - Switch control of machine for variables lookup
                    case 11:
                        return VariablesFSM.GetNextToken(ref sourceCode, ref index, attribute);
                    #endregion
                }
            }

            return new Token(TokenType.eofType);
        }
    }
}
