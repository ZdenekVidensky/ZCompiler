using Lex;
using Parser.ADT.Functions;
using Parser.ADT.Interfaces;
using Parser.ADT.Operands;
using Parser.ADT.OperationNodes;
using Parser.Precedence;
using Parser.SymbolTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public static partial class ParserFunctions
    {
        public static void decl_type(ADVariable variable)
        {
            if(MainFSM.PeekNextToken().Type == TokenType.leftABType)
            {
                MainFSM.GetNextToken();
                variable.Type = ADVariable.VarType.array;
                variable.STRecord.Type = STType.array;
                var arrayDimension = new ADArrayDimension() { ValuesCount = value() };
                variable.ArrayDimensions.Add(arrayDimension);

                if (MainFSM.GetNextToken().Type != TokenType.rightABType)
                {
                    SyntaxError("Byl ocekavan znak \']\'");
                }

                decl_type(variable);
            }
        }

        public static IADExpression value()
        {
            var token = MainFSM.PeekNextToken();

            if (token.Type == TokenType.idType)
            {
                if(MainFSM.PeekNextToken(2).Type == TokenType.leftRBType)
                {
                    return fce_call();
                }
                else
                {
                    var symbol = STSearch(MainFSM.PeekNextToken().Attribute);
                    if(symbol == null)
                    {
                        SemanticError($"Promenna {token.Attribute} nebyla deklarovana.");
                    }

                    return new ADVariable() { Name = symbol.Name, STRecord = symbol };
                }
            }
            else
            {
                if(token.Type == TokenType.decNumberType
                    || token.Type == TokenType.hexNumberType
                    || token.Type == TokenType.octNumberType)
                {
                    return PrecedenceSyntaxAnalysis.Precedence(TokenType.rightABType);
                }
            }

            return null;
        }
    }
}
