using Parser.ADT;
using Parser.ADT.Commands;
using Parser.ADT.Conditions;
using Parser.ADT.Functions;
using Parser.ADT.InnerStatements;
using Parser.ADT.Interfaces;
using Parser.ADT.Loops;
using Parser.ADT.Operands;
using Parser.ADT.OperationNodes;
using Parser.ADT.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxTreeVisualizer
{
    public static partial class Visualizer
    {
        public static void PrintTree(ADTree tree, string fileName)
        {
            string tab = "\t";

            Console.WriteLine("**********************************************");
            Console.WriteLine($"************** {fileName} *****************");
            Console.WriteLine("**********************************************");

            Console.WriteLine("+ Root");

            foreach(var node in tree.Nodes)
            {
                PrintNode(node, tab);
            }
        }

        public static void PrintNode(IADNode node, string tab)
        {
            if (node is ADDeclaration)
            {
                PrintDeclaration((ADDeclaration)node, tab);
            }
            else if (node is ADFunctionDeclaration)
            {
                PrintFunctionDeclaration((ADFunctionDeclaration)node, tab);
            }
            else if (node is ADVariableDeclarations)
            {
                PrintVariableDeclaration((ADVariableDeclarations)node, tab);
            }
            else if (node is ADForLoop)
            {
                PrintForLoop((ADForLoop)node, tab);
            }
            else if (node is ADWhileLoop)
            {
                PrintWhileLoop((ADWhileLoop)node, tab);
            }
            else if (node is ADDoWhileLoop)
            {
                PrintDoWhileLoop((ADDoWhileLoop)node, tab);
            }
            else if (node is ADCondition)
            {
                PrintCondition((ADCondition)node, tab);
            }
            else if (node is ADFunctionCall)
            {
                PrintFunctionCall((ADFunctionCall)node, tab);
            }
            else if (node is ADReturn)
            {
                PrintReturn((ADReturn)node, tab);
            }
            else if (node is ADContinue)
            {
                PrintContinue(tab);
            }
            else if (node is ADInnerStatements)
            {
                PrintInnerStatement((ADInnerStatements)node, tab);
            }
            else if (node is ADArrayValue)
            {
                PrintArrayValue((ADArrayValue)node, tab);
            }
            else if(node is ADSizeOfValue)
            {
                PrintSizeOf(tab);
            }
            else if(node is ADVariable)
            {
                PrintVariable((ADVariable)node, tab);
            }
            else if(node is ADVariableAssignment)
            {
                PrintVariableAssignment((ADVariableAssignment)node, tab);
            }
            else if(node is ADStatementExpression)
            {
                PrintStatementExpression((ADStatementExpression)node, tab);
            }
            else if(node is ADBreak)
            {
                PrintBreak(tab);
            }
        }
    }
}
