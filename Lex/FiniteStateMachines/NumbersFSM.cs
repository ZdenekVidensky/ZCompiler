using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lex.FiniteStateMachines
{
    public static class NumbersFSM
    {
        public static Token GetNextToken(ref string sourceCode, ref int index, string attribute = "")
        {
            int state = 0;
            while(index != sourceCode.Length)
            {
                char c = sourceCode[index];

                switch (state)
                {
                    case 0:
                        if(c == '0')
                        {
                            state = 1;
                        }
                        else if (char.IsNumber(c))
                        {
                            state = 4;
                        }
                        else if(c == '-')
                        {
                        }
                        else
                        {
                            Console.Error.WriteLine("Lex error!");
                            Environment.Exit(2);
                        }
                        attribute += c;
                        break;
                    case 1:
                        if(char.ToLower(c) == 'x')
                        {
                            attribute += c;
                            state = 3;
                        }
                        else if(c == '0')
                        {
                            Console.Error.WriteLine("Lex error!");
                            Environment.Exit(2);
                        }
                        else if(char.IsNumber(c) && int.Parse(c.ToString()) >= 1 && int.Parse(c.ToString()) <= 7)
                        {
                            attribute += c;
                            state = 2;
                        }
                        else
                        {
                            return new Token(TokenType.decNumberType, attribute);
                        }
                        break;
                    case 2:
                        if(char.IsNumber(c) && int.Parse(c.ToString()) >= 0 && int.Parse(c.ToString()) <= 7)
                        {
                            attribute += c;
                        }
                        else if(char.IsNumber(c) && (int)c > 7)
                        {
                            Console.Error.WriteLine("Lex error!");
                            Environment.Exit(2);
                        }
                        else
                        {
                            return new Token(TokenType.octNumberType, attribute);
                        }
                        break;
                    case 3:
                        if(char.IsLetter(c) && (((int)c >= 65 && (int)c <= 70) || ((int)c >= 97 && (int)c <= 102)))
                        {
                            attribute += c;
                        }
                        else if (char.IsNumber(c))
                        {
                            attribute += c;
                        }
                        else if(char.ToLower(attribute.Last()) =='x')
                        {
                            Console.Error.WriteLine("Lex error!");
                            Environment.Exit(2);
                        }
                        else
                        {
                            return new Token(TokenType.hexNumberType, attribute);
                        }
                        break;
                    case 4:
                        if (char.IsNumber(c))
                        {
                            attribute += c;
                        }
                        else
                        {
                            return new Token(TokenType.decNumberType, attribute);
                        }
                        break;
                }

                index++;
            }

            if (attribute.StartsWith("0x") || attribute.StartsWith("-0x"))
            {
                return new Token(TokenType.hexNumberType, attribute);
            }
            else if (attribute.StartsWith("0") || attribute.StartsWith("-0"))
            {
                return new Token(TokenType.octNumberType, attribute);
            }
            else if(attribute != "-")
            {
                return new Token(TokenType.decNumberType, attribute);
            }
            else
            {
                Console.Error.WriteLine("Lex error!");
                Environment.Exit(2);
            }

            return null;
        }
    }
}
