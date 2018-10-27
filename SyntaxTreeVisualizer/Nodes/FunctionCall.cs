using Parser.ADT.Functions;
using Parser.ADT.InnerStatements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxTreeVisualizer
{
    public static partial class Visualizer
    {
        public static void PrintFunctionCall(ADFunctionCall fce, string tab)
        {
            Console.WriteLine($"{tab}|- Function Call:");

            if(fce.STRecord != null)
            {
                Console.WriteLine($"{tab}|\tName: {fce.STRecord.Name}");
            }
            else if(fce.Expression != null)
            {
                Console.WriteLine($"{tab}|\tAdress:");
                PrintExpression(fce.Expression, tab + "\t\t");
            }

            if(fce.Arguments.Count > 0)
            {
                Console.WriteLine($"{tab}|\tArguments:");
                foreach(var expr in fce.Arguments)
                {
                    PrintExpression(expr, tab + "\t\t");
                }
            }
        }

        public static void PrintReturn(ADReturn rtn, string tab)
        {
            Console.WriteLine($"{tab}|- Return");

            if(rtn.Expression != null)
            {
                PrintExpression(rtn.Expression, tab + '\t');
            }
        }

        public static void PrintContinue(string tab)
        {
            Console.WriteLine($"{tab}|- Continue");
        }

        public static void PrintBreak(string tab)
        {
            Console.WriteLine($"{tab}|- Break");
        }

        public static void PrintInnerStatement(ADInnerStatements statement, string tab)
        {
            Console.WriteLine($"{tab}|- Statement:");

            foreach(var node in statement.Statements)
            {
                PrintNode(node, tab + '\t');
            }
        }
    }
}
