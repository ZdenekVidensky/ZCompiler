using Parser.ADT.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.ADT.OperationNodes
{
    public class ADStatementExpression : IADNode, IADExpression
    {
        public IADExpression Expression { get; set; }
    }
}
