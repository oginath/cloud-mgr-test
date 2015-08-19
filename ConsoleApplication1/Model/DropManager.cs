using DropNet;
using DropNet.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.Model
{
    class DropManager : ICloudManager
    {
        DropNetClient Client;

        public void Connect()
        {
            Client = new DropNetClient("a8bg7nzyskpde54", "cxv67uum3k93pfv");
            Client.UserLogin = new UserLogin { Token = "t44b3y1zjw0aeqri", Secret = "fc2cfuft0zg2381" };
            //Client.GetToken();
            //string AuthorizeUrl = Client.BuildAuthorizeUrl();
            //Process.Start(AuthorizeUrl);
            //UserLogin AccessToken = Client.GetAccessToken();

        }

        public string GetFilesList(string directory)
        {
            directory = DropManager.ConvertToDrpPath(directory);

            MetaData metaData = null;
            try
            {
                metaData = Client.GetMetaData(directory, null, false, false);
            }
            catch
            {
                throw new Exception("Directory not found (cloud)");
            }
            StringBuilder sb = new StringBuilder();
            if (metaData.Is_Dir)
            {
                if(metaData.Contents.Count > 0)
                {
                    foreach(MetaData md in metaData.Contents)
                    {
                        if(md.Is_Dir)
                            sb.AppendLine("-> " + md.Name);
                        else
                            sb.AppendLine(md.Name);
                    }
                }
                else
                {
                    sb.AppendLine("Directory is Empty");
                }                
            }
            else
            {
                throw new Exception("Given path is not a directory (cloud)");
            }


            return sb.ToString();
        }

        public void DownloadFile(string FilePath, string DirectoryPath)
        {
            FilePath = DropManager.ConvertToDrpPath(FilePath);

            if (System.IO.Directory.Exists(DirectoryPath))
            {
                MetaData metaData = null;
                try
                {
                    metaData = Client.GetMetaData(FilePath, null, false, false);
                }
                catch
                {
                    throw new Exception("File not found (cloud)");
                }
                if (!metaData.Is_Dir)
                {
                    byte[] FileBytes = Client.GetFile(FilePath);
                    System.IO.File.WriteAllBytes(DirectoryPath + metaData.Name, FileBytes);
                }
                else
                {
                    throw new Exception("Cannot download a directory");
                }
            }
            else
                throw new Exception("Directory not found (local)");
        }

        public void UploadFile(string FilePath, string dirPath)
        {
            dirPath = DropManager.ConvertToDrpPath(dirPath);

            if (System.IO.File.Exists(FilePath))
            {
                MetaData metaData = null;
                try
                {
                    metaData = Client.GetMetaData(dirPath, null, false, false);
                }
                catch
                {
                    throw new Exception("Directory not found (cloud)");
                }
                if (metaData.Is_Dir)
                {
                    byte[] FileBytes = System.IO.File.ReadAllBytes(FilePath);
                    Client.UploadFile(dirPath, System.IO.Path.GetFileName(FilePath), FileBytes);
                }
                else
                {
                    throw new Exception("Directory is not a folder (cloud)");
                }
            }
            else
            {
                throw new Exception("File not found (local)");
            }
        }

        public void DeleteFile(string FilePath)
        {
            FilePath = DropManager.ConvertToDrpPath(FilePath);

            MetaData metaData = null;
            try
            {
                metaData = Client.GetMetaData(FilePath, null, false, false);
            }
            catch
            {
                throw new Exception("File not found (cloud)");
            }
            if(metaData.Is_Dir)
                throw new Exception("Cannot currently delete a folder");
            Client.Delete(FilePath);
        }

        public void CreateDirectory(string title, string dirPath)
        {
            dirPath = DropManager.ConvertToDrpPath(dirPath);
            MetaData metaData = null;
            try
            {
                metaData = Client.GetMetaData(dirPath, null, false, false);
            }
            catch
            {
                throw new Exception("Directory not found (cloud)");
            }
            if (!dirPath.ElementAt(dirPath.Length - 1).Equals('/'))
                dirPath = dirPath + "/";
            dirPath = dirPath + title;
            Client.CreateFolder(dirPath);
        }

        public float GetFileSizeInMB(string FilePath)
        {
            FilePath = DropManager.ConvertToDrpPath(FilePath);
            MetaData metaData = null;
            try
            {
                metaData = Client.GetMetaData(FilePath, null, false, false);
            }
            catch
            {
                throw new Exception("File not found (cloud)");
            }
            if (!metaData.Is_Dir)
            {
                if(metaData.Size.Contains("KB"))
                    return float.Parse(metaData.Size.Remove(metaData.Size.Length - 3)) / 1024;
                else
                    return float.Parse(metaData.Size.Remove(metaData.Size.Length - 3));
            }
            else
            {
                throw new Exception("Cannot return size for a folder");
            }
        }

        private static string ConvertToDrpPath(string path)
        {
            if (path == null)
                return "/";
            if (path.Equals(""))
                return "/";
            if (path.Contains("\\"))
                return path.Replace("\\", "/");
            return path;
        }
    }
}
