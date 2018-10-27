using Parser.ADT.Interfaces;
using Parser.ADT.Operands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.ADT.OperationNodes
{
    public class ADDeclaration : IADNode, IADDeclaration
    {
        public IEnumerable<ADVariable> Variables { get; set; }

        public ADDeclaration()
        {
            Variables = new List<ADVariable>();
        }
    }
}
