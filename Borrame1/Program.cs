using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CifradoPeta;

namespace Borrame1
{
    class Program
    {
        static void Main(string[] args)
        {

            byte[] aura = PetaSecure.GenerateRandomSalt();
            for ( int i = 0; i < aura.Length; i++)
            {
                Console.Write(aura[i]);            }
            Console.ReadKey();
        }
    }
}
