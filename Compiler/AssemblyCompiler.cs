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

namespace Compiler
{
    public static partial class AssemblyCompiler
    {
        public static int SIZE = 8;
        public static void Compile(ADTree tree)
        {
            Console.WriteLine("#################################################################");
            Console.WriteLine("############### HeroC compiler by Zdenek Vidensky ###############");
            Console.WriteLine("#################################################################");

            Console.WriteLine();
            Console.WriteLine();

            foreach(var node in tree.Nodes)
            {
                if(node is ADFunctionDeclaration)
                {
                    Console.WriteLine($".global {(node as ADFunctionDeclaration).Name}");
                    Console.WriteLine();
                    Console.WriteLine();
                }
            }

            string globalAssigns = string.Empty;

            Console.WriteLine(".data");
            foreach(var node in tree.Nodes)
            {
                if(node is ADDeclaration)
                {
                    globalAssigns = DeclareGlobalVariables(node as ADDeclaration);
                }
            }

            Console.WriteLine(".text");

            foreach (var node in tree.Nodes)
            {
                PrintNode(node, "", "", "", null, globalAssigns);
            }
        }

        public static string DeclareGlobalVariables(ADDeclaration variables)
        {
            string result = string.Empty;

            foreach(var variable in variables.Variables)
            {
                variable.STRecord.Address = variable.Name;
                Console.WriteLine($"{variable.STRecord.Name}:");

                if (variable.STRecord.Type == Parser.SymbolTable.STType.array)
                {
                    if(variable.ArrayDimensions.Count > 0)
                    {
                        foreach(var dimension in variable.ArrayDimensions)
                        {
                            if(dimension.Values.Count > 0)
                            {                           
                                foreach(var item in dimension.Values)
                                {
                                    Console.WriteLine($".8byte {(item as ADConstant).Value}"); 
                                }
                            }
                            else if(dimension.ValuesCount != null)
                            {
                                Console.WriteLine($".rept {(dimension.ValuesCount as ADConstant).Value}");
                                Console.WriteLine(".8byte 0");
                                Console.WriteLine(".endr");
                            }
                        }
                    }
                }
                else if(variable.STRecord.Type == Parser.SymbolTable.STType.variable)
                {
                    Console.WriteLine($".8byte 0");

                    if(variable.Value != null)
                    {
                        result += PrintExpression(variable.Value) + Environment.NewLine;
                        result += PrintAssignRdx(variable) + Environment.NewLine;
                    }
                }
            }

            return result;
        }

        public static void PrintBreak(string breakLabel)
        {
            Console.WriteLine("# tisknu break");
            Console.WriteLine($"jmp {breakLabel}");
        }

        public static void PrintContinue(string continueLabel)
        {
            Console.WriteLine("# tisknu continue");
            Console.WriteLine($"jmp {continueLabel}");
        }

        public static void PrintNode(IADNode node, string breakLabel = "", string continueLabel = "", string returnLabel = "", IADExpression forIncrement = null, string globalAssigns = "")
        {
            if(node is ADFunctionDeclaration)
            {
                Console.WriteLine($"# deklarace funkce {(node as ADFunctionDeclaration).Name}");
                PrintFunctionDeclaration(node as ADFunctionDeclaration, globalAssigns);
                Console.WriteLine();
                Console.WriteLine();
            }
            else if(node is ADFunctionCall)
            {
                Console.WriteLine("# volani funkce");
                Console.WriteLine(PrintFunctionCall(node as ADFunctionCall));
            }
            else if(node is ADForLoop)
            {
                Console.WriteLine("# cykus for");
                PrintForLoop(node as ADForLoop, returnLabel);
            }
            else if(node is ADWhileLoop)
            {
                Console.WriteLine("# cykus while");
                PrintWhileLoop(node as ADWhileLoop, returnLabel);
            }
            else if(node is ADDoWhileLoop)
            {
                Console.WriteLine("# cykus do-while");
                PrintDoWhileLoop(node as ADDoWhileLoop, returnLabel);
            }
            else if(node is ADStatementExpression)
            {
                Console.WriteLine(PrintExpression((node as ADStatementExpression).Expression));
            }
            else if(node is ADCondition)
            {
                PrintCondition(node as ADCondition, breakLabel, continueLabel, returnLabel, forIncrement);
            }
            else if(node is ADBreak)
            {
                PrintBreak(breakLabel);
            }
            else if(node is ADContinue)
            {
                PrintContinue(continueLabel);
            }
            else if(node is ADInnerStatements)
            {
                foreach(var item in (node as ADInnerStatements).Statements)
                {
                    PrintNode(item, breakLabel, continueLabel, returnLabel, forIncrement);
                }
            }
            else if(node is ADReturn)
            {
                Console.WriteLine(PrintExpression((node as ADReturn).Expression, false));
                Console.WriteLine("movq %rdx, %rax");
                Console.WriteLine($"jmp {returnLabel}");
            }
            else if(node is ADArrayValue)
            {
                PrintArrayValue(node as ADArrayValue);
            }
            else if(node is ADVariableDeclarations)
            {
                foreach(var variable in (node as ADVariableDeclarations).Variables)
                {
                    if (variable.Value != null && variable.Type != ADVariable.VarType.array)
                    {
                        Console.WriteLine($"# Prirazeni promenne {variable.Name}");
                        Console.WriteLine(PrintExpression(variable.Value));
                        Console.WriteLine($"movq %rdx, {variable.STRecord.Address}");
                    }
                }
            }
        }
 
        public static string GenerateLabel()
        {
            var result = String.Concat(Guid.NewGuid().ToString("N").Select(c => (char)(c + 17)));
            return "z" + result;
        }
    }
}
