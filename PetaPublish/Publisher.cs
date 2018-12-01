using System.IO;
using System.Net;

namespace PetaPublish
{
    public static class Publisher
    {
        private const string USER = "infoapf";
        private const string PASSWORD = "Samahil";

        public static int DownloadInfoAPFDB(string Path, string Remote)
        {
            string ftphost = "peta.dynu.net";
            //string ftpfilepath = "/" + Defines.DataBaseFileNameEncriptado;
            string ftpfilepath = "/" + Remote;

            string ftpfullpath = "ftp://" + ftphost + ftpfilepath;

            using (WebClient request = new WebClient())
            {
                request.Credentials = new NetworkCredential(USER, PASSWORD);
                byte[] fileData = request.DownloadData(ftpfullpath);

                //using (FileStream file = File.Create(Defines.DataBasePath + Defines.DataBaseFileNameEncriptado))
                using (FileStream file = File.Create(Path + Remote))
                {
                    file.Write(fileData, 0, fileData.Length);
                    file.Close();
                }
                return fileData.Length;
            }
        }

        public static void UploadInfoAPFDB(string Path, string Local)
        {
            string ftphost = "peta.dynu.net";
            string ftpfilepath = "/" + Local;

            string ftpfullpath = "ftp://" + ftphost + ftpfilepath;

            using (WebClient request = new WebClient())
            {
                request.Credentials = new NetworkCredential(USER, PASSWORD);
                request.UploadFile(ftpfullpath, Path + Local);
            }
        }
    }
}
