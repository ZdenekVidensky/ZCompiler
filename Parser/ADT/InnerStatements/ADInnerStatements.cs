using Parser.ADT.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.ADT.InnerStatements
{
    public class ADInnerStatements : IADNode
    {
        public List<IADNode> Statements { get; set; }

        public ADInnerStatements()
        {
            Statements = new List<IADNode>();
        }
    }
}
