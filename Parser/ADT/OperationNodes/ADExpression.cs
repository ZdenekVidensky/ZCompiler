using Parser.ADT.Interfaces;
using Parser.Precedence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.ADT.OperationNodes
{
    public class ADExpression : IADNode, IADExpression
    {
        public IADExpression Left { get; set; }
        public TermType Operator { get; set; }
        public IADExpression Right { get; set; }
    }
}
