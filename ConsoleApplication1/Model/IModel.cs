using ConsoleApplication1.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.Model
{
    interface IModel
    {
        void SetController(CloudController controller);
        string GetFilesList(string directory);
        void DownloadFile(string fileName, string dirPath);
        void UploadFile(string filePath, string dirPath);
        void DeleteFile(string filePath);
        void CreateDirectory(string directoryName, string dirPath);
        float GetFileSizeInMB(string fileName);
    }
}
