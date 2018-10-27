using Parser.ADT.Interfaces;
using Parser.ADT.OperationNodes;
using Parser.SymbolTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.ADT.Operands
{
    public class ADVariable : IADNode, IADVariable, IADOperand, IADExpression
    {
        public enum VarType
        {
            array, variable, pointer, function
        }
        public string Name { get; set; }
        public STRecord STRecord { get; set; }
        public IADExpression Value { get; set; }
        public List<ADArrayDimension> ArrayDimensions { get; set; }
        public VarType Type { get; set; }

        public ADVariable()
        {
            ArrayDimensions = new List<ADArrayDimension>();
        }
    }
}
