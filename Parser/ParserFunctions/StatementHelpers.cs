using Lex;
using Parser.ADT.Commands;
using Parser.ADT.Conditions;
using Parser.ADT.Functions;
using Parser.ADT.InnerStatements;
using Parser.ADT.Interfaces;
using Parser.ADT.Loops;
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
    public static class StatementHelpers
    {
        public static void VariableDeclaration(ref ADVariableDeclarations list, bool recursive = false)
        {
            if (!recursive)
            {
                if (MainFSM.GetNextToken().Type != TokenType.longType)
                {
                    ParserFunctions.SyntaxError("Bylo ocekavano klicove slovo \'long\'");
                }
            }

            var token = MainFSM.GetNextToken();

            if(token.Type != TokenType.idType)
            {
                ParserFunctions.SyntaxError("Byl ocekavan identifikator");
            }

            if(ParserFunctions.STSearch(token.Attribute, true) != null)
            {
                ParserFunctions.SemanticError($"Promenna {token.Attribute} byla jiz deklarovana");
            }

            var newRecord = new STRecord()
            {
                Name = token.Attribute,
                Access = STAccess.local
            };
            ParserFunctions.STablesStack.Peek().Records.Add(newRecord);

            var variableDeclaration = new ADVariable()
            {
                Name = token.Attribute,
                STRecord = newRecord,
                Type = ADVariable.VarType.variable
            };

            ParserFunctions.decl_type(variableDeclaration);


            if(MainFSM.PeekNextToken().Type == TokenType.comType)
            {
                MainFSM.GetNextToken();
                VariableDeclaration(ref list, true);
            }

            if(MainFSM.PeekNextToken().Type == TokenType.semiType)
            {
                list.Variables.Add(variableDeclaration);

                if (!recursive)
                {
                    MainFSM.GetNextToken(); 
                }
                return;
            }

            if (MainFSM.GetNextToken().Type != TokenType.asgnType)
            {
                ParserFunctions.SyntaxError("Byl ocekavan znak \';\' nebo \'=\'");
            }

            if(MainFSM.PeekNextToken().Type == TokenType.leftCBType)
            {
                variableDeclaration.STRecord.Type = STType.array;

                if(variableDeclaration.ArrayDimensions.Count == 0)
                {
                    variableDeclaration.ArrayDimensions.Add(new ADArrayDimension());
                }
                ArrayValuesDeclaration(variableDeclaration, 0);
            }
            else
            {
                variableDeclaration.STRecord.Type = STType.variable;

                variableDeclaration.Value = PrecedenceSyntaxAnalysis.Precedence(TokenType.semiType, true);
            }

            if(MainFSM.PeekNextToken().Type == TokenType.comType)
            {
                MainFSM.GetNextToken();
                VariableDeclaration(ref list, true);
            }

            if (!recursive)
            {
                if (MainFSM.GetNextToken().Type != TokenType.semiType)
                {
                    ParserFunctions.SyntaxError("Byl ocekavan znak \';\'");
                }
            }

            list.Variables.Add(variableDeclaration);
        }

        public static void ArrayValuesDeclaration(ADVariable array, int dimensionIndex)
        {
            if(array.ArrayDimensions.Count > 0 && array.ArrayDimensions.Count < (dimensionIndex + 1))
            {
                ParserFunctions.SemanticError("Dane pole nema tolik dimenzi.");
            }

            array.Type = ADVariable.VarType.array;

            if(MainFSM.GetNextToken().Type == TokenType.leftCBType)
            {
                while(MainFSM.PeekNextToken().Type != TokenType.rightCBType)
                {
                    array.ArrayDimensions[dimensionIndex].Values.Add
                        (
                            PrecedenceSyntaxAnalysis.Precedence(TokenType.rightCBType)
                        );

                    if(MainFSM.PeekNextToken().Type == TokenType.comType)
                    {
                        MainFSM.GetNextToken();
                    }
                }

                if(MainFSM.GetNextToken().Type != TokenType.rightCBType)
                {
                    ParserFunctions.SyntaxError("Byl ocekavan znak \'}\'");
                }

                if (MainFSM.PeekNextToken().Type == TokenType.comType)
                {
                    ArrayValuesDeclaration(array, dimensionIndex++);
                }
            }
            else
            {
                ParserFunctions.SyntaxError("Byl ocekavan znak \'{\'");
            }
        }

        public static ADVariableAssignment IDAssign(bool dereference)
        {
            var token = MainFSM.GetNextToken();

            if(token.Type != TokenType.idType)
            {
                ParserFunctions.SyntaxError("Byla ocekavana promenna.");
            }

            var record = ParserFunctions.STSearch(token.Attribute);

            if(record == null)
            {
                ParserFunctions.SemanticError($"Promenna {token.Attribute} nebyla deklarovana");
            }

            var asgnOperator = MainFSM.GetNextToken();

            if (!IsAssignOperator(asgnOperator.Type))
            {
                ParserFunctions.SyntaxError("Byl ocekavan prirazovaci operator");
            }

            var assign = new ADVariableAssignment() { Dereference = dereference, STRecord = record, AssignOperator = asgnOperator.Type };

            bool nextDereference = false;

            if(MainFSM.PeekNextToken().Type == TokenType.starType)
            { 
                nextDereference = true;
                MainFSM.GetNextToken();
            }

            if(MainFSM.PeekNextToken().Type == TokenType.idType)
            {
                if (IsAssignOperator(MainFSM.PeekNextToken(2).Type))
                {
                    assign.Expression = IDAssign(nextDereference);
                }
                else
                {
                    assign.Expression = PrecedenceSyntaxAnalysis.Precedence(TokenType.semiType);
                }
            }
            else
            {
                assign.Expression = PrecedenceSyntaxAnalysis.Precedence(TokenType.semiType);
            }

            return assign;
        }

        public static IADNode Condition()
        {
            MainFSM.GetNextToken();
            if(MainFSM.GetNextToken().Type != TokenType.leftRBType)
            {
                ParserFunctions.SyntaxError("Byl ocekavan znak \'(\'");
            }

            var result = new ADCondition();
            result.Condition = PrecedenceSyntaxAnalysis.Precedence(TokenType.rightRBType);
            MainFSM.GetNextToken();

            if (MainFSM.GetNextToken().Type != TokenType.leftCBType)
            {
                ParserFunctions.SyntaxError("Byl ocekavan znak \'{\'");
            }

            ParserFunctions.STablesStack.Push(new STable());

            result.IfBody = ParserFunctions.statement_list();

            if (MainFSM.GetNextToken().Type != TokenType.rightCBType)
            {
                ParserFunctions.SyntaxError("Byl ocekavan znak \'}\'");
            }

            ParserFunctions.STablesStack.Pop();

            if(MainFSM.PeekNextToken().Type != TokenType.elseType)
            {
                return result;
            }
            else
            {
                MainFSM.GetNextToken();
                if (MainFSM.GetNextToken().Type != TokenType.leftCBType)
                {
                    ParserFunctions.SyntaxError("Byl ocekavan znak \'{\'");
                }

                ParserFunctions.STablesStack.Push(new STable());

                result.ElseBody = ParserFunctions.statement_list();

                if (MainFSM.GetNextToken().Type != TokenType.rightCBType)
                {
                    ParserFunctions.SyntaxError("Byl ocekavan znak \'}\'");
                }

                ParserFunctions.STablesStack.Pop();

                return result;
            }
        }

        public static IADNode ForLoop()
        {
            MainFSM.GetNextToken();

            if(MainFSM.GetNextToken().Type != TokenType.leftRBType)
            {
                ParserFunctions.SyntaxError("Byl ocekavan znak \'(\'");
            }

            var result = new ADForLoop();
            if(MainFSM.PeekNextToken().Type != TokenType.semiType)
            {
                result.DeclarationPart = PrecedenceSyntaxAnalysis.Precedence(TokenType.semiType);
            }

            if (MainFSM.GetNextToken().Type != TokenType.semiType)
            {
                ParserFunctions.SyntaxError("Byl ocekavan znak \';\'");
            }

            if (MainFSM.PeekNextToken().Type != TokenType.semiType)
            {
                result.ConditionPart = PrecedenceSyntaxAnalysis.Precedence(TokenType.semiType);
            }

            if (MainFSM.GetNextToken().Type != TokenType.semiType)
            {
                ParserFunctions.SyntaxError("Byl ocekavan znak \';\'");
            }

            if (MainFSM.PeekNextToken().Type != TokenType.rightRBType)
            {
                result.IncrementalPart = PrecedenceSyntaxAnalysis.Precedence(TokenType.rightRBType);
            }

            if (MainFSM.GetNextToken().Type != TokenType.rightRBType)
            {
                ParserFunctions.SyntaxError("Byl ocekavan znak \')\'");
            }

            if (MainFSM.GetNextToken().Type != TokenType.leftCBType)
            {
                ParserFunctions.SyntaxError("Byl ocekavan znak \'{\'");
            }

            ParserFunctions.STablesStack.Push(new STable());

            result.Body = ParserFunctions.statement_list();

            if (MainFSM.GetNextToken().Type != TokenType.rightCBType)
            {
                ParserFunctions.SyntaxError("Byl ocekavan znak \'}\'");
            }

            ParserFunctions.STablesStack.Pop();

            return result;
        }

        public static IADNode WhileLoop()
        {
            MainFSM.GetNextToken();

            if(MainFSM.GetNextToken().Type != TokenType.leftRBType)
            {
                ParserFunctions.SyntaxError("Byl ocekavan znak \'(\'");
            }

            var result = new ADWhileLoop();
            result.Condition = PrecedenceSyntaxAnalysis.Precedence(TokenType.rightRBType);

            if (MainFSM.GetNextToken().Type != TokenType.rightRBType)
            {
                ParserFunctions.SyntaxError("Byl ocekavan znak \')\'");
            }

            if (MainFSM.GetNextToken().Type != TokenType.leftCBType)
            {
                ParserFunctions.SyntaxError("Byl ocekavan znak \'{\'");
            }

            ParserFunctions.STablesStack.Push(new STable());
            result.Body = ParserFunctions.statement_list();

            if (MainFSM.GetNextToken().Type != TokenType.rightCBType)
            {
                ParserFunctions.SyntaxError("Byl ocekavan znak \'}\'");
            }

            ParserFunctions.STablesStack.Pop();

            return result;
        }
        public static IADNode DoWhileLoop()
        {
            MainFSM.GetNextToken();

            var result = new ADDoWhileLoop();
            if (MainFSM.GetNextToken().Type != TokenType.leftCBType)
            {
                ParserFunctions.SyntaxError("Byl ocekavan znak \'{\'");
            }

            ParserFunctions.STablesStack.Push(new STable());
            result.Body = ParserFunctions.statement_list();

            if (MainFSM.GetNextToken().Type != TokenType.rightCBType)
            {
                ParserFunctions.SyntaxError("Byl ocekavan znak \'}\'");
            }

            ParserFunctions.STablesStack.Pop();

            if(MainFSM.GetNextToken().Type != TokenType.whileType)
            {
                ParserFunctions.SyntaxError("Bylo ocekavano klicove slovo while");
            }

            if(MainFSM.GetNextToken().Type != TokenType.leftRBType)
            {
                ParserFunctions.SyntaxError("Byl ocekavan znak \'(\'");
            }

            result.Condition = PrecedenceSyntaxAnalysis.Precedence(TokenType.rightRBType);

            if (MainFSM.GetNextToken().Type != TokenType.rightRBType)
            {
                ParserFunctions.SyntaxError("Byl ocekavan znak \')\'");
            }

            if (MainFSM.GetNextToken().Type != TokenType.semiType)
            {
                ParserFunctions.SyntaxError("Byl ocekavan znak \';\'");
            }

            return result;
        }

        public static IADNode Return()
        {
            if(MainFSM.GetNextToken().Type != TokenType.returnType)
            {
                ParserFunctions.SyntaxError("Bylo ocekavano klicove slovo \'return\'");
            }

            var result = PrecedenceSyntaxAnalysis.Precedence(TokenType.semiType);

            if(MainFSM.GetNextToken().Type != TokenType.semiType)
            {
                ParserFunctions.SyntaxError("Byl ocekavan znak \';\'");
            }

            return new ADReturn { Expression = result } ;
        }

        public static IADNode InnerStatements()
        {
            MainFSM.GetNextToken();
            ParserFunctions.STablesStack.Push(new STable());

            var result = ParserFunctions.statement_list();

            if(MainFSM.GetNextToken().Type != TokenType.rightCBType)
            {
                ParserFunctions.SyntaxError("Byl ocekavan znak \'}\'");
            }

            ParserFunctions.STablesStack.Pop();

            if(MainFSM.PeekNextToken().Type == TokenType.semiType)
            {
                MainFSM.GetNextToken();
            }

            return new ADInnerStatements() { Statements = result };
        }

        public static IADNode Break()
        {
            if(MainFSM.GetNextToken().Type != TokenType.breakType)
            {
                ParserFunctions.SyntaxError("Bylo ocekavano slovo \'break\'");
            }

            if(MainFSM.GetNextToken().Type != TokenType.semiType)
            {
                ParserFunctions.SyntaxError("Byl ocekavan znak \';\'");
            }
            return new ADBreak();
        }
    
        public static IADNode Continue()
        {
            if (MainFSM.GetNextToken().Type != TokenType.continueType)
            {
                ParserFunctions.SyntaxError("Bylo ocekavano slovo \'continue\'");
            }

            if (MainFSM.GetNextToken().Type != TokenType.semiType)
            {
                ParserFunctions.SyntaxError("Byl ocekavan znak \';\'");
            }

            return new ADContinue();
        }

        public static bool IsAssignOperator(TokenType type)
        {
            if(type == TokenType.asgnType || type == TokenType.multEqType || type == TokenType.plusEqType || type == TokenType.minusEqType
                || type == TokenType.divEqType || type == TokenType.bOrEqType || type == TokenType.procEq || type == TokenType.ampEqType
                || type == TokenType.xorEqType || type == TokenType.leftEqType || type == TokenType.rightEqType)
            {
                return true;
            }

            return false;
        }
    }
}
