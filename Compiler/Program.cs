using Lex;
using Lex.FiniteStateMachines;
using Parser;
using Parser.ADT;
using Parser.Precedence;
using SourceCodeImport;
using SyntaxTreeVisualizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    /*
    * Exit codes:
    *   1 -> Lex error
    *   2 -> Syntax error
    *   3 -> Semantic error
    */
    class Program
    {
        static void Main(string[] args)
        {
            PrecedenceSyntaxAnalysis.LoadPrecTable();

            string input = string.Empty;
            string sourceCode = string.Empty;

            while((input = Console.ReadLine()) != null)
            {
                sourceCode += input + Environment.NewLine;
            }

            MainFSM.SourceCode = sourceCode;
            MainFSM.Index = 0;
            MainFSM.RowCounter = 0;

            var adTree = new ADTree();
            ParserFunctions.prog(adTree);

            AssemblyCompiler.Compile(adTree);
        }
    }
}
