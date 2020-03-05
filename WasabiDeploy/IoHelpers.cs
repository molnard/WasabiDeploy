using System;
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

        public static void PrintSubdirsToConsole(string path)
        {
            var subdirs = new DirectoryInfo(path).GetDirectories("*", SearchOption.TopDirectoryOnly).Select(di => di.Name);
            Console.WriteLine($"Subdirectories of {path}");
            Console.Write("Sub directories: ");
            if (subdirs is { } && subdirs.Any())
            {
                Console.WriteLine($"{string.Join(", ", subdirs)}");
            }
            else
            {
                Console.WriteLine("None");
            }
        }

        /// <summary>
        /// On local machine the current directory is: C:\work\WasabiDeploy\WasabiDeploy.Publish\bin\Debug
        /// On Azure Pipelines the current directory is: D:\a\1\s\
        /// </summary>
        /// <returns></returns>
        public static string GetBaseDirectory()
        {
            var scanDirectory = new DirectoryInfo("./");

            var wasabiDeploy = scanDirectory.EnumerateDirectories("WasabiDeploy").FirstOrDefault();
            // If the WasabiDeploy directory is already there.
            if (wasabiDeploy is { })
            {
                scanDirectory = wasabiDeploy;
            }

            PrintSubdirsToConsole(scanDirectory.FullName);
            FileInfo slnFile;
            do
            {
                Console.WriteLine($"Looking for solution: {scanDirectory.FullName}");
                slnFile = scanDirectory.GetFiles("WasabiDeploy.sln", SearchOption.TopDirectoryOnly).FirstOrDefault();

                if (slnFile is { })
                {
                    break;
                }

                scanDirectory = scanDirectory.Parent;
            }
            while (scanDirectory?.Parent is { });

            var rootDirectory = slnFile.Directory.Parent.FullName;
            return rootDirectory;
        }
    }
}
