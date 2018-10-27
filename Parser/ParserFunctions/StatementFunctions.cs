using Lex;
using Parser.ADT;
using Parser.ADT.Functions;
using Parser.ADT.Interfaces;
using Parser.ADT.Operands;
using Parser.ADT.OperationNodes;
using Parser.ADT.Variables;
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
        public static IEnumerable<IADNode> st_main_list()
        {
            while (MainFSM.PeekNextToken().Type != TokenType.eofType)
            {
                yield return st_main();
            }
        }

        public static IADNode st_main()
        {
            var token = MainFSM.GetNextToken();

            if (token.Type != TokenType.idType)
            {
                SyntaxError("Byl ocekavan identifikator funkce.");
            }

            if (STSearch(token.Attribute, true) != null)
            {
                SemanticError($"Funkce nebo globalni promenna se jmenem {token.Attribute} je jiz deklarovana.");
            }

            ADFunctionDeclaration fceDeclaration = new ADFunctionDeclaration() { Name = token.Attribute };

            var newRecord = new STRecord()
            {
                Access = STAccess.global,
                Name = token.Attribute,
                Type = STType.function,
                Function = fceDeclaration
            };

            FunctionsST.Records.Add(newRecord);

            if (MainFSM.GetNextToken().Type != TokenType.leftRBType)
            {
                SyntaxError("Byl ocekavan znak \'(\'");
            }

            var arguments = decl_arg();

            var symbolTable = new STable();

            foreach (var arg in arguments)
            {
                var stRecord = new STRecord()
                {
                    Access = STAccess.local,
                    Name = arg,
                    Type = STType.variable
                };

                symbolTable.Records.Add(stRecord);
                fceDeclaration.Arguments.Add(new ADVariable { Name = arg, Type = ADVariable.VarType.variable, STRecord = stRecord});
            }

            STablesStack.Push(symbolTable);

            if (MainFSM.GetNextToken().Type != TokenType.leftCBType)
            {
                SyntaxError("Byl ocekavan znak \'{\'");
            }

            fceDeclaration.Body = statement_list(true);

            if (MainFSM.GetNextToken().Type != TokenType.rightCBType)
            {
                SyntaxError("Byl ocekavan znak \'}\'");
            }

            STablesStack.Pop();
            return fceDeclaration;
        }

        public static IEnumerable<string> decl_arg()
        {
            Token token = MainFSM.GetNextToken();

            while (token.Type != TokenType.rightRBType)
            {
                if (token.Type == TokenType.idType)
                {
                    yield return token.Attribute;
                }
                else if (token.Type == TokenType.comType)
                {
                    yield return next_decl_arg();
                }

                token = MainFSM.GetNextToken();
            }
        }

        public static string next_decl_arg()
        {
            var token = MainFSM.GetNextToken();
            if (token.Type != TokenType.idType)
            {
                SyntaxError("Byl ocekavan nazev argumentu");
            }

            return token.Attribute;
        }

        public static IADNode statement()
        {
            var nextToken = MainFSM.PeekNextToken();
            IADNode result = null;

            switch (nextToken.Type)
            {
                case TokenType.longType:
                    var list = new ADVariableDeclarations();
                    StatementHelpers.VariableDeclaration(ref list);

                    list.Variables.Reverse();
                    result = list;
                    break;

                case TokenType.starType:
                    result = new ADStatementExpression { Expression = PrecedenceSyntaxAnalysis.Precedence(TokenType.semiType) };

                    if (MainFSM.GetNextToken().Type != TokenType.semiType)
                    {
                        SyntaxError("Byl ocekavan znak \';\'");
                    }

                    break;

                case TokenType.idType:
                case TokenType.incrType:
                case TokenType.decrType:
                    if(MainFSM.PeekNextToken(2).Type == TokenType.leftRBType)
                    {
                        var fceToken = MainFSM.GetNextToken();

                        result = fce_call(fceToken.Attribute);

                        if(MainFSM.GetNextToken().Type != TokenType.semiType)
                        {
                            SyntaxError("Byl ocekavan znak \';\'");
                        }
                    }
                    else
                    {
                        result = new ADStatementExpression { Expression = PrecedenceSyntaxAnalysis.Precedence(TokenType.semiType) };

                        if (MainFSM.GetNextToken().Type != TokenType.semiType)
                        {
                            SyntaxError("Byl ocekavan znak \';\'");
                        }
                    }
                    break;

                case TokenType.ifType:
                    result = StatementHelpers.Condition();
                    break;

                case TokenType.forType:
                    result = StatementHelpers.ForLoop();
                    break;

                case TokenType.whileType:
                    result = StatementHelpers.WhileLoop();
                    break;

                case TokenType.doType:
                    result = StatementHelpers.DoWhileLoop();
                    break;

                case TokenType.returnType:
                    result = StatementHelpers.Return();
                    break;

                case TokenType.breakType:
                    result = StatementHelpers.Break();
                    break;

                case TokenType.continueType:
                    result = StatementHelpers.Continue();
                    break;
                case TokenType.leftCBType:
                    result = StatementHelpers.InnerStatements();
                    break;

                default:
                    SyntaxError("Byl ocekavan jeden z nasledujici klicovych slov nebo znaku: long, *, identifikator, if, for, while, do, return, break, continue");
                    break;
            }
            return result;
        }

        public static List<IADNode> statement_list(bool fceDeclaration = false)
        {
            var result = new List<IADNode>();

            if (fceDeclaration)
            {
                PrecedenceSyntaxAnalysis.AnonymousDeclarations = result;
            }

            while (MainFSM.PeekNextToken().Type != TokenType.rightCBType)
            {
                if(MainFSM.PeekNextToken().Type == TokenType.eofType)
                {
                    SyntaxError("Byl ocekavan znak \'}\'");
                }

                result.Add(statement());
            }

            return result;
        }

        public static ADFunctionCall fce_call(string fceName = "")
        {

            STRecord fceRecord = null;
            var result = new ADFunctionCall();

            fceRecord = STSearch(fceName);

            if (MainFSM.GetNextToken().Type != TokenType.leftRBType)
            {
                SyntaxError("Byl ocekavan znak \'(\' pri volani funkce.");
            }

            var argumentsList = new List<IADExpression>();
            while (MainFSM.PeekNextToken().Type != TokenType.rightRBType)
            {
                argumentsList.Add(PrecedenceSyntaxAnalysis.Precedence(TokenType.rightRBType));

                if (MainFSM.PeekNextToken().Type == TokenType.comType)
                {
                    MainFSM.GetNextToken();
                }
            }

            result.Arguments = argumentsList;
            result.STRecord = fceRecord;


            if (MainFSM.GetNextToken().Type != TokenType.rightRBType)
            {
                SyntaxError("Byl ocekavan znak \')\' pri volani funkce.");
            }

            return result;
        }
    }
}
