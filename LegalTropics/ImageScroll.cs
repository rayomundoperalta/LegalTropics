using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Globales;

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
                fi.Delete();
            }

            InitializeComponent();

            IDs = MSAccess.GetFotoIDs();

            hScrollBar1.Maximum = IDs.Length - 1;
            hScrollBar1.Minimum = 0;
            hScrollBar1.LargeChange = 1;

            string fileName = MSAccess.GetFoto(IDs[ImagenMostrandose]);
            pictureBox1.Image = Image.FromFile(fileName);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            ImagenMostrandose = hScrollBar1.Value;
            pictureBox1.Image = Image.FromFile(MSAccess.GetFoto(IDs[ImagenMostrandose]));

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Globals.Ribbons.Tropicalizador.GeneraReporte(IDs[ImagenMostrandose]);
        }
    }
}
