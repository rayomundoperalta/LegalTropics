using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncionesAuxiliares
{
    static public class FormatoFecha
    {
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
    }
}
