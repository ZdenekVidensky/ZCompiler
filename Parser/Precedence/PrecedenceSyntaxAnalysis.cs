using Lex;
using Parser.ADT.Functions;
using Parser.ADT.Interfaces;
using Parser.ADT.Operands;
using Parser.ADT.OperationNodes;
using Parser.ADT.Variables;
using Parser.SymbolTable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Precedence
{
    // https://en.wikipedia.org/wiki/Operator-precedence_grammar
    public static class PrecedenceSyntaxAnalysis
    {
        private const int G = 0; // Greater >
        private const int L = 1; // Less <
        private const int E = 2; // Equal =
        private const int B = 3; // Blank
        private const int S = 4; // Smile

        private static int[,] precTable;

        public static IEnumerable<IADNode> AnonymousDeclarations { get; set; }

        public static IADExpression Precedence(TokenType endType, bool decl = false)
        {
            Stack<Term> SS = new Stack<Term>(); // Sign stack
            Stack<Term> ST = new Stack<Term>(); // Term stack

            int numOfLeftB = 0;
            int numOfRightB = 0;

            bool mustBeId = true;
            Term actualTerm = null;

            SS.Push(new Term() { Type = TermType.end });

            while (true)
            {
                if(actualTerm == null || actualTerm.Type != TermType.end)
                {
                    actualTerm = GetNextTerm(endType, numOfLeftB, numOfRightB, ref SS, decl);
                }

                var stackSym = SS.Peek();

                if (actualTerm.Type == TermType.leftBr)
                {
                    numOfLeftB++;
                }

                if(actualTerm.Type == TermType.rightBr)
                {
                    numOfRightB++;
                }

                if((numOfRightB > numOfLeftB) && endType == TokenType.semiType)
                {
                    ParserFunctions.SyntaxError("Neocekavany konec vyrazu.");
                }

                if (actualTerm.Type == TermType.leftABtype)
                {
                    if (ST.Count == 0)
                    {
                        ParserFunctions.SyntaxError("Vyraz nemuze zacinat znakem \'[\'");
                    }

                    var arrayTerm = ST.Pop();
                    var record = arrayTerm.STRecord;

                    MainFSM.Index--;

                    var arrayValue = new ADArrayValue() { STRecord = record };

                    arrayValue.ComputeIndexes();

                    if(arrayTerm.Expression != null)
                    {
                        arrayValue.ArrayExpression = arrayTerm.Expression;
                    }

                    arrayTerm.Expression = arrayValue;

                    actualTerm = GetNextTerm(endType, numOfLeftB, numOfRightB, ref SS, decl);

                    if(actualTerm.Type == TermType.leftBr)
                    {
                        MainFSM.Index--;
                        var fceCall = ParserFunctions.fce_call();
                        fceCall.Expression = arrayValue;

                        ST.Push(new Term() { Expression = fceCall, Type = TermType.id });

                        actualTerm = GetNextTerm(endType, numOfLeftB, numOfRightB, ref SS, decl);
                    }
                    else
                    {
                        ST.Push(arrayTerm);
                    }
                }

                if (mustBeId && actualTerm.Type == TermType.binAnd)
                {
                    actualTerm.Type = TermType.reference;
                }
                else if(mustBeId && actualTerm.Type == TermType.mul)
                {
                    actualTerm.Type = TermType.dereference;
                }

                if(actualTerm.Type == TermType.inc || actualTerm.Type == TermType.dec)
                {
                    var expr = new ADExpression();
                    expr.Operator = actualTerm.Type;

                    if (mustBeId)
                    {
                        actualTerm = GetNextTerm(endType, numOfLeftB, numOfRightB, ref SS, decl);

                        if (actualTerm.Type != TermType.id)
                        {
                            ParserFunctions.SyntaxError("Byl ocekavan identifikator.");
                        }

                        expr.Right = GetExpression(actualTerm.STRecord);

                        mustBeId = false;            
                    }
                    else
                    {
                        actualTerm = ST.Pop();
                        expr.Left = GetExpression(actualTerm.STRecord);
                    }

                    var newTerm = new Term { Expression = expr };
                    ST.Push(newTerm);
                    continue;
                }

                switch(precTable[(int)stackSym.Type, (int)actualTerm.Type])
                {
                    case L: 
                        if(actualTerm.Type == TermType.id)
                        {
                            if (mustBeId)
                            {
                                ST.Push(actualTerm);
                                mustBeId = false;
                            }
                            else
                            {
                                ParserFunctions.SyntaxError("Byl ocekavan operator.");
                            }
                        }
                        else if((actualTerm.Type != TermType.id) && (actualTerm.Type != TermType.end))
                        {
                            if(!mustBeId || actualTerm.Type == TermType.leftBr 
                                || actualTerm.Type == TermType.rightBr)
                            {
                                SS.Push(actualTerm);
                                mustBeId = true;
                            }
                            else if(mustBeId && (actualTerm.Type == TermType.not || actualTerm.Type == TermType.minus
                                || actualTerm.Type == TermType.plus || actualTerm.Type == TermType.compl ||
                                actualTerm.Type == TermType.reference || actualTerm.Type == TermType.dereference))
                            {
                                SS.Push(actualTerm);
                            }
                            else
                            {
                                ParserFunctions.SyntaxError("Byla ocekavana hodnota typu long.");
                            }
                        }
                        break;

                    case G:
                        if(actualTerm.Type == TermType.rightBr)
                        {
                            while(stackSym.Type != TermType.leftBr)
                            {
                                Reduce(ref SS, ref ST);
                                stackSym = SS.Peek();
                            }
                            SS.Pop();
                        }
                        else
                        {
                            Reduce(ref SS, ref ST);
                            if(actualTerm.Type != TermType.end)
                            {
                                SS.Push(actualTerm);
                                mustBeId = true;
                            }
                        }
                        break;

                    case E:
                        SS.Pop();
                        break;
                    case B:
                        ParserFunctions.SyntaxError("Chyba ve vyrazu.");
                        break;
                    case S:
                        break;
                }

                if (actualTerm.Type == TermType.end && stackSym.Type == TermType.end)
                {
                    break;
                }           
            }

            MainFSM.Index--;

            if (ST.Count > 0)
            {
                var result = ST.Pop();
                if (result.Expression == null)
                {
                    if (result.STRecord.Type == STType.constant)
                    {
                        result.Expression = new ADConstant { Value = result.STRecord.Value };
                    }
                    else if (result.STRecord.Type == STType.variable || result.STRecord.Type == STType.array || result.STRecord.Type == STType.str)
                    {
                        result.Expression = new ADVariable
                        {
                            Name = result.STRecord.Name,
                            Type = ADVariable.VarType.variable,
                            STRecord = result.STRecord
                        };
                    }
                    else if (result.STRecord.Type == STType.function)
                    {

                        result.Expression = ParserFunctions.fce_call();
                    }
                }
                return result.Expression;
            }

            return null;         
        }

        public static void Reduce(ref Stack<Term> SS, ref Stack<Term> ST)
        {
            var rightOperand = ST.Pop();
            var operatorTerm = SS.Pop();
            Term leftOperand = null;

            if(ST.Count > 0 && (operatorTerm.Type != TermType.dec && operatorTerm.Type != TermType.inc 
                && operatorTerm.Type != TermType.compl && operatorTerm.Type != TermType.reference
                && operatorTerm.Type != TermType.dereference && operatorTerm.Type != TermType.not))
            {
                leftOperand = ST.Pop();
            }

            ADExpression expression = new ADExpression();

            if (rightOperand.Expression != null)
            {
                expression.Right = rightOperand.Expression;
            }
            else
            {
                expression.Right = GetExpression(rightOperand.STRecord);
            }

            expression.Operator = operatorTerm.Type;


            if(leftOperand != null)
            {
                if (leftOperand.Expression != null)
                {
                    expression.Left = leftOperand.Expression;
                }
                else
                {
                    expression.Left = GetExpression(leftOperand.STRecord);
                }
            }

            if(expression.Operator == TermType.asgn)
            {
                if(leftOperand.STRecord.Type != STType.variable && leftOperand.STRecord.Type != STType.array)
                {
                    ParserFunctions.SyntaxError("Na leve strane operace prirazeni musi byt promenna.");
                }
            }

            ST.Push(new Term() { Type = TermType.id, Expression = expression });
        }

        private static IADExpression GetExpression(STRecord stRecord)
        {
            IADExpression result = null;

            if (stRecord.Type == STType.constant)
            {
                result = new ADConstant { Value = stRecord.Value };
            }
            else if (stRecord.Type == STType.variable || stRecord.Type == STType.array)
            {
                result = new ADVariable { Name = stRecord.Name, Type = ADVariable.VarType.variable, STRecord = stRecord };
            }
            else if(stRecord.Type == STType.function)
            {
                MainFSM.Index--;
                result = ParserFunctions.fce_call();
            }

            return result;
        }

        private static Term GetNextTerm(TokenType endType, int numOfLeftB, int numOfRightB, ref Stack<Term> SS, bool decl = false)
        {
            var token = MainFSM.GetNextToken();
            TermType type = TermType.error;
            Term newTerm = new Term();

            #region Term type by token
            switch (token.Type)
            {
                case TokenType.exrType:
                    type = TermType.not;
                    break;
                case TokenType.decNumberType:
                case TokenType.hexNumberType:
                case TokenType.octNumberType:
                case TokenType.charType:
                case TokenType.idType:
                case TokenType.longType:
                case TokenType.sizeofType:
                case TokenType.stringType:
                    type = TermType.id;
                    break;
                case TokenType.asgnType:
                    type = TermType.asgn;
                    break;
                case TokenType.procType:
                    type = TermType.mod;
                    break;
                case TokenType.ampType:
                    type = TermType.binAnd;
                    break;
                case TokenType.andType:
                    type = TermType.logAnd;
                    break;
                case TokenType.starType:
                    type = TermType.mul;
                    break;
                case TokenType.plusType:
                    type = TermType.plus;
                    break;
                case TokenType.incrType:
                    type = TermType.inc;
                    break;
                case TokenType.minusType:
                    type = TermType.minus;
                    break;
                case TokenType.decrType:
                    type = TermType.dec;
                    break;
                case TokenType.divType:
                    type = TermType.div;
                    break;
                case TokenType.tElseType:
                    type = TermType.cElse;
                    break;
                case TokenType.leftRBType:
                    type = TermType.leftBr;
                    break;
                case TokenType.rightRBType:
                    type = TermType.rightBr;
                    break;
                case TokenType.lessType:
                    type = TermType.less;
                    break;
                case TokenType.leftType:
                    type = TermType.left;
                    break;
                case TokenType.lessEqType:
                    type = TermType.lessEq;
                    break;
                case TokenType.eqEqType:
                    type = TermType.eq;
                    break;
                case TokenType.grtType:
                    type = TermType.grt;
                    break;
                case TokenType.grtEqType:
                    type = TermType.grtEq;
                    break;
                case TokenType.rightType:
                    type = TermType.right;
                    break;
                case TokenType.tIfType:
                    type = TermType.cCond;
                    break;
                case TokenType.xorType:
                    type = TermType.xor;
                    break;
                case TokenType.bOrType:
                    type = TermType.binOr;
                    break;
                case TokenType.lOrType:
                    type = TermType.logOr;
                    break;
                case TokenType.plusEqType:
                    type = TermType.plusEq;
                    break;
                case TokenType.minusEqType:
                    type = TermType.minusEq;
                    break;
                case TokenType.multEqType:
                    type = TermType.mulEq;
                    break;
                case TokenType.divEqType:
                    type = TermType.divEq;
                    break;
                case TokenType.procEq:
                    type = TermType.modEq;
                    break;
                case TokenType.rightEqType:
                    type = TermType.rightEq;
                    break;
                case TokenType.leftEqType:
                    type = TermType.leftEq;
                    break;
                case TokenType.ampEqType:
                    type = TermType.andEq;
                    break;
                case TokenType.xorEqType:
                    type = TermType.xorEq;
                    break;
                case TokenType.bOrEqType:
                    type = TermType.orEq;
                    break;
                case TokenType.bNotType:
                    type = TermType.compl;
                    break;
                case TokenType.exrEqType:
                    type = TermType.negEq;
                    break;
                case TokenType.leftABType:
                    type = TermType.leftABtype;
                    break;
            }
            #endregion

            if ((endType == TokenType.rightCBType
                || endType == TokenType.rightRBType)
                && token.Type == TokenType.comType
                && (numOfLeftB == numOfRightB))
            {
                type = TermType.end;
            }

            if(endType == TokenType.semiType && decl && token.Type == TokenType.comType)
            {
                type = TermType.end;
            }

            if(token.Type == endType && (numOfLeftB == numOfRightB))
            {
                type = TermType.end;
            }

            if(token.Type == TokenType.leftCBType)
            {
                MainFSM.Index--;
                var array = new ADVariable() { Type = ADVariable.VarType.array};
                array.ArrayDimensions.Add(new ADArrayDimension());

                StatementHelpers.ArrayValuesDeclaration(array, 0);

                type = TermType.id;

                array.STRecord = new STRecord { Access = STAccess.local, Name = array.Name, Type = STType.array};
                newTerm.Expression = array;
                newTerm.STRecord = array.STRecord;

                var declaration = new ADVariableDeclarations();
                declaration.Variables.Add(array);
                (AnonymousDeclarations as List<IADNode>).Add(declaration);
            }

            if(type != TermType.error)
            {
                if(token.Type == TokenType.decNumberType
                    || token.Type == TokenType.hexNumberType
                    || token.Type == TokenType.octNumberType
                    || token.Type == TokenType.charType)
                {
                    string newId = Guid.NewGuid().ToString("N");

                    newTerm.STRecord.Name = newId;
                    newTerm.STRecord.Value = token.Attribute;
                    newTerm.STRecord.Type = STType.constant;
                }
                else if(token.Type == TokenType.stringType)
                {
                    string newId = Guid.NewGuid().ToString("N");
                    newTerm.STRecord.Name = newId;
                    newTerm.STRecord.Value = token.Attribute;
                    newTerm.STRecord.Type = STType.str;
                    newTerm.STRecord.Access = STAccess.local;

                    var array = new ADVariable
                    {
                        STRecord = newTerm.STRecord,
                        Name = newId,
                        Type = ADVariable.VarType.array
                    };

                    if(MainFSM.PeekNextToken().Type == TokenType.leftABType)
                    {
                        var arrayValue = new ADArrayValue() { STRecord = newTerm.STRecord };
                        arrayValue.ComputeIndexes();

                        newTerm.Expression = arrayValue;
                    }

                    var declaration = new ADVariableDeclarations();
                    declaration.Variables.Add(array);
                    (AnonymousDeclarations as List<IADNode>).Add(declaration);
                }
                else if(token.Type == TokenType.idType)
                {
                    var stRecord = ParserFunctions.STSearch(token.Attribute);

                    if (stRecord == null)
                    {
                        if(MainFSM.PeekNextToken().Type == TokenType.leftRBType)
                        {
                            stRecord = ParserFunctions.STablesStack.Peek().Records.Where(m => m.Name == token.Attribute).FirstOrDefault();

                            if(stRecord == null)
                            {
                                stRecord = new STRecord() { Name = token.Attribute, Type = STType.function };
                            }
                        }

                        if(stRecord == null)
                        {
                            ParserFunctions.SemanticError($"Promenna \'{token.Attribute}\' nebyla deklarovana.");
                        }
                    }

                    if(stRecord.Type == STType.function)
                    {
                        if(SS.Peek().Type == TermType.reference)
                        {
                            stRecord.Address = stRecord.Name;
                            var variable = new ADVariable() { Name = stRecord.Name, STRecord = stRecord, Type = ADVariable.VarType.function};
                            newTerm.Expression = variable;
                        }
                        else
                        {
                            var fceCall = ParserFunctions.fce_call(token.Attribute);
                            fceCall.STRecord = stRecord;
                            newTerm.Expression = fceCall;
                        }
                    }

                    newTerm.STRecord = stRecord;
                }
                else if (token.Type == TokenType.sizeofType)
                {
                    if (MainFSM.GetNextToken().Type != TokenType.leftRBType)
                    {
                        ParserFunctions.SyntaxError("Byl ocekavan znak \'(\'");
                    }

                    if (MainFSM.GetNextToken().Type != TokenType.longType)
                    {
                        ParserFunctions.SyntaxError("Bylo ocekavano klicove slovo \'long\'");
                    }

                    if (MainFSM.GetNextToken().Type != TokenType.rightRBType)
                    {
                        ParserFunctions.SyntaxError("Byl ocekavan znak \')\'");
                    }
                    newTerm.Expression = new ADSizeOfValue();
                    type = TermType.id;
                }

                newTerm.Type = type;
                return newTerm;
            }
            else
            {
                ParserFunctions.SyntaxError("Chyba ve vyrazu.");
                return null;
            }        
        }

        public static void LoadPrecTable()
        {
            var data = string.Empty;

            using (var openFile = File.Open(@"ExternFiles/PRecedenceTable.csv", FileMode.Open))
            {
                var sr = new StreamReader(openFile);

                string firstRow = sr.ReadLine();
                string[] columns = firstRow.Split(';');

                precTable = new int[columns.Length, columns.Length];

                data = sr.ReadToEnd();
                sr.Close();
            }

            string[] rows = data.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < rows.Length; i++)
            {
                string[] cols = rows[i].Split(';').Skip(1).ToArray();
                for (int j = 0; j < cols.Length; j++)
                {
                    precTable[i, j] = GetPrecTableSymbol(cols[j]);
                }
            }
        }

        private static int GetPrecTableSymbol(string symbol)
        {
            switch (symbol)
            {
                case "G":
                    return G;
                case "L":
                    return L;
                case "B":
                    return B;
                case "E":
                    return E;
                case "S":
                    return S;
            }

            return 0;
        }
    }
}
