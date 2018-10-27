using Parser.ADT.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.ADT.Functions
{
    public class ADReturn : IADNode
    {
        public IADExpression Expression { get; set; }
    }
}
