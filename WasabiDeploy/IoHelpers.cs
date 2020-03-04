﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WasabiDeploy
{
    public static class IoHelpers
    {
        public static void DeleteDirectory(string pathToDirectory)
        {
            int attemps = 0;
            if (Directory.Exists(pathToDirectory))
            {
                do
                {
                    try
                    {
                        attemps++;

                        if (attemps > 1)
                        {
                            foreach (var f in Directory.GetFiles(pathToDirectory, "*.*", SearchOption.AllDirectories))
                            {
                                File.SetAttributes(f, FileAttributes.Normal);
                                File.Delete(f);
                            }
                        }

                        Directory.Delete(pathToDirectory, true);
                    }
                    catch (DirectoryNotFoundException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (attemps > 10)
                        {
                            throw ex;
                        }
                    }
                }
                while (Directory.Exists(pathToDirectory));
            }
        }

        public static string GetWorkingDirectory()
        {
            var currentDirectory = new DirectoryInfo("./");
            FileInfo slnFile = null;
            do
            {
                Console.WriteLine($"currentdir: {currentDirectory}");
                slnFile = currentDirectory.GetFiles("WasabiDeploy.sln", SearchOption.TopDirectoryOnly).FirstOrDefault();

                if (slnFile is { })
                {
                    break;
                }

                currentDirectory = currentDirectory.Parent;
            }
            while (currentDirectory.Parent is { });

            var rootDirectory = slnFile.Directory.Parent.FullName;
            return Path.Combine(rootDirectory, "WasabiDeploy.Temp");
        }
    }
}
