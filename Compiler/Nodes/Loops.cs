using Parser.ADT.Commands;
using Parser.ADT.Loops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public static partial class AssemblyCompiler
    {
        public static void PrintWhileLoop(ADWhileLoop whileLoop, string returnLabel)
        {
            var conditionLabel = GenerateLabel();
            var endLabel = GenerateLabel();

            Console.WriteLine($"{conditionLabel}:");

            Console.WriteLine("# podminkova cast");
            Console.WriteLine(PrintExpression(whileLoop.Condition));


            Console.WriteLine("# Vyhodnoceni podminky");
            EvalCondition(endLabel);

            Console.WriteLine("# telo cyklu");
            foreach (var item in whileLoop.Body)
            {
                PrintNode(item, endLabel, conditionLabel, returnLabel);
            }
           
            Console.WriteLine($"jmp {conditionLabel}");
            Console.WriteLine($"{endLabel}:");
        }

        public static void PrintDoWhileLoop(ADDoWhileLoop doWhileLoop, string returnLabel)
        {
            var bodyLabel = GenerateLabel();
            var endLabel = GenerateLabel();
            var conditionLabel = GenerateLabel();

            Console.WriteLine($"{bodyLabel}:");
            foreach(var item in doWhileLoop.Body)
            {
                PrintNode(item, endLabel, conditionLabel, returnLabel);
            }

            Console.WriteLine("#Podminka cyklu do-while");
            Console.WriteLine($"{conditionLabel}:");
            Console.WriteLine(PrintExpression(doWhileLoop.Condition));
            EvalCondition(endLabel);

            Console.WriteLine($"jmp {bodyLabel}");
            Console.WriteLine($"{endLabel}:");
        }

        public static void PrintForLoop(ADForLoop forLoop, string returnLabel)
        {
            if(forLoop.DeclarationPart != null)
            {
                Console.WriteLine("# deklaracni cast");
                Console.WriteLine(PrintExpression(forLoop.DeclarationPart));
            }
            
            var condLabel = GenerateLabel();
            var endLabel = GenerateLabel();

            Console.WriteLine($"{condLabel}:");

            if(forLoop.ConditionPart != null)
            {
                Console.WriteLine("# podminkova cast");
                Console.WriteLine(PrintExpression(forLoop.ConditionPart));


                Console.WriteLine("# Vyhodnoceni podminky");
                EvalCondition(endLabel);
            }

            Console.WriteLine("# telo cyklu for");
            foreach(var item in forLoop.Body)
            {
                if(item is ADContinue)
                {
                    Console.WriteLine(PrintExpression(forLoop.IncrementalPart));
                }

                PrintNode(item, endLabel, condLabel, returnLabel, forLoop.IncrementalPart);
            }

            if(forLoop.IncrementalPart != null)
            {
                Console.WriteLine("# inkrementacni cast");
                Console.WriteLine(PrintExpression(forLoop.IncrementalPart));
            }

            Console.WriteLine($"jmp {condLabel}");

            Console.WriteLine($"{endLabel}:");
        }
    }
}
