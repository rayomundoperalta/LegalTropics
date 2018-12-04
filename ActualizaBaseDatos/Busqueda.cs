using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ActualizaBaseDatos
{
    class Busqueda
    {
        List<Regex> PalabrasBuscadas = new List<Regex>();
        
        public void Add(string palabra)
        {
            PalabrasBuscadas.Add(new Regex(SinA(palabra.Replace(" ", string.Empty))));
        }

        private string SinA(string Cadena)
        {
            return Cadena.ToLower().Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u").Replace("ü", "u")
                .Replace("\n", string.Empty).Replace("\t", string.Empty);
        }

        public bool SatisfaceCriterio(string DatosPersonales)
        {
            bool result = true;
            foreach (Regex ParticularRegex in PalabrasBuscadas)
            {
                MatchCollection AllMatches = ParticularRegex.Matches(SinA(DatosPersonales));
                if (AllMatches.Count > 0) { result &= true; } else { result &= false; break; }
            }
            return result;
        }
    }
}
