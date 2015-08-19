using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication1.View;
using ConsoleApplication1.Model;

namespace ConsoleApplication1.Controller
{
    class CloudController
    {
        IView View { get; set; }
        IModel Model { get; set; }

        public CloudController(IView v, IModel m)
        {
            View = v;
            Model = m;
        }

        public string GetFilesList(string directory)
        {
            return Model.GetFilesList(directory);
        }

        public void DownloadFile(string fileName, string dirPath)
        {
            Model.DownloadFile(fileName, dirPath);
        }

        public void UploadFile(string filePath, string dirPath)
        {
            Model.UploadFile(filePath, dirPath);
        }

        public void DeleteFile(string filePath)
        {
            Model.DeleteFile(filePath);
        }

        public void CreateDirectory(string directoryName, string dirPath)
        {
            Model.CreateDirectory(directoryName, dirPath);
        }

        public string GetFileSizeInMB(string fileName)
        { 
            return Model.GetFileSizeInMB(fileName).ToString();
        }

        public void DisplayMessage(string message)
        {
            View.DisplayMessage(message);
        }
    }
}
