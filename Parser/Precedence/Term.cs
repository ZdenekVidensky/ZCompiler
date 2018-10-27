using Parser.ADT.Interfaces;
using Parser.ADT.OperationNodes;
using Parser.SymbolTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Precedence
{
    public enum TermType
    {
        id, not, mod, binAnd, logAnd, mul, plus, inc, minus, dec, div,
        cElse, leftBr, rightBr, less, left, lessEq, asgn, eq, grt, grtEq, right,
        cCond, xor, binOr, logOr, compl, plusEq, minusEq, mulEq, divEq, modEq,
        rightEq, leftEq, andEq, xorEq, orEq, negEq, reference, dereference, end,
        leftABtype, error
    }
    public class Term
    {
        public TermType Type { get; set; }
        public STRecord STRecord { get; set; }
        public IADExpression Expression { get; set; }

        public Term()
        {
            STRecord = new STRecord();
        }
    }
}
