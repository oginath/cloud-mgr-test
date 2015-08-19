using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Auth;
using System.Net;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConsoleApplication1.Model;
using System.Collections;

class GDriveManager : ICloudManager
{
    DriveService service;
    FilesResource.ListRequest listRequest;
    string firstPageToken;

    public void Connect()
    {
        string[] Scopes = new string[] { DriveService.Scope.Drive,
                                     DriveService.Scope.DriveFile};
        string ApplicationName = "Drive API Quickstart";
        UserCredential credential;
        using (var stream =
                    new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
        {
            string credPath = System.Environment.GetFolderPath(
                System.Environment.SpecialFolder.Personal);
            credPath = Path.Combine(credPath, ".credentials");

            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true)).Result;
            //Console.WriteLine("Credential file saved to: " + credPath);
        }

        service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });
        listRequest = service.Files.List();

        listRequest.MaxResults = 1000;

        firstPageToken = listRequest.PageToken;

    }

    public string GetFilesList(string dirPath)
    {
        listRequest.Q = "trashed=false";
        listRequest.PageToken = firstPageToken;
        FileList Files = listRequest.Execute();
        StringBuilder sb = new StringBuilder();
        string parentID;

        if (dirPath == null)
        {
            parentID = service.About.Get().Execute().RootFolderId;
        }
        else
        {
            var Directory = GetFileByPath(dirPath);
            if (Directory != null)
            {
                if (Directory.MimeType.Equals("application/vnd.google-apps.folder"))
                {
                    parentID = Directory.Id;
                }
                else
                {
                    throw new Exception("Given path is not a directory");
                }
            }
            else
            {
                throw new Exception("No directory found at given path");
            }
        }
        if (Files.Items != null && Files.Items.Count > 0)
        {
            while (Files != null)
            {
                foreach (var File in Files.Items)
                {
                    if (File.Parents.Any())
                    {
                        if ((from f in File.Parents where f.Id.Equals(parentID) select f).Any())
                        {
                            if (File.MimeType.Equals("application/vnd.google-apps.folder"))
                            {
                                sb.AppendLine("-> " + File.Title);
                            }
                            else
                            {
                                sb.AppendLine(File.Title);
                            }
                        }
                    }
                }

                if (Files.NextPageToken == null)
                {
                    break;
                }

                listRequest.PageToken = Files.NextPageToken;
                Files = listRequest.Execute();
            }
        }
        else
        {
            sb.AppendLine("No files found.");
        }
        return sb.ToString();
    }

    public void DownloadFile(string FilePath, string DirectoryPath)
    {
        if (System.IO.Directory.Exists(DirectoryPath))
        {
            Google.Apis.Drive.v2.Data.File File = GetFileByPath(FilePath);
            if (File != null)
            {
                if (File.DownloadUrl != null)
                {
                    byte[] FileBytes = service.HttpClient.GetByteArrayAsync(File.DownloadUrl).Result;
                    System.IO.File.WriteAllBytes(DirectoryPath + File.Title, FileBytes);
                }
                else if(File.MimeType.Equals("application/vnd.google-apps.folder"))
                {
                    throw new Exception("Cannot download a directory");
                }
            }
            else
                throw new Exception("File not found");
        }
        else
            throw new Exception("No such local directory");
    }

    public void UploadFile(string FilePath, string dirPath)
    {
        if (System.IO.File.Exists(FilePath))
        {
            var File = GetFileByPath(dirPath);
            if (File != null && File.MimeType.Equals("application/vnd.google-apps.folder"))
            {
                Google.Apis.Drive.v2.Data.File body = new Google.Apis.Drive.v2.Data.File();
                body.Title = System.IO.Path.GetFileName(FilePath);
                body.Description = "File uploaded by Drive Test";

                body.MimeType = GDriveManager.GetMimeType(FilePath);
                body.Parents = new List<ParentReference>() { new ParentReference() { Id = File.Id } };

                byte[] FileBytes = System.IO.File.ReadAllBytes(FilePath);
                System.IO.MemoryStream stream = new System.IO.MemoryStream(FileBytes);
                try
                {
                    FilesResource.InsertMediaUpload request = service.Files.Insert(body, stream, body.MimeType);
                    request.Upload();
                    Google.Apis.Drive.v2.Data.File res = request.ResponseBody;
                }
                catch (Exception e)
                {
                    throw new Exception("An error occurred: " + e.Message);
                }
            }

            else
            {
                throw new Exception("Upload destination not found");
            }
        }
        else
        {
            throw new Exception("File not found");
        }
    }

    public void DeleteFile(string FilePath)
    {
        Google.Apis.Drive.v2.Data.File File = GetFileByPath(FilePath);
        if (File != null)
        {
            if (!File.MimeType.Equals("application/vnd.google-apps.folder"))
            {
                FilesResource.DeleteRequest DeleteRequest = service.Files.Delete(File.Id);
                DeleteRequest.Execute();
            }
            else
            {
                throw new Exception("Cannot currently delete a folder");
            }
        }
        else
            throw new Exception("File not found");
    }

    public void CreateDirectory(string title, string dirPath)
    {
        string parentID;
        if (dirPath != null)
        {
            var Directory = GetFileByPath(dirPath);
            if (Directory != null)
            {
                if (Directory.MimeType.Equals("application/vnd.google-apps.folder"))
                {
                    parentID = Directory.Id;
                }
                else
                {
                    throw new Exception("Given path is not a directory");
                }
            }
            else
            {
                throw new Exception("No directory found at given path");
            }
        }
        else
        {
            parentID = service.About.Get().Execute().RootFolderId;
        }

        Google.Apis.Drive.v2.Data.File body = new Google.Apis.Drive.v2.Data.File();
        body.Title = title;
        body.Description = "Descripiton";
        body.MimeType = "application/vnd.google-apps.folder";
        body.Parents = new List<ParentReference>() { new ParentReference() { Id = parentID } };
        try
        {
            FilesResource.InsertRequest request = service.Files.Insert(body);
            request.Execute();
        }
        catch (Exception e)
        {
            throw new Exception("An error occurred: " + e.Message);
        }
    }

    public float GetFileSizeInMB(string FilePath)
    {
        if (FilePath == null)
        {
            return (float)(service.About.Get().Execute().QuotaBytesTotal / 1024f) / 1024f;
        }
        Google.Apis.Drive.v2.Data.File File = GetFileByPath(FilePath);
        if (File != null)
        {
            if (File.FileSize != null)
            {
                return (float)(File.FileSize / 1024f) / 1024f;
            }
            else if (File.MimeType.Equals("application/vnd.google-apps.folder"))
            {
                throw new Exception("Cannot return size for a folder");
            }
        }
        throw new Exception("File not found");
    }

    private Google.Apis.Drive.v2.Data.File GetFileByPath(string file)
    {
        string[] fileDir = file.Split('\\');
        string parentID = service.About.Get().Execute().RootFolderId;
        listRequest.MaxResults = 1000;
        listRequest.PageToken = firstPageToken;
        FileList Files = listRequest.Execute();

        bool flag = false;

        for (int i = 0; i < fileDir.Length; i++)
        {
            if (flag)
                break;

            if (listRequest.PageToken != firstPageToken)
            {
                listRequest.PageToken = firstPageToken;
                Files = listRequest.Execute();
            }

            do
            {
                var possibleFile = (from f in Files.Items
                                    where (from p in f.Parents where p.Id.Equals(parentID) select p).Any()
              && f.Title.Equals(fileDir[i])
                                    select f);
                if (possibleFile.Any())
                {
                    if (i == fileDir.Length - 1)
                        return possibleFile.ElementAt(0);
                    else
                        parentID = possibleFile.ElementAt(0).Id;
                    break;
                }

                if (Files.NextPageToken == null)
                {
                    flag = true;
                    break;
                }

                listRequest.PageToken = Files.NextPageToken;
                Files = listRequest.Execute();

            } while (Files != null && Files.Items.Count > 0);
        }
        return null;
    }

    private static string GetMimeType(string fileName)
    {
        string mimeType = "application/unknown";
        string ext = System.IO.Path.GetExtension(fileName).ToLower();
        Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
        if (regKey != null && regKey.GetValue("Content Type") != null)
            mimeType = regKey.GetValue("Content Type").ToString();
        return mimeType;
    }

}
