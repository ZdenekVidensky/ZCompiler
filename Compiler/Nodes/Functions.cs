using Parser.ADT.Conditions;
using Parser.ADT.Functions;
using Parser.ADT.InnerStatements;
using Parser.ADT.Interfaces;
using Parser.ADT.Loops;
using Parser.ADT.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parser.ADT.Operands;
using Parser.ADT.OperationNodes;

namespace Compiler
{
    public static partial class AssemblyCompiler
    {
        public static void PrintFunctionDeclaration(ADFunctionDeclaration fceDeclaration, string globalAssigns = "")
        {
            Console.WriteLine($"{fceDeclaration.Name}:");

            Console.WriteLine("pushq %rbp");
            Console.WriteLine("movq %rsp, %rbp");

            int argumentsCounter = 1;

            foreach(var arg in fceDeclaration.Arguments)
            {
                argumentsCounter++;
                arg.STRecord.Address = $"{argumentsCounter * SIZE}(%rbp)";
            }

            string arrayDeclarations = string.Empty;
            int localVariablesCounter = GetAllLocalVariablesDeclarations(fceDeclaration.Body, 0, ref arrayDeclarations);


            if (localVariablesCounter > 0)
            {
                Console.WriteLine($"subq ${localVariablesCounter * SIZE}, %rsp");
                Console.WriteLine(arrayDeclarations);
            }

            var returnLabel = GenerateLabel();


            if(fceDeclaration.Name.ToLower() == "main")
            {
                Console.WriteLine(globalAssigns);
            }

            foreach (IADNode node in fceDeclaration.Body)
            {
                PrintNode(node, "", "", returnLabel);
            }

            Console.WriteLine("movq $0, %rax");

            Console.WriteLine($"{returnLabel}:");
            Console.WriteLine($"movq %rbp, %rsp");
            Console.WriteLine($"popq %rbp");
            Console.WriteLine($"ret ${(argumentsCounter - 1) * SIZE}");
        }
    
        public static int GetAllLocalVariablesDeclarations(IEnumerable<IADNode> nodes, int counter, ref string arrayDeclarations, IEnumerable<IADExpression> expressions = null)
        {
            if(nodes != null)
            {
                foreach (var node in nodes)
                {
                    if (node is ADVariableDeclarations)
                    {
                        foreach (var variable in (node as ADVariableDeclarations).Variables)
                        {
                            if (variable.STRecord != null)
                            {
                                if (variable.Type == ADVariable.VarType.variable)
                                {
                                    Console.WriteLine($"# deklarace promenne {variable.Name}");
                                    counter++;
                                    variable.STRecord.Address = $"-{counter * SIZE}(%rbp)";
                                }
                                else if (variable.Type == ADVariable.VarType.array)
                                {
                                    arrayDeclarations += ($"# deklarace pole {variable.Name}") + Environment.NewLine;

                                    if (variable.ArrayDimensions.Count > 0)
                                    {
                                        foreach (var dimension in variable.ArrayDimensions)
                                        {
                                            if (dimension.Values.Count > 0)
                                            {
                                                dimension.Values.Reverse();

                                                foreach (var value in dimension.Values)
                                                {
                                                    counter++;
                                                    arrayDeclarations += (PrintExpression(value));
                                                    arrayDeclarations += ($"movq %rdx, -{counter * SIZE}(%rbp)") + Environment.NewLine;
                                                }

                                                arrayDeclarations += ($"leaq -{counter * SIZE}(%rbp), %rsi") + Environment.NewLine;
                                                counter++;
                                                arrayDeclarations += ($"movq %rsi, -{counter * SIZE}(%rbp)") + Environment.NewLine;
                                                variable.STRecord.Address = $"-{counter * SIZE}(%rbp)";
                                            }
                                            else
                                            {
                                                if (dimension.ValuesCount != null)
                                                {
                                                    if (dimension.ValuesCount is ADConstant)
                                                    {
                                                        counter += int.Parse((dimension.ValuesCount as ADConstant).Value);

                                                        arrayDeclarations += ($"leaq -{counter * SIZE}(%rbp), %rsi") + Environment.NewLine;
                                                        counter++;
                                                        arrayDeclarations += ($"movq %rsi, -{counter * SIZE}(%rbp)") + Environment.NewLine;
                                                        variable.STRecord.Address = $"-{counter * SIZE}(%rbp)";
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if(variable.STRecord.Type == Parser.SymbolTable.STType.str)
                                    {
                                        var chars = variable.STRecord.Value.ToCharArray();

                                        Array.Reverse(chars);

                                        foreach (var value in chars)
                                        {
                                            counter++;
                                            arrayDeclarations += ($"movq $'{value}', -{counter * SIZE}(%rbp)") + Environment.NewLine;
                                        }

                                        arrayDeclarations += ($"leaq -{counter * SIZE}(%rbp), %rsi") + Environment.NewLine;
                                        counter++;
                                        arrayDeclarations += ($"movq %rsi, -{counter * SIZE}(%rbp)") + Environment.NewLine;
                                        variable.STRecord.Address = $"-{counter * SIZE}(%rbp)";
                                    }
                                }
                            }
                        }
                    }
                    else if (node is ADDoWhileLoop)
                    {
                        counter = GetAllLocalVariablesDeclarations((node as ADDoWhileLoop).Body, counter, ref arrayDeclarations);
                    }
                    else if (node is ADForLoop)
                    {
                        counter = GetAllLocalVariablesDeclarations((node as ADForLoop).Body, counter, ref arrayDeclarations);
                    }
                    else if (node is ADDoWhileLoop)
                    {
                        counter = GetAllLocalVariablesDeclarations((node as ADDoWhileLoop).Body, counter, ref arrayDeclarations);
                    }
                    else if (node is ADCondition)
                    {
                        counter = GetAllLocalVariablesDeclarations((node as ADCondition).IfBody, counter, ref arrayDeclarations);

                        if ((node as ADCondition).ElseBody != null)
                        {
                            counter = GetAllLocalVariablesDeclarations((node as ADCondition).ElseBody, counter, ref arrayDeclarations);
                        }
                    }
                    else if (node is ADInnerStatements)
                    {
                        counter = GetAllLocalVariablesDeclarations((node as ADInnerStatements).Statements, counter, ref arrayDeclarations);
                    }
                    else if (node is ADFunctionCall)
                    {
                        counter = GetAllLocalVariablesDeclarations(null, counter, ref arrayDeclarations, (node as ADFunctionCall).Arguments);
                    }
                }
            }

            return counter;
        }

        public static string PrintFunctionCall(ADFunctionCall fceCall)
        {
            string result = string.Empty;

            if(fceCall.Expression == null)
            {
                var fceName = fceCall.STRecord.Name;

                if (fceName == "print_long" || fceName == "print_char")
                {
                    result += (PrintExpression(fceCall.Arguments[0], false)) + Environment.NewLine;

                    result += ("movq %rdx, %rdi") + Environment.NewLine;

                    result += ($"call {fceName}") + Environment.NewLine;
                }
                else
                {
                    fceCall.Arguments.Reverse();

                    foreach (var arg in fceCall.Arguments)
                    {
                        result += (PrintExpression(arg)) + Environment.NewLine;
                        result += ("pushq %rdx") + Environment.NewLine;
                    }
                    result += ($"call {fceName}") + Environment.NewLine;
                }
            }
            else
            {
                fceCall.Arguments.Reverse();
                foreach (var arg in fceCall.Arguments)
                {
                    result += (PrintExpression(arg)) + Environment.NewLine;
                    result += ("pushq %rdx") + Environment.NewLine;
                }

                result += PrintExpression(fceCall.Expression);
                result += ($"call *%rdx") + Environment.NewLine;
            }

            return result;
        }
    }
}
