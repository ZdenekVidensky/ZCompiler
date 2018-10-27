using Parser.ADT.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.ADT
{
    public class ADTree : IADNode
    {
        public List<IADNode> Nodes { get; set; }

        public ADTree()
        {
            Nodes = new List<IADNode>();
        }
    }
}
