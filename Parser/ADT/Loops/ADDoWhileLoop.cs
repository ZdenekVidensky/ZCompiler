using Parser.ADT.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.ADT.Loops
{
    public class ADDoWhileLoop : IADNode
    {
        public IADExpression Condition { get; set; }
        public IEnumerable<IADNode> Body { get; set; }
    }
}
