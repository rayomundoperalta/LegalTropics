using Peta;
using APFInfo;
using AccesoBaseDatos;
using System.Windows.Forms;
using System;
using OrgBusqueda;

namespace OrganigramaAdmin
{
    public class Organigrama
    {
        public void LlenaTreeAPF(TreeNodeCollection APFtreeNodes, Node<Registro> APF, int i, bool IsTheFirstTime, ref TreeNode raiz)
        {
            TreeNode newNode;
            if (APF.Data.Id1Presupuesto > 0)
            {
                newNode = new TreeNode("#" + APF.Data.Id1Presupuesto + " " + APF.Data.NombrePuesto + " - " + Datos.Instance.GetNombreFuncionario(APF.Data.ID) + "_" + APF.Data.ID);
            }
            else
            {
                newNode = new TreeNode(APF.Data.NombrePuesto + " - " + Datos.Instance.GetNombreFuncionario(APF.Data.ID) + "_" + APF.Data.ID);

            }
            APF.Data.NodoDelTreeView = newNode;
            if (IsTheFirstTime)
            {
                raiz = newNode;
            }
            APFtreeNodes.Add(newNode);
            if (APF.Sons == null || APF.Sons.Count == 0)
            {
            }
            else
            {
                int j = 0;
                foreach (Node<Registro> nodo in APF.Sons)
                {
                    TreeNode auxiliar = null;
                    LlenaTreeAPF(APFtreeNodes[i].Nodes, nodo, j, false, ref auxiliar);
                    j++;
                }
            }
        }

        public void PrintTreeAPF(Node<Registro> APF, Func<string, int> Print)
        {
            Print(APF.Data.TipoRegistro + " " + APF.Data.NombrePuesto + " " + APF.Data.ID + " " + APF.Data.AbogadoIrresponsable);
            foreach(Node<Registro> hijo in APF.Sons)
            {
                PrintTreeAPF(hijo, Print);
            }
        }

        int i = 1;

        public void SalvaTreeAPF(Node<Registro> APF, Func<string, int> Print, bool NoFirstTime)
        {

            if (NoFirstTime)
            {
                Datos.Instance.InsertRegistroOrganigrama(APF.Data.TipoRegistro, APF.Data.NombrePuesto, APF.Data.ID, APF.Data.AbogadoIrresponsable, APF.Data.Id1Presupuesto, i++, Print);
            }
            else
            {
                i = 1;
            }
            foreach (Node<Registro> hijo in APF.Sons)
            {
                SalvaTreeAPF(hijo, Print, true);
            }
        }

        public void ListaDeNodosAPF(Node<Registro> APF, ListaNombreNodo ListaSecNodos)
        {
            ListaSecNodos.Add(APF);
            foreach (Node<Registro> hijo in APF.Sons)
            {
                ListaDeNodosAPF(hijo, ListaSecNodos);
            }
        }
    }
}
