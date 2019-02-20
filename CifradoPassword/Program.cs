using System;
using System.Text;
using AccesoBaseDatos;
using Peta;

namespace CifradoPassword
{
    /*
            ConsoleKeyInfo cki;
            // Prevent example from ending if CTL+C is pressed.
            Console.TreatControlCAsInput = true;

            Console.WriteLine("Press any combination of CTL, ALT, and SHIFT, and a console key.");
            Console.WriteLine("Press the Escape (Esc) key to quit: \n");
            do
            {
                cki = Console.ReadKey();
                Console.Write(" --- You pressed ");
                if ((cki.Modifiers & ConsoleModifiers.Alt) != 0) Console.Write("ALT+");
                if ((cki.Modifiers & ConsoleModifiers.Shift) != 0) Console.Write("SHIFT+");
                if ((cki.Modifiers & ConsoleModifiers.Control) != 0) Console.Write("CTL+");
                Console.WriteLine(cki.Key.ToString());
            } while (cki.Key != ConsoleKey.Escape);
    */
    class Program
    {
        static string GetSecretString ()
        {
            string cadena = string.Empty;
            ConsoleKeyInfo cki;
            // Prevent example from ending if CTL+C is pressed.
            Console.TreatControlCAsInput = true;
            do
            {
                cki = Console.ReadKey(true);
                Console.Write("*");
                if (((cki.Modifiers & ConsoleModifiers.Alt) == 0) &&
                    ((cki.Modifiers & ConsoleModifiers.Control) == 0) &&
                    (cki.Key != ConsoleKey.Enter))
                {
                    cadena += cki.KeyChar.ToString();
                }
                else
                    Console.Beep();
            } while (cki.Key != ConsoleKey.Enter);
            Console.WriteLine();
            Console.TreatControlCAsInput = false;
            return cadena;
        }

        static string PrintHexaConsole(string cadena)
        {
            for (int j = 0; j < cadena.Length; j++)
            {
                Console.Write(cadena[j]);
                Console.Write(" ");
            }
            Console.WriteLine();
            byte[] ba = Encoding.Default.GetBytes(cadena);
            var hexString = BitConverter.ToString(ba);
            hexString = hexString.Replace("-", "");
            return hexString;
        }

        static void Main(string[] args)
        {
            string userResponsable = string.Empty;
            Console.WriteLine("Administrador de usuarios y contraseñas.");
            Console.Write("No utilice CTL o ALT y otra tecla en los passwords. ");
            Console.WriteLine("Para terminar presione Enter.");
            while (!userResponsable.Equals("rayo") && !userResponsable.Equals("rayo1"))
            {
                Console.WriteLine("Dame User Responsable");
                userResponsable = Console.ReadLine();
            }
            Console.WriteLine("Dame Password");
            string passwordResponsable = GetSecretString();
            
            if (userResponsable.Equals("rayo1"))
            {
                Console.WriteLine(PetaSecure.ComputeSha256Hash(passwordResponsable));
                userResponsable = "rayo";
                Console.WriteLine(userResponsable);
                Console.WriteLine(PrintHexaConsole(userResponsable));
                Console.WriteLine(passwordResponsable);
                Console.WriteLine(PrintHexaConsole(passwordResponsable));
            }
            
            if (Datos.Instance.VerificaAbogadoIrresponsable(userResponsable, passwordResponsable))
            {
                Console.WriteLine("Usuario autorizado");
                string password1 = "1";
                string password2 = "2";
                Console.WriteLine("Dame User");
                string user = Console.ReadLine();

                while (!password1.Equals(password2))
                {
                    Console.WriteLine("Dame Password");
                    password1 = GetSecretString();
                    Console.WriteLine("Dame Password de nuevo");
                    password2 = GetSecretString();
                }
                Console.WriteLine("User: {0}", user);
                Datos.Instance.AltaUserPassword(user, PetaSecure.ComputeSha256Hash(password1));
            }
            else
            {
                Console.WriteLine("Clave de usuario autorizado invalida");
                Console.Beep();
                Console.Beep();
            }
                
            Console.ReadKey();
        }
    }
}
