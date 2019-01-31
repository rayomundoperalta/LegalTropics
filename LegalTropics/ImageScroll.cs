using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Globales;
using AccesoBaseDatos;

namespace LegalTropics
{
    public partial class ImageScroll : Form
    {
        int ImagenMostrandose = 0;
        string[] IDs;

        public ImageScroll()
        {
            // Borramos la basura dejada en la sesiòn anteror
            string[] PhotoFiles = Directory.GetFiles(Defines.FotoTempBasePath, "*.*");

            for (int i = 0; i < PhotoFiles.Length; i++)
            {
                FileInfo fi = new FileInfo(PhotoFiles[i]);
                try
                {
                    fi.Delete();
                }
                catch (IOException) { }
            }
            InitializeComponent();
            IDs = Datos.Instance.GetFotoIDs();

            hScrollBar1.Maximum = IDs.Length - 1;
            hScrollBar1.Minimum = 0;
            hScrollBar1.LargeChange = 1;

            pictureBox1.Image = Image.FromFile(Datos.Instance.GetFoto(IDs[ImagenMostrandose]));
            labelNombreFoto.Text = Datos.Instance.GetNombreFuncionario(IDs[ImagenMostrandose]);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            ImagenMostrandose = hScrollBar1.Value;
            pictureBox1.Image = Image.FromFile(Datos.Instance.GetFoto(IDs[ImagenMostrandose]));
            labelNombreFoto.Text = Datos.Instance.GetNombreFuncionario(IDs[ImagenMostrandose]);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Globals.Ribbons.Tropicalizador.GeneraReporte(IDs[ImagenMostrandose]);
        }
    }
}