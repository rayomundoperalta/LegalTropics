using System;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;

namespace FuncionesAuxiliares
{
    public static class IntParse
    {
        static public int Numero(string entero)
        {
            string Pattern = @"[0123456789]+";
            Regex ParseRegex = new Regex(Pattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);

            foreach(Match m in ParseRegex.Matches(entero.Trim())) {
                return int.Parse(entero);
            }
            return 0;
        }
    }
}
