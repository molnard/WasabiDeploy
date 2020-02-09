﻿using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace WasabiDeploy.Publish
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var workingDirectory = new DirectoryInfo("temp").FullName;
            var cloneDirectory = Path.Combine(workingDirectory, "clone");
            var outputDirectory = Path.Combine(workingDirectory, "output");
            var guiDirectory = Path.Combine(cloneDirectory, "WalletWasabi", "WalletWasabi.Gui");

            var versionPrefix = "1.1.10.2";

#if (!DEBUG)
            IoHelpers.DeleteDirectory(cloneDirectory);
            Directory.CreateDirectory(cloneDirectory);
            await GitTools.CloneAsync("https://github.com/zkSNACKs/WalletWasabi.git", cloneDirectory);
#endif
            // https://docs.microsoft.com/en-us/dotnet/articles/core/rid-catalog
            // BOTTLENECKS:
            // Tor - win-32, linux-32, osx-64
            // .NET Core - win-32, linux-64, osx-64
            // Avalonia - win7-32, linux-64, osx-64
            // We'll only support x64, if someone complains, we can come back to it.
            // For 32 bit Windows there needs to be a lot of WIX configuration to be done.

            var targets = new[]
            {
                (target: "win7-x64" ,outputDir: "win"),
                (target: "linux-x64",outputDir: "lin"),
                (target: "osx-x64",  outputDir: "mac")
            };

            IoHelpers.DeleteDirectory(outputDirectory);
            Directory.CreateDirectory(outputDirectory);

            Environment.SetEnvironmentVariable("DOTNET_CLI_TELEMETRY_OPTOUT", "1");

            foreach (var target in targets.Take(1))
            {
                var outputDir = Path.Combine(outputDirectory, target.outputDir);

                await ProcessTools.StartAsync(
                    "dotnet",
                    $"publish --configuration Release --force --output \"{outputDir}\" --self-contained true --runtime \"{target.target}\" /p:VersionPrefix={versionPrefix} --disable-parallel --no-cache /p:DebugType=none /p:DebugSymbols=false /p:ErrorReport=none /p:DocumentationFile=\"\" /p:Deterministic=true /p:PublishSingleFile=true",
                    guiDirectory);

                ZipFile.CreateFromDirectory(outputDir, $"{outputDir}.zip");
            }
        }
    }
}
