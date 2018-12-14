using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncionesAuxiliares
{
    public static class IntParse
    {
        static public int Numero(string entero)
        {
            if (entero.Replace(" ", string.Empty).Equals(string.Empty)) return 0;
            return int.Parse(entero);
        }
    }
}
