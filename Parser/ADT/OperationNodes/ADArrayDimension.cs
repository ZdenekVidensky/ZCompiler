using Parser.ADT.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.ADT.OperationNodes
{
    public class ADArrayDimension
    {
        public List<IADExpression> Values { get; set; }
        public IADExpression ValuesCount { get; set; }

        public ADArrayDimension()
        {
            Values = new List<IADExpression>();
        }
    }
}
