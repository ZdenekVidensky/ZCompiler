using Parser.ADT.Interfaces;
using Parser.ADT.Operands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.ADT.Variables
{
    public class ADVariableDeclarations : IADNode
    {
        public List<ADVariable> Variables { get; set; }

        public ADVariableDeclarations()
        {
            Variables = new List<ADVariable>();
        }
    }
}
