using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FuncionesAuxiliares;

namespace ActualizaBaseDatos
{
    public class Busqueda
    {
        List<Regex> PalabrasBuscadas = new List<Regex>();
        Regex Separador = new Regex("[a-zA-ZáéíóúÁÉÍÓÚÑñüÜÇç]+");

        public void AddLine(string LineaBusqueda)
        {
            MatchCollection AllMatches = Separador.Matches(LineaBusqueda);
            foreach (Match palabra in AllMatches)
            {
                for (int i = 0; i < palabra.Groups.Count; i++)
                {
                    PalabrasBuscadas.Add(new Regex(Ut.SinA(palabra.Groups[i].ToString())));
                }
            }
        }

        public void Add(string palabra)
        {
            PalabrasBuscadas.Add(new Regex(Ut.SinA(palabra.Replace(" ", string.Empty))));
        }

        public bool SatisfaceCriterio(string DatosPersonales)
        {
            bool result = true;
            foreach (Regex ParticularRegex in PalabrasBuscadas)
            {
                MatchCollection AllMatches = ParticularRegex.Matches(Ut.SinA(DatosPersonales));
                if (AllMatches.Count > 0) { result &= true; } else { result &= false; break; }
            }
            return result;
        }
    }
}
