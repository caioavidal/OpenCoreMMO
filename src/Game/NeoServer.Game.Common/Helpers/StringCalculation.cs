using Microsoft.CodeAnalysis.CSharp.Scripting;
using System;
using System.Data;
using System.Globalization;

namespace NeoServer.Game.Common.Helpers
{
    public class StringCalculation
    {
        private static DataTable dataTable = new DataTable();
        public static double Calculate(string formula, params (string, double)[] arguments)
        {
            foreach (var arg in arguments)
            {
                formula = formula.Replace(arg.Item1, arg.Item2.ToString(CultureInfo.InvariantCulture));
            }
            double result = Convert.ToDouble(dataTable.Compute(formula, null));
            return result;
        }
        public static double Calculate2(string formula, params (string, double)[] arguments)
        {
            foreach (var arg in arguments)
            {
                formula = formula.Replace(arg.Item1, arg.Item2.ToString(CultureInfo.InvariantCulture));
            }

            var result = Convert.ToDouble(CSharpScript.EvaluateAsync(formula).Result);
            return result;
        }
    }
}
