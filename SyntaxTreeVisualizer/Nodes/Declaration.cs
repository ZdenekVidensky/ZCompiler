using Parser.ADT.Functions;
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
        public static void PrintDeclaration(ADDeclaration declaration, string tab)
        {
            foreach (var variable in declaration.Variables)
            {
                Console.WriteLine($"{tab}|- Decl: {variable.Name}");
                Console.WriteLine($"{tab}|\tType: {variable.Type}");
                Console.WriteLine($"{tab}|\tAccess: {variable.STRecord.Access}");

                if (variable.Type == Parser.ADT.Operands.ADVariable.VarType.array)
                {
                    Console.WriteLine($"{tab}|\tDimensions: {variable.ArrayDimensions.Count}");

                    foreach (var dimension in variable.ArrayDimensions)
                    {
                        if (dimension.ValuesCount != null)
                        {
                            Console.WriteLine($"{tab}|\tCount:");
                            PrintExpression(dimension.ValuesCount, tab + "\t\t");
                        }

                        if (dimension.Values.Count > 0)
                        {
                            Console.WriteLine($"{tab}|\tValues:");
                        }
                        foreach (var value in dimension.Values)
                        {

                            PrintExpression(value, tab + "\t\t");
                        }
                    }
                }
                else if (variable.Type == ADVariable.VarType.variable)
                {
                    if (variable.Value != null)
                    {
                        Console.WriteLine($"{tab}|\tValue:");
                        PrintExpression(variable.Value, tab + "\t\t");
                    }
                }
            }
        }

        private static void PrintVariableDeclaration(ADVariableDeclarations declaration, string tab)
        {
            foreach (var variable in declaration.Variables)
            {
                Console.WriteLine($"{tab}|- Decl: {variable.Name}");
                Console.WriteLine($"{tab}|\tType: {variable.Type}");
                Console.WriteLine($"{tab}|\tAccess: {variable.STRecord.Access}");

                if (variable.Type == Parser.ADT.Operands.ADVariable.VarType.array)
                {
                    Console.WriteLine($"{tab}|\tDimensions: {variable.ArrayDimensions.Count}");

                    foreach (var dimension in variable.ArrayDimensions)
                    {
                        if (dimension.ValuesCount != null)
                        {
                            Console.WriteLine($"{tab}|\tCount:");
                            PrintExpression(dimension.ValuesCount, tab + "\t\t");
                        }

                        if (dimension.Values.Count > 0)
                        {
                            Console.WriteLine($"{tab}|\tValues:");
                        }
                        foreach (var value in dimension.Values)
                        {

                            PrintExpression(value, tab + "\t\t");
                        }
                    }
                }
                else if (variable.Type == ADVariable.VarType.variable)
                {
                    if (variable.Value != null)
                    {
                        Console.WriteLine($"{tab}|\tValue:");
                        PrintExpression(variable.Value, tab + "\t\t");
                    }
                }
            }
        }

        public static void PrintFunctionDeclaration(ADFunctionDeclaration declaration, string tab)
        {
            Console.WriteLine($"{tab}|- Function: {declaration.Name}");

            if(declaration.Arguments.Count > 0)
            {
                Console.WriteLine($"{tab}\tArguments:");

                foreach(var arg in declaration.Arguments)
                {
                    Console.WriteLine($"{tab}\t\t\\-{arg.Name}");
                }
            }

            if(declaration.Body.Count() > 0)
            {
                Console.WriteLine($"{tab}\tBody:");
                foreach(var node in declaration.Body)
                {
                    PrintNode(node, tab + "\t\t");
                }
            }
        }
    }
}
