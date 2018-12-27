using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncionesAuxiliares
{
    public class IndiceBD
    {
        int pos;
        int length;

        public IndiceBD(int Length)
        {
            pos = 0;
            length = Length;
        }

        public void Next()
        {
            pos = (pos + 1) % length;
        }

        public void Previous()
        {
            if (pos == 0)
            {
                pos = length - 1;
            }
            else
            {
                pos--;
            }
        }

        public void Inicial()
        {
            pos = 0;
        }

        public void Final()
        {
            pos = length - 1;
        }

        public int Pos { get { return pos; } set { pos = value; } }

        public int Length { get { return length; } }
    }
}
