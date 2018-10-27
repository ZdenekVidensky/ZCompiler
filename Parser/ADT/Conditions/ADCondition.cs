using Parser.ADT.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.ADT.Conditions
{
    public class ADCondition : IADNode, IADCondition
    {
        public IADExpression Condition { get; set; }
        public IEnumerable<IADNode> IfBody { get; set; }
        public IEnumerable<IADNode> ElseBody { get; set; }
    }
}
