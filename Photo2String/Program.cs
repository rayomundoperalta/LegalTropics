using System;
using System.IO;

namespace Photo2String
{
    class Program
    {
        // var sevenItems = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
        static void Main(string[] args)
        {
            FileInfo fi = new FileInfo(@"C:\Users\Rayo\Pictures\Centro Global de Servicios.png");

            if (fi.Exists)
            {
                byte[] bData = null;

                // Read file data into buffer

                using (FileStream fs = fi.OpenRead())
                {
                    bData = new byte[fi.Length];

                    int nReadLength = fs.Read(bData, 0, (int)(fi.Length));
                    Console.Write("byte[] ImagenDefault = new byte[] {");
                    for (int i = 0; i < nReadLength - 1; i++)
                    {
                        Console.Write(bData[i] + ",");
                    }
                    Console.Write(bData[nReadLength - 1]);
                    Console.Write("};");
                }
            }
            Console.ReadKey();
        }
    }
}
