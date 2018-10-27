using Lex;
using Parser.ADT.Interfaces;
using Parser.SymbolTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.ADT.Variables
{
    public class ADVariableAssignment : IADNode, IADExpression
    {
        public bool Dereference { get; set; }
        public STRecord STRecord { get; set; }
        public TokenType AssignOperator { get; set; }
        public IADExpression Expression { get; set; }
    }
}
