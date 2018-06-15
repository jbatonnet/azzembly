using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;

namespace Azzembly.Expozer
{
    public class Program
    {
        public static Dictionary<string, string> Options { get; private set; }
        public static List<string> Parameters { get; private set; }

        public static void Main(params string[] args)
        {
            Options = args.Where(a => a.StartsWith("/"))
                          .Select(a => a.TrimStart('/'))
                          .Select(a => new { Parameter = a.Trim(), Separator = a.Trim().IndexOf(':') })
                          .ToDictionary(a => a.Separator == -1 ? a.Parameter : a.Parameter.Substring(0, a.Separator).ToLower(), a => a.Separator == -1 ? null : a.Parameter.Substring(a.Separator + 1), StringComparer.InvariantCultureIgnoreCase);
            Parameters = args.Where(a => !a.StartsWith("/"))
                             .ToList();

            // List assemblies to expose
            List<string> assemblies = new List<string>();

            foreach (string parameter in Parameters)
            {
                if (parameter.Contains("*"))
                {
                    string directory = Path.GetDirectoryName(parameter);
                    string filter = Path.GetFileName(parameter);

                    assemblies.AddRange(Directory.GetFiles(directory, filter, SearchOption.AllDirectories));
                }
                else if (Directory.Exists(parameter))
                {
                    assemblies.AddRange(Directory.GetFiles(parameter, "*.dll", SearchOption.AllDirectories));
                    assemblies.AddRange(Directory.GetFiles(parameter, "*.exe", SearchOption.AllDirectories));
                }
                else if (File.Exists(parameter))
                    assemblies.Add(parameter);
            }

            if (assemblies.Count == 0)
                return;

            // Prepare output
            Options.TryGetValue("output", out string output);
            if (output != null && !Directory.Exists(output))
                Directory.CreateDirectory(output);

            // Expose assemblies
            foreach (string assemblyPath in assemblies)
            {
                DefaultAssemblyResolver assemblyResolver = new DefaultAssemblyResolver();
                assemblyResolver.AddSearchDirectory(Path.GetDirectoryName(assemblyPath));
                
                try
                {
                    AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(assemblyPath, new ReaderParameters() { AssemblyResolver = assemblyResolver });

                    foreach (ModuleDefinition module in assembly.Modules)
                    {
                        foreach (TypeDefinition type in module.Types)
                        {
                            type.IsPublic = true;
                            type.IsSealed = false;

                            foreach (FieldDefinition field in type.Fields)
                                field.IsPublic = true;
                            foreach (MethodDefinition method in type.Methods)
                                method.IsPublic = true;
                        }
                    }

                    string outputAssemblyPath = output == null ? assemblyPath : Path.Combine(output, Path.GetFileName(assemblyPath));
                    assembly.Write(outputAssemblyPath + ".temp");

                    assembly.Dispose();
                    if (File.Exists(outputAssemblyPath))
                        File.Delete(outputAssemblyPath);
                    File.Move(outputAssemblyPath + ".temp", outputAssemblyPath);

                    Console.WriteLine($"File {assemblyPath} succesfully processed");
                }
                catch
                {
                    Console.WriteLine($"File {assemblyPath} could not be loaded. It will be skipped");
                }
            }
        }
    }
}
