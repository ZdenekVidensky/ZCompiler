using Parser.ADT.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxTreeVisualizer
{
    public static partial class Visualizer
    {
        public static void PrintCondition(ADCondition condition, string tab)
        {
            Console.WriteLine($"{tab}|- Condition:");

            Console.WriteLine($"{tab}\t If:");
            PrintExpression(condition.Condition, tab+"\t");

            if(condition.IfBody.Count() > 0)
            {
                foreach (var node in condition.IfBody)
                {
                    PrintNode(node, tab + "\t\t");
                }
            }
            
            if(condition.ElseBody != null && condition.ElseBody.Count() > 0)
            {
                Console.WriteLine($"{tab}\t Else:");
                foreach (var node in condition.ElseBody)
                {
                    PrintNode(node, tab + "\t\t");
                }
            }
        }
    }
}
