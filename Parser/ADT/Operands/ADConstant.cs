using Parser.ADT.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.ADT.Operands
{
    public class ADConstant : IADNode, IADOperand, IADExpression
    {
        public string Value { get; set; }
    }
}
