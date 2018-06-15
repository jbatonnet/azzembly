extern alias expozer;
extern alias patcher;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Expozer = expozer.Azzembly.Expozer.Program;
using Patcher = patcher.Azzembly.Patcher.Program;

namespace SourceTree.Launcher
{
    class Program
    {
        public static Dictionary<string, string> Options { get; private set; }
        public static List<string> Parameters { get; private set; }

        public static void Main(string[] args)
        {
            Options = args.Where(a => a.StartsWith("/"))
                          .Select(a => a.TrimStart('/'))
                          .Select(a => new { Parameter = a.Trim(), Separator = a.Trim().IndexOf(':') })
                          .ToDictionary(a => a.Separator == -1 ? a.Parameter : a.Parameter.Substring(0, a.Separator).ToLower(), a => a.Separator == -1 ? null : a.Parameter.Substring(a.Separator + 1), StringComparer.InvariantCultureIgnoreCase);
            Parameters = args.Where(a => !a.StartsWith("/"))
                             .ToList();

            string originalSourceTreeDirectory = @"C:\Projects\Tests\SourceTree\app-2.5.5";
            string expozedSourceTreeDirectory = @"C:\Projects\Tests\PublicSourceTree";
            string finalSourceTreeDirectory = @"C:\Projects\Tests\FinalSourceTree";

            string sourceTreeFileName = "SourceTree.exe";
            string sourceTreeApiUIWpfFileName = "SourceTree.Api.UI.Wpf.dll";

            string sourceTreePatchFileName = @"C:\Projects\jbatonnet\Azzembly\SourceTree\bin\Debug\SourceTree.dll";
            string sourceTreeApiUIWpfPatchFileName = @"C:\Projects\jbatonnet\Azzembly\SourceTree.Api.UI.Wpf\bin\Debug\SourceTree.Api.UI.Wpf.dll";

            // Copy fresh assemblies
            File.Copy(Path.Combine(originalSourceTreeDirectory, sourceTreeFileName), Path.Combine(expozedSourceTreeDirectory, sourceTreeFileName), true);
            File.Copy(Path.Combine(originalSourceTreeDirectory, sourceTreeFileName), Path.Combine(finalSourceTreeDirectory, sourceTreeFileName), true);
            File.Copy(Path.Combine(originalSourceTreeDirectory, sourceTreeApiUIWpfFileName), Path.Combine(expozedSourceTreeDirectory, sourceTreeApiUIWpfFileName), true);
            File.Copy(Path.Combine(originalSourceTreeDirectory, sourceTreeApiUIWpfFileName), Path.Combine(finalSourceTreeDirectory, sourceTreeApiUIWpfFileName), true);

            // Expose SourceTree assemblies
            Expozer.Main(Path.Combine(expozedSourceTreeDirectory, sourceTreeFileName));
            Expozer.Main(Path.Combine(expozedSourceTreeDirectory, sourceTreeApiUIWpfFileName));

            // Patch SourceTree assemblies
            Patcher.Main($"/Patch:{sourceTreePatchFileName}", $"/Source:{Path.Combine(originalSourceTreeDirectory, sourceTreeFileName)}", $"/Target:{Path.Combine(finalSourceTreeDirectory, sourceTreeFileName)}");
            Patcher.Main($"/Patch:{sourceTreeApiUIWpfPatchFileName}", $"/Source:{Path.Combine(originalSourceTreeDirectory, sourceTreeApiUIWpfFileName)}", $"/Target:{Path.Combine(finalSourceTreeDirectory, sourceTreeApiUIWpfFileName)}");

            // Run SourceTree
            Environment.CurrentDirectory = finalSourceTreeDirectory;
            Process.Start(Path.Combine(finalSourceTreeDirectory, sourceTreeFileName));
            //AppDomain sourceTreeAppDomain = AppDomain.CreateDomain("SourceTree", null, originalSourceTreeDirectory, originalSourceTreeDirectory, false);
            //sourceTreeAppDomain.ExecuteAssembly(Path.Combine(originalSourceTreeDirectory, sourceTreeFileName));
        }
    }
}
