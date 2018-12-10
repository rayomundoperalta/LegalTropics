using Arboles;
using APFInfo;
using MSAccess;
using System.Windows.Forms;
using System;
using System.Collections.Generic;

namespace OrganigramaAdmin
{
    public class Organigrama
    {
        public void LlenaTreeAPF(TreeNodeCollection APFtreeNodes, Node<Registro> APF, int i)
        {
            TreeNode newNode = new TreeNode(APF.Data.NombrePuesto + " - " + AccessUtility.GetNombreFuncionario(APF.Data.ID) + "_" + APF.Data.ID);
            APFtreeNodes.Add(newNode);
            if (APF.Sons == null || APF.Sons.Count == 0)
            {
            }
            else
            {
                int j = 0;
                foreach (Node<Registro> nodo in APF.Sons)
                {
                    LlenaTreeAPF(APFtreeNodes[i].Nodes, nodo, j);
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

        
    }
}
