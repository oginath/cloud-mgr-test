using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication1.Controller;

namespace ConsoleApplication1.Model
{
    class CloudModel : IModel
    {

        CloudController Controller;
        List<ICloudManager> Managers;

        public CloudModel()
        {
            Managers = new List<ICloudManager>();

            ICloudManager dm = new GDriveManager();
            Managers.Add(dm);

            foreach (var Manager in Managers)
            {
                Manager.Connect();
            }
        }

        public string GetFilesList(string directory)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var Manager in Managers)
            {
                try
                {
                    sb.AppendLine(Manager.GetFilesList(directory));
                }
                catch (Exception e)
                {
                    Controller.DisplayMessage(e.Message);
                }
            }

            return sb.ToString();
        }

        public void DownloadFile(string fileName, string dirPath)
        {
            Controller.DisplayMessage("Download Starting...");

            // TODO: Download from where
            try
            {
                Managers[0].DownloadFile(fileName, dirPath);
                Controller.DisplayMessage("Finished Downloading");
            }
            catch (Exception e)
            {
                Controller.DisplayMessage(e.Message);
                Controller.DisplayMessage("No files downloaded");
            }
        }

        public void UploadFile(string filePath, string dirPath)
        {
            Controller.DisplayMessage("Upload starting...");

            // TODO: Upload to where
            try
            {
                Managers[0].UploadFile(filePath, dirPath);
                Controller.DisplayMessage("Finished Uploading");
            }
            catch(Exception e)
            {
                if (e.Message.Equals("File not found"))
                {
                    Controller.DisplayMessage(e.Message + " at " + filePath);
                }
                else
                    Controller.DisplayMessage(e.Message);
                Controller.DisplayMessage("No files were uploaded");
            }
        }

        public void DeleteFile(string filePath)
        {
            Controller.DisplayMessage("Deleting File...");
            try
            {
                Managers[0].DeleteFile(filePath);
                Controller.DisplayMessage("File deleted successfully");
            }
            catch (Exception e)
            {
                Controller.DisplayMessage(e.Message);
                Controller.DisplayMessage("No files were deleted");
            }
        }

        public void CreateDirectory(string directoryName, string dirPath)
        {
            // TODO: create according to... ?
            try
            {
                Managers[0].CreateDirectory(directoryName, dirPath);
            }
            catch (Exception e)
            {
                Controller.DisplayMessage(e.Message);
            }
        }

        public float GetFileSizeInMB(string fileName)
        {
            // TODO:
            try
            {
                return Managers[0].GetFileSizeInMB(fileName);
            }
            catch (Exception e)
            {
                Controller.DisplayMessage(e.Message);
                throw new Exception();
            }
        }

        public void SetController(CloudController controller)
        {
            this.Controller = controller;
        }

    }
}
