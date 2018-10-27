using Parser.ADT.Loops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxTreeVisualizer
{
    public static partial class Visualizer
    {
        public static void PrintForLoop(ADForLoop loop, string tab)
        {
            Console.WriteLine($"{tab}|- For loop");
            Console.WriteLine($"{tab}|\t Initial:");
            PrintExpression(loop.DeclarationPart, tab +"\t\t");

            Console.WriteLine($"{tab}|\t Condition:");
            PrintExpression(loop.ConditionPart, tab + "\t\t");

            Console.WriteLine($"{tab}|\t Increment:");
            PrintExpression(loop.IncrementalPart, tab + "\t\t");

            if(loop.Body.Count() > 0)
            {
                Console.WriteLine($"{tab}|\t Body:");
                foreach(var node in loop.Body)
                {
                    PrintNode(node, tab + "\t\t");
                }
            }
        }

        public static void PrintWhileLoop(ADWhileLoop loop, string tab)
        {
            Console.WriteLine($"{tab}|- While loop");
     
            Console.WriteLine($"{tab}|\tCondition:");
            PrintExpression(loop.Condition, tab + "\t\t");

            if(loop.Body.Count() > 0)
            {
                Console.WriteLine($"{tab}|\tBody:");
                foreach(var node in loop.Body)
                {
                    PrintNode(node, tab + "\t\t");
                }
            }
        }

        public static void PrintDoWhileLoop(ADDoWhileLoop loop, string tab)
        {
            Console.WriteLine($"{tab}|- Do-while loop");

            Console.WriteLine($"{tab}|\tCondition:");
            PrintExpression(loop.Condition, tab + "\t\t");

            if (loop.Body.Count() > 0)
            {
                Console.WriteLine($"{tab}|\tBody:");
                foreach (var node in loop.Body)
                {
                    PrintNode(node, tab + "\t\t");
                }
            }
        }
    }
}
