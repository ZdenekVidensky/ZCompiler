using Parser.ADT.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parser.ADT.Interfaces;
using Parser.ADT.Operands;

namespace Parser.SymbolTable
{
    public enum STType
    {
        variable, array, function, constant, str
    }
    public enum STAccess
    {
        global, local
    }
    public class STRecord
    {
        public string Name { get; set; }
        public STType Type { get; set; }
        public STAccess Access { get; set; }
        public string Address { get; set; }
        public string Value { get; set; }
        public ADFunctionDeclaration Function { get; set; }
    }
}
