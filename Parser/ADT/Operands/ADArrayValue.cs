using Lex;
using Parser.ADT.Interfaces;
using Parser.Precedence;
using Parser.SymbolTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.ADT.Operands
{
    public class ADArrayValue : IADNode, IADOperand, IADExpression
    {
        public STRecord STRecord { get; set; }
        public List<IADExpression> Indexes { get; set; }

        public IADExpression ArrayExpression { get; set; }

        public ADArrayValue()
        {
            Indexes= new List<IADExpression>();
        }

        public void ComputeIndexes()
        {
            MainFSM.GetNextToken();
            var newIndex = PrecedenceSyntaxAnalysis.Precedence(TokenType.rightABType);
            MainFSM.GetNextToken();

            Indexes.Add(newIndex);

            if(MainFSM.PeekNextToken().Type == TokenType.leftABType)
            {
                ComputeIndexes();
            }
        }
    }
}
