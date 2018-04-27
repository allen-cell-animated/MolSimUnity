using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace AICS.AgentSim
{
    /// <summary>
    /// This class evaluates simple math expressions
    /// </summary>
    public static class MathParser
    {
        private enum EOperations
        {
            Addition = '+',
            Subtraction = '-',
            Multiplication = '*',
            Division = '/',
            Exponent = '^',
            LeftParenthesis = '(',
            RightParenthesis = ')'
        };

        private static Dictionary<EOperations, int> OperationPriority = new Dictionary<EOperations, int>()
        {
            {EOperations.LeftParenthesis, -1}, // Parenthesis priority is handled in the evaluation function
            {EOperations.RightParenthesis, -1},
            {EOperations.Exponent, 2},
            {EOperations.Multiplication, 1},
            {EOperations.Division, 1},
            {EOperations.Addition, 0},
            {EOperations.Subtraction, 0},
        };

        private static bool isOperator(char c)
        {
            foreach(EOperations e in Enum.GetValues(typeof(EOperations)))
            {
                if ((EOperations)c == e) return true;
            }

            return false;
        }

        /// <summary>
        /// Evaluate a simple mathematical expression
        /// Recognizes +, -, *, /, ^,and ()
        /// Uses the Dijkstra Shunting-yard algorithm
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        public static float Evaluate(string exp, Dictionary<string,float> variables)
        {
            float sum = 0;

            Stack<float> vals = new Stack<float>();
            Stack<EOperations> ops = new Stack<EOperations>();
            string s = "";

            for(int i = 0; i < exp.Length; ++i)
            {
                // Parenthesis logic
                if(exp[i] == '(')
                {
                    ops.Push(EOperations.LeftParenthesis);
                    continue;
                }

                if (exp[i] == ')')
                {
                    while (ops.Peek() != EOperations.LeftParenthesis)
                    {
                        EOperations op = ops.Pop();
                        float val1 = vals.Pop();
                        float val2 = vals.Pop();
                        float result = EvalBinaryOperation(val1, val2, op);

                        vals.Push(result);
                    }

                    ops.Pop(); // remove left parenthesis
                    continue;
                }

                // Operator logic
                if (isOperator(exp[i]))
                {
                    // Perform higher priority operations already recorded
                    while (ops.Count > 0 &&
                        OperationPriority[ops.Peek()] > OperationPriority[(EOperations)(exp[i])])
                    {
                        EOperations op = ops.Pop();
                        float val1 = vals.Pop();
                        float val2 = vals.Pop();
                        float result = EvalBinaryOperation(val1, val2, op);

                        vals.Push(result);
                    }

                    ops.Push((EOperations)exp[i]);
                    continue;
                }
                else
                {
                    // Operand logic
                    while (i < exp.Length && !isOperator(exp[i]))
                    {
                        // captures the next character after special characters
                        if(exp[i] == 'e' || exp[i] == '.') { s += exp[i++]; }

                        s += exp[i++];
                        continue;
                    }

                    // Check if the operand is a variable or a numerical value
                    float num = 0;
                    bool isNum = float.TryParse(s, out num);

                    // If this operand is a variable, substitute the value from the variable dictionary
                    if (!isNum)
                    {
                        if (!variables.ContainsKey(s))
                        {
                            Debug.LogError(String.Format("Math Parser Error: An unrecognized variable was used in an expression {0}", s));
                        }
                        num = variables[s];
                    }

                    vals.Push(num);
                    s = "";
                    --i;
                }
            }

            while(ops.Count > 0)
            {
                EOperations op = ops.Pop();
                float val1 = vals.Pop();
                float val2 = vals.Pop();

                float result = EvalBinaryOperation(val1, val2, op);

                vals.Push(result);
            }

            sum = vals.Pop();
            return sum;
        }

        /// <summary>
        ///  Evaluates an operation with two operands
        /// </summary>
        /// <param name="a"> The first operand</param>
        /// <param name="b"> The second operand</param>
        /// <param name="op"> The operation to perform</param>
        /// <returns></returns>
        private static float EvalBinaryOperation(float a, float b, EOperations op)
        {
            switch(op)
            {
                case EOperations.Addition: return a + b;
                case EOperations.Subtraction: return a - b;
                case EOperations.Multiplication: return a * b;
                case EOperations.Division: return a / b;
                case EOperations.Exponent: return (float)Math.Pow(a,b);
                default: Debug.LogError(String.Format("Unkown Operation requested in Math Parser: {0}",(char)op)); break;
            }

            return int.MaxValue;
        }
    }
}