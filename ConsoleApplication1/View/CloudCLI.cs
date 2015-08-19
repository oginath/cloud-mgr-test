using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication1.Controller;
using ConsoleApplication1.Model;
using ConsoleApplication1.View;


class CloudCLI : IView
{
    string UserInput = "null";
    string[] InputFragmented = null;
    string[] MethodArguments = null;
    CloudController Controller;

    public void Start()
    {
        Console.WriteLine("Hello. Enter a command (for cmd list, type !cmds):");
        while (true)
        {
            UserInput = Console.ReadLine();
            InputFragmented = UserInput.Split(' ');
            switch (InputFragmented[0])
            {
                case "list":
                    MethodArguments = new string[1];
                    if (InputFragmented.Length > 1)
                    {
                        MethodArguments[0] = UserInput.Substring(5);
                    }
                    else
                    {
                        MethodArguments[0] = null;
                    }
                    Console.Write(Controller.GetFilesList(MethodArguments[0]));
                    break;
                case "down":
                    if (InputFragmented.Length > 1)
                    {
                        if (UserInput.Contains("-file") && UserInput.Contains("-path"))
                        {
                            MethodArguments = new string[2];
                            MethodArguments[0] = UserInput.Substring(UserInput.IndexOf("-file") + 6, UserInput.Length - UserInput.Substring(UserInput.IndexOf("-path")).Length - 12);
                            MethodArguments[1] = UserInput.Substring(UserInput.IndexOf("-path") + 6);
                            if (!MethodArguments[1].ElementAt(MethodArguments[1].Length - 1).Equals('\\'))
                                MethodArguments[1] = MethodArguments[1] + "\\";
                            Controller.DownloadFile(MethodArguments[0], MethodArguments[1]);
                        }
                    }
                    break;
                case "up":
                    if (InputFragmented.Length > 1)
                    {
                        if (UserInput.Contains("-file") && UserInput.Contains("-path"))
                        {
                            MethodArguments = new string[2];
                            MethodArguments[0] = UserInput.Substring(UserInput.IndexOf("-file") + 6, UserInput.Length - UserInput.Substring(UserInput.IndexOf("-path")).Length - 10);
                            MethodArguments[1] = UserInput.Substring(UserInput.IndexOf("-path") + 6);
                            Controller.UploadFile(MethodArguments[0], MethodArguments[1]);
                        }
                    }
                    break;
                case "remv":
                    if(InputFragmented.Length > 1)
                    {
                        MethodArguments = new string[1];
                        MethodArguments[0] = UserInput.Substring(5);
                        Console.WriteLine("Are you certain? Y\\N");
                        switch (Console.ReadLine())
                        {
                            case "Y":
                            case "y":
                              Controller.DeleteFile(MethodArguments[0]);
                                break;
                        }
                    }
                    break; 
                case "mkdir":
                    if (InputFragmented.Length > 1)
                    {
                        if (UserInput.Contains("-name") && UserInput.Contains("-path"))
                        {
                            MethodArguments = new string[2];
                            MethodArguments[0] = UserInput.Substring(UserInput.IndexOf("-name") + 6, UserInput.Length - UserInput.Substring(UserInput.IndexOf("-path")).Length - 13);
                            MethodArguments[1] = UserInput.Substring(UserInput.IndexOf("-path") + 6);
                            Controller.CreateDirectory(MethodArguments[0], MethodArguments[1]);
                        }
                    }
                    break;
                case "size":
                    MethodArguments = new string[1];
                    if (InputFragmented.Length > 1)
                    {
                        MethodArguments[0] = UserInput.Substring(5);
                    }
                    else
                    {
                        MethodArguments[0] = null;
                    }
                    Console.WriteLine("File Size: " + Controller.GetFileSizeInMB(MethodArguments[0]) + "MB");
                    break;
                case "!cmds":
                    PrintCmds();
                    break;
                case "exit":
                    return;
            }
            Console.WriteLine("Enter a command:");
        }
    }

    private void PrintCmds()
    {
        Console.WriteLine("list   XYZW                --- list all files in given directory (leave blank for root)");
        Console.WriteLine("down  -file XYZ -path WVK  --- download specified file (cloud) to specified path (local)");
        Console.WriteLine("up    -file XYZ -path WVK  --- upload specified file (local) to specified path (cloud)");
        Console.WriteLine("remv   XYZW                --- delete specified file (cloud)");
        Console.WriteLine("mkdir -name XYZ -path WVK  --- create a directory with specified name in specified path");
        Console.WriteLine("size   XYZW                --- size of a file in specified path (leave blank for cloud)");
        Console.WriteLine("exit                       --- exit the program");
    }

    public void SetController(CloudController controller)
    {
        this.Controller = controller;
    }

    public void DisplayMessage(string message)
    {
        Console.WriteLine(message);
    }
}

