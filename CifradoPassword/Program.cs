using System;
using AccesoBaseDatos;
using Peta;

namespace CifradoPassword
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Dame User");
            string user = Console.ReadLine();
            Console.WriteLine("Dame Password");
            string password = Console.ReadLine();
            Console.WriteLine("User: {0}, Password: {1}", user, password);
            Datos.Instance.AltaUserPassword(user, PetaSecure.ComputeSha256Hash(password));
        }
    }
}
