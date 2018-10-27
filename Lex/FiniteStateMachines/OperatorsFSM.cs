using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lex.FiniteStateMachines
{
    public class OperatorsFSM
    {
        public static Token GetNextToken(ref string sourceCode, ref int index, string attribute = "")
        {
            int state = 0;
            while (index != sourceCode.Length)
            {
                char c = sourceCode[index];
                index++;

                switch (state)
                {
                    case 0:
                        if (char.IsWhiteSpace(c))
                        {
                        }
                        else if (c == '!')
                        {
                            state = 1;
                        }
                        else if (c == '%')
                        {
                            state = 2;
                        }
                        else if (c == '&')
                        {
                            state = 3;
                        }
                        else if (c == '*')
                        {
                            state = 4;
                        }
                        else if (c == '+')
                        {
                            state = 5;
                        }
                        else if (c == '-')
                        {
                            state = 6;
                        }
                        else if (c == '/')
                        {
                            state = 7;
                        }
                        else if (c == ':')
                        {
                            return new Token(TokenType.tElseType);
                        }
                        else if (c == '<')
                        {
                            state = 8;
                        }
                        else if (c == '=')
                        {
                            state = 10;
                        }
                        else if (c == '>')
                        {
                            state = 11;
                        }
                        else if (c == '?')
                        {
                            return new Token(TokenType.tIfType);
                        }
                        else if (c == '^')
                        {
                            state = 13;
                        }
                        else if (c == '|')
                        {
                            state = 14;
                        }
                        else if (c == '~')
                        {
                            return new Token(TokenType.bNotType);
                        }
                        else if (c == ';')
                        {
                            return new Token(TokenType.semiType);
                        }
                        else if(c == ',')
                        {
                            return new Token(TokenType.comType);
                        }
                        else if (c == '(')
                        {
                            return new Token(TokenType.leftRBType);
                        }
                        else if (c == ')')
                        {
                            return new Token(TokenType.rightRBType);
                        }
                        else if (c == '{')
                        {
                            return new Token(TokenType.leftCBType);
                        }
                        else if (c == '}')
                        {
                            return new Token(TokenType.rightCBType);
                        }
                        else if (c == '[')
                        {
                            return new Token(TokenType.leftABType);
                        }
                        else if (c == ']')
                        {
                            return new Token(TokenType.rightABType);
                        }
                        else if (c == '\"')
                        {
                            state = 15;
                        }
                        else if (c == '\'')
                        {
                            state = 16;
                        }
                        else
                        {
                            Console.Error.WriteLine("Lex error!");
                            Environment.Exit(2);
                        }
                        break;
                    case 1:
                        if (c == '=')
                        {
                            return new Token(TokenType.exrEqType);
                        }
                        else
                        {
                            index--;
                            return new Token(TokenType.exrType);
                        }
                    case 2:
                        if (c == '=')
                        {
                            return new Token(TokenType.procEq);
                        }
                        else
                        {
                            index--;
                            return new Token(TokenType.procType);
                        }
                    case 3:
                        if (c == '&')
                        {
                            return new Token(TokenType.andType);
                        }
                        else if (c == '=')
                        {
                            index--;
                            return new Token(TokenType.ampEqType);
                        }
                        else
                        {
                            index--;
                            return new Token(TokenType.ampType);
                        }
                    case 4:
                        if (c == '=')
                        {
                            return new Token(TokenType.multEqType);
                        }
                        else
                        {
                            index--;
                            return new Token(TokenType.starType);
                        }
                    case 5:
                        if (c == '+')
                        {
                            return new Token(TokenType.incrType);
                        }
                        else if (c == '=')
                        {
                            return new Token(TokenType.plusEqType);
                        }
                        else
                        {
                            index--;
                            return new Token(TokenType.plusType);
                        }
                    case 6:
                        if (c == '-')
                        {
                            return new Token(TokenType.decrType);
                        }
                        else if (c == '=')
                        {
                            return new Token(TokenType.minusEqType);
                        }
                        else if (char.IsNumber(c))
                        {
                            index--;
                            return NumbersFSM.GetNextToken(ref sourceCode, ref index, "-");
                        }
                        else
                        {
                            index--;
                            return new Token(TokenType.minusType);
                        }
                    case 7:
                        if (c == '*')
                        {
                            CommentsFSM.GetNextToken(ref sourceCode, ref index);
                            index++;
                            return new Token(TokenType.commentType);
                        }
                        else if (c == '=')
                        {
                            return new Token(TokenType.divEqType);
                        }
                        else
                        {
                            index--;
                            return new Token(TokenType.divType);
                        }
                    case 8:
                        if (c == '<')
                        {
                            state = 9;
                        }
                        else if (c == '=')
                        {
                            return new Token(TokenType.lessEqType);
                        }
                        else
                        {
                            index--;
                            return new Token(TokenType.lessType);
                        }
                        break;
                    case 9:
                        if (c == '=')
                        {
                            return new Token(TokenType.leftEqType);
                        }
                        else
                        {
                            index--;
                            return new Token(TokenType.leftType);
                        }
                    case 10:
                        if(c == '=')
                        {
                            return new Token(TokenType.eqEqType);
                        }
                        else
                        {
                            index--;
                            return new Token(TokenType.asgnType);
                        }
                    case 11:
                        if(c == '>')
                        {
                            state = 12;
                        }
                        else if(c == '=')
                        {
                            return new Token(TokenType.grtEqType);
                        }
                        else
                        {
                            index--;
                            return new Token(TokenType.grtType);
                        }
                        break;
                    case 12:
                        if(c == '=')
                        {
                            return new Token(TokenType.rightEqType);
                        }
                        else
                        {
                            index--;
                            return new Token(TokenType.rightType);
                        }
                    case 13:
                        if(c == '=')
                        {
                            return new Token(TokenType.xorEqType);
                        }
                        else
                        {
                            index--;
                            return new Token(TokenType.xorType);
                        }
                    case 14:
                        if(c == '=')
                        {
                            return new Token(TokenType.bOrEqType);
                        }
                        else if(c == '|')
                        {
                            return new Token(TokenType.lOrType);
                        }
                        else
                        {
                            index--;
                            return new Token(TokenType.bOrType);
                        }
                    case 15:
                        if (c == '\\')
                        {
                            state = 17;
                        }
                        else if (c == '\"')
                        {
                            return new Token(TokenType.stringType, attribute);
                        }
                        else
                        {
                            attribute += c;
                        }
                        break;
                    case 16:
                        if (c == '\'')
                        {
                            return new Token(TokenType.charType, $"\'{attribute}\'");
                        }
                        else if(attribute.Length > 0)
                        {
                            Console.Error.WriteLine("Lex error!");
                            Environment.Exit(2);
                        }
                        else
                        {
                            attribute += c;
                        }
                        break;
                    case 17:
                        attribute += '\\';
                        attribute += c;
                        state = 15;
                        break;
                }
            }
                return null;
        }
    }
}
