using Parser.ADT.Functions;
using Parser.ADT.Interfaces;
using Parser.ADT.Operands;
using Parser.ADT.OperationNodes;
using Parser.Precedence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxTreeVisualizer
{
    public static partial class Visualizer
    {
        public static void PrintExpression(IADExpression expression, string tab)
        {
            if (expression is ADConstant)
            {
                Console.WriteLine($"{tab}Constant: {((ADConstant)expression).Value}");
            }

            if(expression is ADExpression)
            {
                var expr = expression as ADExpression;

                if(expr.Left != null)
                {
                    PrintExpression(expr.Left, tab + '\t');
                }

                Console.WriteLine($"{tab}\t{GetOperator(expr.Operator)}");

                if(expr.Right != null)
                {
                    PrintExpression(expr.Right, tab + '\t');
                }
            }

            if(expression is ADArrayValue)
            {
                PrintArrayValue((ADArrayValue)expression, tab);
            }

            if(expression is ADSizeOfValue)
            {
                PrintSizeOf(tab);
            }

            if(expression is ADVariable)
            {
                PrintVariable((ADVariable)expression, tab);
            }

            if(expression is ADFunctionCall)
            {
                PrintFunctionCall((ADFunctionCall)expression, tab);
            }
        }

        public static string GetOperator(TermType opr)
        {
            switch (opr)
            {
                case TermType.andEq:
                    return "&=";
                case TermType.asgn:
                    return "=";
                case TermType.binAnd:
                    return "&";
                case TermType.binOr:
                    return "|";
                case TermType.cCond:
                    return "?";
                case TermType.cElse:
                    return ":";
                case TermType.compl:
                    return "~";
                case TermType.dec:
                    return "--";
                case TermType.dereference:
                    return "* (dereference)";
                case TermType.div:
                    return "/";
                case TermType.divEq:
                    return "/=";
                case TermType.eq:
                    return "==";
                case TermType.grt:
                    return ">";
                case TermType.grtEq:
                    return ">=";
                case TermType.inc:
                    return "++";
                case TermType.left:
                    return "<<";
                case TermType.leftEq:
                    return "<<=";
                case TermType.less:
                    return "<";
                case TermType.lessEq:
                    return "<=";
                case TermType.logAnd:
                    return "&&";
                case TermType.logOr:
                    return "||";
                case TermType.minus:
                    return "-";
                case TermType.minusEq:
                    return "-=";
                case TermType.mod:
                    return "%";
                case TermType.modEq:
                    return "%=";
                case TermType.mul:
                    return "*";
                case TermType.mulEq:
                    return "*=";
                case TermType.negEq:
                    return "!=";
                case TermType.not:
                    return "!";
                case TermType.orEq:
                    return "|=";
                case TermType.plus:
                    return "+";
                case TermType.plusEq:
                    return "+=";
                case TermType.reference:
                    return "& (reference)";
                case TermType.right:
                    return ">>";
                case TermType.rightEq:
                    return ">>=";
                case TermType.xor:
                    return "^";
                case TermType.xorEq:
                    return "^=";
            }

            return "";
        }
    }
}
