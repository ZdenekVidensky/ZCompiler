using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public static class Errors
    {
        public static void LexError(string text)
        {
            Console.Error.WriteLine(text);
            Environment.Exit(1);
        }
    }
}
