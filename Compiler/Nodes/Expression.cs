using Parser.ADT.Functions;
using Parser.ADT.Interfaces;
using Parser.ADT.Operands;
using Parser.ADT.OperationNodes;
using Parser.Precedence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public static partial class AssemblyCompiler
    {
        public static string PrintExpression(IADExpression expression, bool recursive = false, bool dereference = false, string testFailsLabel = "", string testSuccessLabel = "")
        {
            string result = string.Empty;

            if(expression == null)
            {
                return string.Empty;
            }

            if(expression is ADFunctionCall)
            {
                result += PrintFunctionCall(expression as ADFunctionCall);
                result+=("movq %rax, %rdx") + Environment.NewLine;
                result += ("pushq %rdx") + Environment.NewLine;
            }

            if(expression is ADSizeOfValue)
            {
                result += "movq $8, %rdx" + Environment.NewLine;
                result += "pushq %rdx" + Environment.NewLine;
            }

            if(expression is ADArrayValue)
            {
                result += PrintArrayValue(expression as ADArrayValue);
                result += ("pushq %rdx") + Environment.NewLine;
            }

            // PostOrder tree travelsal
            if(expression is ADExpression)
            {
                var expr = expression as ADExpression;

                if(expr.Operator == TermType.inc)
                {
                    if(expr.Left == null)
                    {
                        result += ($"addq $1, {(expr.Right as ADVariable).STRecord.Address}") + Environment.NewLine;
                        result += ($"pushq {(expr.Right as ADVariable).STRecord.Address}") + Environment.NewLine;
                    }
                    else if(expr.Right == null)
                    {
                        result += ($"pushq {(expr.Left as ADVariable).STRecord.Address}") +Environment.NewLine;
                        result += ($"addq $1, {(expr.Left as ADVariable).STRecord.Address}") + Environment.NewLine;
                    }
                }
                else if(expr.Operator == TermType.dec)
                {
                    if (expr.Left == null)
                    {
                        result += ($"subq $1, {(expr.Right as ADVariable).STRecord.Address}") + Environment.NewLine;
                        result += ($"pushq {(expr.Right as ADVariable).STRecord.Address}") + Environment.NewLine;
                    }
                    else if (expr.Right == null)
                    {
                        result += ($"pushq {(expr.Left as ADVariable).STRecord.Address}") + Environment.NewLine;
                        result += ($"subq $1, {(expr.Left as ADVariable).STRecord.Address}") + Environment.NewLine;
                    }
                }
                else if(expr.Operator == TermType.reference)
                {
                    if((expr.Right as ADVariable).STRecord.Access == Parser.SymbolTable.STAccess.global)
                    {
                        if((expr.Right as ADVariable).STRecord.Type == Parser.SymbolTable.STType.function)
                        {
                            result += $"movabs ${(expr.Right as ADVariable).Name}, %rdx" + Environment.NewLine;
                        }
                        else
                        {
                            result += $"movq ${(expr.Right as ADVariable).STRecord.Address}, %rdx" + Environment.NewLine;
                        }
                    }
                    else
                    {
                        result += ($"leaq {(expr.Right as ADVariable).STRecord.Address}, %rdx") + Environment.NewLine;     
                    }

                    result += ("pushq %rdx") + Environment.NewLine;
                }
                else if(expr.Operator == TermType.dereference)
                {
                    result += PrintExpression(expr.Right, true, true);
                    result += ("popq %rdx") + Environment.NewLine;
                    result += ("movq (%rdx), %rcx") + Environment.NewLine;
                    result += ("movq %rcx, %rdx") + Environment.NewLine;
                    result += ("pushq %rdx") + Environment.NewLine;
                }
                else if(expr.Operator == TermType.asgn)
                {
                    result += $"# Prirazeni promenne" + Environment.NewLine;
                    result += PrintExpression(expr.Right, false);

                    result += PrintAssignRdx(expr.Left);
                    recursive = true;
                }
                else if(expr.Operator == TermType.not)
                {
                    result += PrintExpression(expr.Right, false);

                    var label = GenerateLabel();
                    var endLabel = GenerateLabel();

                    result += ("cmp $0, %rdx") + Environment.NewLine;
                    result += ($"je {label}") + Environment.NewLine;
                    result += ("movq $0, %rdx") + Environment.NewLine;
                    result += ($"jmp {endLabel}") + Environment.NewLine;
                    result += ($"{label}:") + Environment.NewLine;
                    result += ("movq $1, %rdx") + Environment.NewLine;
                    result += ($"{endLabel}:") + Environment.NewLine;

                    result += ("pushq %rdx") + Environment.NewLine;
                }
                else if(expr.Left == null && expr.Operator == TermType.minus)
                {
                    result += PrintExpression(expr.Right);
                    result += ("negq %rdx") + Environment.NewLine;
                    result += ("pushq %rdx") + Environment.NewLine;
                }
                else if(expr.Operator == TermType.cCond)
                {
                    result += ("# Tercialni operator") + Environment.NewLine;
                    result += PrintExpression(expr.Left);

                    var nextExpr = expr.Right as ADExpression;
                    var elseLabel = GenerateLabel();
                    var endCondLabel = GenerateLabel();

                    result += ("cmp $0, %rdx") + Environment.NewLine;
                    result += ($"je {elseLabel}") + Environment.NewLine;
                    result += PrintExpression(nextExpr.Left);
                    result += ($"jmp {endCondLabel}") + Environment.NewLine;

                    result += ($"{elseLabel}:") + Environment.NewLine;
                    result += PrintExpression(nextExpr.Right);
                    result += ($"{endCondLabel}:") + Environment.NewLine;
                    result += ("pushq %rdx") + Environment.NewLine; 
                }
                else if (expr.Operator == TermType.logAnd && testFailsLabel != "")
                {
                    result += PrintExpression(expr.Left, true);

                    result += "cmp $0, %rdx" + Environment.NewLine;
                    result += $"je {testFailsLabel}" + Environment.NewLine;
                    result += PrintExpression(expr.Right, true);
                    result += PrintOperation(expr);
                }
                else
                {
                    result += PrintExpression(expr.Left, true);
                    result += PrintExpression(expr.Right, true);
                    result += PrintOperation(expr, dereference);
                }
            }

            if(expression is ADConstant)
            {
                result += ($"pushq ${(expression as ADConstant).Value}") + Environment.NewLine;
            }

            if(expression is ADVariable)
            {
                var variable = expression as ADVariable;

                result += ($"# promenna {variable.Name}") + Environment.NewLine;
                result += ($"pushq {variable.STRecord.Address}") + Environment.NewLine;
            }

            if (!recursive)
            {
                result += ("popq %rdx") + Environment.NewLine;
            }

            return result;
        }

        public static string PrintArrayValue(ADArrayValue value)
        {
            string result = string.Empty;

            for(int i = 0; i < value.Indexes.Count; i++)
            {
                result += PrintExpression(new ADExpression { Left = value.Indexes[i], Operator = TermType.mul, Right = new ADConstant { Value = "8" } });
                result += "movq %rdx, %rsi" + Environment.NewLine;

                if (i == 0)
                {
                    if (value.STRecord.Access == Parser.SymbolTable.STAccess.global)
                    {
                        if(value.STRecord.Type == Parser.SymbolTable.STType.array)
                        {
                            result += $"movq ${value.STRecord.Address}, %rdx" + Environment.NewLine;
                        }
                        else 
                        {
                            result += $"movq {value.STRecord.Address}, %rdx" + Environment.NewLine;
                        }
                    }
                    else
                    {
                        result += $"movq {value.STRecord.Address}, %rdx" + Environment.NewLine;
                    }
                }
                else
                {
                    result += "movq %rcx, %rdx" + Environment.NewLine;
                }

                result += "addq %rsi, %rdx" + Environment.NewLine;

                result += "movq (%rdx), %rsi" + Environment.NewLine;
                result += "movq %rsi, %rcx" + Environment.NewLine;
            }

            result += "movq %rcx, %rdx" + Environment.NewLine;
            return result;
        }

        public static string GetArrayValuePointer(ADArrayValue value)
        {
            string result = string.Empty;
            foreach (var index in value.Indexes)
            {
                result += PrintExpression(new ADExpression { Left = index, Operator = TermType.mul, Right = new ADConstant { Value = "8" } });

                result += "movq %rdx, %rsi" + Environment.NewLine;

                if(value.STRecord.Access == Parser.SymbolTable.STAccess.global)
                {
                    result += $"movq ${value.STRecord.Address}, %rdx" + Environment.NewLine;
                }
                else
                {
                    result += $"movq {value.STRecord.Address}, %rdx" + Environment.NewLine;
                }

                result += "addq %rsi, %rdx" + Environment.NewLine;

            }
            return result;
        }
  
        public static string PrintAssignRdx(IADExpression left)
        {
            string result = string.Empty;
            result += ("movq %rdx, %rcx") + Environment.NewLine;

            if (left is ADExpression)
            {
                if ((left as ADExpression).Operator == TermType.dereference)
                {
                    result += PrintExpression((left as ADExpression).Right, false, true);
                    result += ("movq %rcx, (%rdx)") + Environment.NewLine;
                }
            }
            else if (left is ADVariable)
            {
                result += ($"movq %rcx, {(left as ADVariable).STRecord.Address}") + Environment.NewLine;
            }
            else if(left is ADArrayValue)
            {
                result += GetArrayValuePointer(left as ADArrayValue);
                result += "movq %rcx, (%rdx)" + Environment.NewLine;
                result += "movq (%rdx), %rdx" + Environment.NewLine;
            }

            return result;
        }

        public static string PrintOperation(ADExpression expression, bool dereference = false)
        {
            string result = string.Empty;

            if(expression.Right != null)
            {
                result += ("popq %rsi") + Environment.NewLine;
            }

            if(expression.Left != null)
            {
                result += ("popq %rdx") + Environment.NewLine;
            }

            switch (expression.Operator)
            {
                case TermType.plus:
                    if (expression.Left is ADVariable &&
                        (expression.Left as ADVariable).STRecord.Type == Parser.SymbolTable.STType.array
                        && !dereference)
                    {
                        result += "#Nasobim index 8 kvuli poli" + Environment.NewLine;
                        result += "pushq %rdx" + Environment.NewLine;

                        result += "movq %rsi, %rax" + Environment.NewLine;
                        result += "movq $8, %rcx" + Environment.NewLine;
                        result += "mulq %rcx" + Environment.NewLine;
                        result += "movq %rax, %rsi" + Environment.NewLine;

                        result += "popq %rdx" + Environment.NewLine;
                    }
                    result += ("addq %rsi, %rdx") + Environment.NewLine;
                    break;
                case TermType.compl:
                    result += "notq %rsi" + Environment.NewLine;
                    result += "movq %rsi, %rdx" + Environment.NewLine;
                    break;
                case TermType.minus:
                    result += ("subq %rsi, %rdx") + Environment.NewLine;
                    break;
                case TermType.binAnd:
                    result += ("andq %rsi, %rdx") + Environment.NewLine;
                    break;
                case TermType.mul:
                    result += ("movq %rsi, %rax") + Environment.NewLine;
                    result += ("mulq %rdx") + Environment.NewLine;
                    result += ("movq %rax, %rdx") + Environment.NewLine;
                    break;
                case TermType.div:
                    result += "movq %rdx, %rcx" + Environment.NewLine;
                    result += "xorq %rdx, %rdx" + Environment.NewLine;
                    result += "movq %rcx, %rax" + Environment.NewLine;
                    result += "divq %rsi" + Environment.NewLine;
                    result += "movq %rax, %rdx" + Environment.NewLine;
                    break;
                case TermType.left:
                    result += ("mov %sil, %cl") + Environment.NewLine;
                    result += ("shlq %cl, %rdx") + Environment.NewLine;
                    break;
                case TermType.right:
                    result += ("mov %sil, %cl") + Environment.NewLine;
                    result += ("shrq %cl, %rdx") + Environment.NewLine;
                    break;
                case TermType.binOr:
                    result += ("orq %rsi, %rdx") + Environment.NewLine;
                    break;
                case TermType.rightEq:
                    result += PrintExpression(new ADExpression { Left = expression.Left, Operator = TermType.right, Right = expression.Right });
                    result += PrintAssignRdx(expression.Left);
                    break;
                case TermType.mod:
                    result += "movq %rdx, %rcx" + Environment.NewLine;
                    result += "xorq %rdx, %rdx" + Environment.NewLine;
                    result += "movq %rcx, %rax" + Environment.NewLine;
                    result += "divq %rsi" + Environment.NewLine;
                    break;
                case TermType.modEq:
                    result += PrintExpression(new ADExpression { Left = expression.Left, Operator = TermType.mod, Right = expression.Right });
                    result += PrintAssignRdx(expression.Left);
                    break;
                case TermType.leftEq:
                    result += PrintExpression(new ADExpression { Left = expression.Left, Operator = TermType.left, Right = expression.Right });
                    result += PrintAssignRdx(expression.Left);
                    break;
                case TermType.reference:
                    result += ($"leaq {(expression.Right as ADVariable).STRecord.Address}, %rdx") + Environment.NewLine;
                    break;
                case TermType.dereference:
                    result += ("popq %rdx") + Environment.NewLine;
                    result += ("movq (%rdx), %rcx") + Environment.NewLine;
                    result += ("movq %rcx, %rdx") + Environment.NewLine;
                    break;
                case TermType.less:
                    result += ("cmpq %rsi, %rdx") + Environment.NewLine;
                    result += ("setl %cl") + Environment.NewLine;
                    result += ("xorq %rdx, %rdx") + Environment.NewLine;
                    result += ("mov %cl, %dl") + Environment.NewLine;
                    break;
                case TermType.lessEq:
                    result += ("cmpq %rsi, %rdx") + Environment.NewLine;
                    result += ("setle %cl") + Environment.NewLine;
                    result += ("xorq %rdx, %rdx") + Environment.NewLine;
                    result += ("mov %cl, %dl") + Environment.NewLine;
                    break;
                case TermType.grt:
                    result += ("cmpq %rsi, %rdx") + Environment.NewLine;
                    result += ("setg %cl") + Environment.NewLine;
                    result += ("xorq %rdx, %rdx") + Environment.NewLine;
                    result += ("mov %cl, %dl") + Environment.NewLine;
                    break;
                case TermType.grtEq:
                    result += ("cmpq %rsi, %rdx") + Environment.NewLine;
                    result += ("setge %cl") + Environment.NewLine;
                    result += ("xorq %rdx, %rdx") + Environment.NewLine;
                    result += ("mov %cl, %dl") + Environment.NewLine;
                    break;
                case TermType.eq:
                    result += ("cmpq %rsi, %rdx") + Environment.NewLine;
                    result += ("sete %cl") + Environment.NewLine;
                    result += ("xorq %rdx, %rdx") + Environment.NewLine;
                    result += ("mov %cl, %dl") + Environment.NewLine;
                    break;
                case TermType.negEq:
                    result += ("cmpq %rsi, %rdx") + Environment.NewLine;
                    result += ("setne %cl") + Environment.NewLine;
                    result += ("xorq %rdx, %rdx") + Environment.NewLine;
                    result += ("mov %cl, %dl") + Environment.NewLine;
                    break;
                case TermType.logAnd:
                    result += GetLogicalExpression("%rdx");
                    result += GetLogicalExpression("%rsi");

                    result += ("andq %rsi, %rdx") + Environment.NewLine;
                    break;
                case TermType.logOr:
                    result += ("orq %rsi, %rdx") + Environment.NewLine;
                    break;
                case TermType.xorEq:
                    result += PrintExpression(new ADExpression { Left = expression.Left, Operator = TermType.xor, Right = expression.Right});
                    result += PrintAssignRdx(expression.Left);
                    break;
                case TermType.xor:
                    result += "xorq %rsi, %rdx" + Environment.NewLine;
                    break;
                case TermType.mulEq:
                    result += PrintExpression(new ADExpression { Left = expression.Left, Operator = TermType.mul, Right = expression.Right });
                    result += PrintAssignRdx(expression.Left);
                    break;
                case TermType.plusEq:
                    result += PrintExpression(new ADExpression { Left = expression.Left, Operator = TermType.plus, Right = expression.Right });
                    result += PrintAssignRdx(expression.Left);
                    break;
                case TermType.minusEq:
                    result += PrintExpression(new ADExpression { Left = expression.Left, Operator = TermType.minus, Right = expression.Right });
                    result += PrintAssignRdx(expression.Left);
                    break;
            }

            result += ("pushq %rdx") + Environment.NewLine;

            return result;
        }

        public static string GetLogicalExpression(string register)
        {
            string result = string.Empty;

            var falseLabel = GenerateLabel();
            var endLabel = GenerateLabel();

            result += $"cmp $0, {register}" + Environment.NewLine;
            result += $"je {falseLabel}" + Environment.NewLine;
            result += $"movq $1, {register}" + Environment.NewLine;
            result += $"jmp {endLabel}" + Environment.NewLine;
            result += $"{falseLabel}:" + Environment.NewLine;
            result += $"movq $0, {register}" + Environment.NewLine;
            result += $"{endLabel}:" + Environment.NewLine;

            return result;
        }
    }
}
