 using Parser.ADT.Commands;
using Parser.ADT.Conditions;
using Parser.ADT.Interfaces;
using Parser.ADT.Operands;
using Parser.ADT.OperationNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public static partial class AssemblyCompiler
    {
        public static void EvalCondition(string endLabel)
        {
            Console.WriteLine("cmp $0, %rdx");
            Console.WriteLine($"je {endLabel}");
        }

        public static void PrintCondition(ADCondition condition, string breakLabel = "", string continueLabel = "", string returnLabel = "", IADExpression forIncrement = null)
        {
            var endLabel = GenerateLabel();
            var elseLabel = GenerateLabel();
            var bodyLabel = GenerateLabel();

            if(condition.ElseBody != null)
            {
                Console.WriteLine(PrintExpression(condition.Condition, false, false, elseLabel, bodyLabel));
                EvalCondition(elseLabel);
            }
            else
            {
                Console.WriteLine(PrintExpression(condition.Condition, false, false, endLabel, bodyLabel));
                EvalCondition(endLabel);
            }

            Console.WriteLine($"{bodyLabel}:");

            foreach(var item in condition.IfBody)
            {
                if(item is ADContinue && forIncrement != null)
                {
                    Console.WriteLine(PrintExpression(forIncrement));
                }
                PrintNode(item, breakLabel, continueLabel, returnLabel);
            }

            Console.WriteLine($"jmp {endLabel}");

            if(condition.ElseBody != null)
            {
                Console.WriteLine($"{elseLabel}:");

                foreach(var item in condition.ElseBody)
                {
                    if (item is ADBreak)
                    {
                        PrintBreak(breakLabel);
                    }
                    else
                    {
                        PrintNode(item, breakLabel, continueLabel, returnLabel);
                    }
                }
            }

            Console.WriteLine($"{endLabel}:");
        }
    }
}
