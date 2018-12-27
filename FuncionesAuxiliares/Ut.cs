using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;

namespace FuncionesAuxiliares
{
    public static class Ut
    {
        static public string SinA(string Cadena)
        {
            return Cadena.ToLower().Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u").Replace("ü", "u")
                .Replace("\n", string.Empty).Replace("\t", string.Empty);
        }

        static public string FechaString(string año, string mes, string dia, string PorDefecto)
        {
            if (año.Equals(string.Empty) || año.Equals("0") || año.Equals("1900"))
            {
                return PorDefecto;
            }
            else
            {
                if (mes.Equals(string.Empty) || mes.Equals("0"))
                {
                    return año;
                }
                else
                {
                    string MesString = string.Empty;
                    switch (mes)
                    {
                        case "1":
                            MesString = "ene";
                            break;

                        case "2":
                            MesString = "feb";
                            break;

                        case "3":
                            MesString = "mar";
                            break;

                        case "4":
                            MesString = "abr";
                            break;

                        case "5":
                            MesString = "may";
                            break;

                        case "6":
                            MesString = "jun";
                            break;

                        case "7":
                            MesString = "jul";
                            break;

                        case "8":
                            MesString = "ago";
                            break;

                        case "9":
                            MesString = "sep";
                            break;

                        case "10":
                            MesString = "oct";
                            break;

                        case "11":
                            MesString = "nov";
                            break;

                        case "12":
                            MesString = "dic";
                            break;

                        default:
                            MesString = string.Empty;
                            break;
                    }
                    if (dia.Equals(string.Empty) || dia.Equals("0"))
                    {

                        return año + "-" + MesString;
                    }
                    else
                    {
                        if (dia.Length == 1)
                        {
                            return año + "-" + MesString + "-0" + dia;
                        }
                        else
                        {
                            return año + "-" + MesString + "-" + dia;
                        }

                    }
                }
            }
        }

        static public int Numero(string entero)
        {
            string Pattern = @"[0123456789]+";
            Regex ParseRegex = new Regex(Pattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);

            foreach (Match m in ParseRegex.Matches(entero.Trim()))
            {
                return int.Parse(entero);
            }
            return 0;
        }
    }
}
