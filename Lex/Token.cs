using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lex
{
    public enum TokenType
    {
        breakType,                  // break
        continueType,               // continue
        doType,                     // do
        elseType,                   // else
        forType,                    // for
        ifType,                     // if
        longType,                   // long
        returnType,                 // return
        sizeofType,                 // sizeof
        whileType,                  // while
        exrType,                    // !
        exrEqType,                  // !=
        procType,                   // %
        procEq,                     // %=
        ampType,                    // &
        andType,                    // &&
        ampEqType,                  // &=
        starType,                   // *
        multEqType,                 // *=
        plusType,                   // +
        incrType,                   // ++
        plusEqType,                 // +=
        minusType,                  // -
        decrType,                   // --
        minusEqType,                // -=
        divType,                    // /
        divEqType,                  // /=
        tElseType,                  // :
        tIfType,                    // ?
        lessType,                   // <
        leftType,                   // <<
        leftEqType,                 // <<=
        lessEqType,                 // <=
        asgnType,                   // =
        eqEqType,                   // ==
        grtType,                    // >
        grtEqType,                  // >=
        rightType,                  // >>
        rightEqType,                // >>=
        xorType,                    // ^
        xorEqType,                  // ^=
        bOrType,                    // |
        bOrEqType,                  // |=
        lOrType,                    // ||
        bNotType,                   // ~
        decNumberType,              // 123
        hexNumberType,              // 0xff
        octNumberType,              // 0176
        stringType,                 // abcd
        charType,                   // a
        idType,                     // a3bc
        comType,                    // ,
        semiType,                   // ;
        leftRBType,                 // (
        rightRBType,                // )
        leftCBType,                 // {
        rightCBType,                // }
        leftABType,                 // [
        rightABType,                // ]
        eofType,                    // EOF
        commentType,                // Comment
        constant,                   // Number in ADT
        pointer                     // Pointer
    }
    public class Token
    {
        public TokenType Type { get; set; }
        public string Attribute { get; set; }

        public Token(TokenType type, string attribute = "")
        {
            this.Type = type;
            this.Attribute = attribute;
        }
    }
}
