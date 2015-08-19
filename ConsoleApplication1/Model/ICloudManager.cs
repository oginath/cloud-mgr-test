using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.Model
{
    interface ICloudManager
    {
        void Connect();
        string GetFilesList(string directory);
        void DownloadFile(string file, string dest);
        void UploadFile(string file, string dest);
        void DeleteFile(string file);
        float GetFileSizeInMB(string file);
        void CreateDirectory(string title, string dest);
    }
}
