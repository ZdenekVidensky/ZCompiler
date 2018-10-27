using Parser.ADT.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.ADT.Loops
{
    public class ADForLoop : IADNode
    {
        public IADExpression DeclarationPart { get; set; }
        public IADExpression ConditionPart { get; set; }
        public IADExpression IncrementalPart { get; set; }
        public IEnumerable<IADNode> Body { get; set; }
    }
}
