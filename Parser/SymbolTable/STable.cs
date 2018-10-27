using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.SymbolTable
{
    public class STable
    {
        public List<STRecord> Records { get; set; }

        public STable()
        {
            Records = new List<STRecord>();
        }
    }
}
