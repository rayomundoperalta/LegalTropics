using Arboles;
using APFInfo;
using MSAccess;
using System.Windows.Forms;
using System;

namespace OrganigramaAdmin
{
    public class Organigrama
    {
        public void LlenaTreeAPF(TreeNodeCollection APFtreeNodes, Node<Registro> APF, int i, bool IsTheFirstTime, ref TreeNode raiz)
        {
            TreeNode newNode = new TreeNode(APF.Data.NombrePuesto + " - " + AccessUtility.GetNombreFuncionario(APF.Data.ID) + "_" + APF.Data.ID);
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
            Print(APF.Data.TipoRegistro + " " + APF.Data.NombrePuesto + " " + APF.Data.ID);
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
                AccessUtility.InsertRegistroOrganigrama(APF.Data.TipoRegistro, APF.Data.NombrePuesto, APF.Data.ID, i++, Print);
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
    }
}
