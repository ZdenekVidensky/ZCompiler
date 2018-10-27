using Parser.ADT.Interfaces;
using Parser.ADT.Operands;
using Parser.ADT.OperationNodes;
using Parser.SymbolTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.ADT.Functions
{
    public class ADFunctionDeclaration : IADNode
    {
        public string Name { get; set; }
        public List<ADVariable> Arguments { get; set; }
        public IEnumerable<IADNode> Body { get; set; }
        public ADFunctionDeclaration()
        {
            Arguments = new List<ADVariable>();
        }
    }
}
