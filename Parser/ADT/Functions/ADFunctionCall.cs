using Parser.ADT.Interfaces;
using Parser.ADT.OperationNodes;
using Parser.SymbolTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.ADT.Functions
{
    public class ADFunctionCall : IADNode, IADExpression
    {
        public STRecord STRecord { get; set; }
        public List<IADExpression> Arguments { get; set; }
        public IADExpression Expression { get; set; }

        public ADFunctionCall()
        {
            Arguments = new List<IADExpression>();
        }
    }
}
