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
        public static void PrintArrayValue(ADArrayValue array, string tab)
        {
            Console.WriteLine($"{tab}|- Array:");

            if(array.STRecord != null)
            {
                Console.WriteLine($"{tab}|\tName: {array.STRecord.Name}");
            }
            else if(array.ArrayExpression != null)
            {
                Console.WriteLine($"{tab}|\tExpression:");
                PrintExpression(array.ArrayExpression, tab + "\t\t");
            }

            if(array.Indexes.Count > 0)
            {
                Console.WriteLine($"{tab}|\tIndexes:");
                foreach(var item in array.Indexes)
                {
                    PrintExpression(item, tab + "\t\t");
                }
            }
        }

        public static void PrintSizeOf(string tab)
        {
            Console.WriteLine($"{tab}Sizeof(long)");
        }

        public static void PrintVariable(ADVariable variable, string tab)
        {
            if(variable.Type == ADVariable.VarType.variable)
            {
                Console.WriteLine($"{tab}Variable: {variable.Name}");
            }
            else if(variable.Type == ADVariable.VarType.array)
            {
                Console.WriteLine($"{tab}Array: {variable.Name}:");

                Console.WriteLine($"{tab}\t Dimensions:");
                foreach(var dimension in variable.ArrayDimensions)
                {
                    if (dimension.ValuesCount != null)
                    {
                        Console.WriteLine($"{tab}\t\tCount:");
                        PrintExpression(dimension.ValuesCount, tab + "\t\t\t");
                    }

                    if (dimension.Values.Count > 0)
                    {
                        Console.WriteLine($"{tab}\t\tValues:");
                    }
                    foreach (var value in dimension.Values)
                    {
                        PrintExpression(value, tab + "\t\t\t");
                    }
                }
            }
            else if(variable.Type == ADVariable.VarType.pointer)
            {
                Console.WriteLine($"{tab} Pointer {variable.Name}:");
            }

            if(variable.Value != null)
            {
                Console.WriteLine($"{tab}\t Value");
                PrintExpression(variable.Value, tab + "\t\t");
            }
        }

        public static void PrintVariableAssignment(ADVariableAssignment asgn, string tab)
        {
            Console.WriteLine($"{tab}|- Assign:");

            if (asgn.STRecord != null)
            {
                Console.WriteLine($"{tab}|\tName: {asgn.STRecord.Name}");
            }


            Console.WriteLine($"{tab}|\tOperator: {asgn.AssignOperator}");

            if(asgn.Expression != null)
            {
                Console.WriteLine($"{tab}|\tExpression:");
                PrintExpression(asgn.Expression, tab + "\t\t");
            }

            if (asgn.Dereference)
            {
                Console.WriteLine($"{tab}|\tType: Dereference");
            }         
        }

        public static void PrintStatementExpression(ADStatementExpression expr, string tab)
        {
            Console.WriteLine($"{tab}|- Expression");
            PrintExpression(expr.Expression, tab + '\t');
        }
    }
}
