using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace AICS.AgentSim
{
    /// <summary>
    /// This class evaluates simple math expressions
    /// </summary>
    public static class MathParser
    {
        /// <summary>
        /// Evaluate a simple mathematical expression
        /// Recognizes +, -, *, /, and ()
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        public static float Evaluate(string expression, Dictionary<string,float> variables)
        {
            float sum = 0;
            Debug.LogWarning("Math Expression Evaluation is currently unimplemented");
            return sum;
        }
    }
}