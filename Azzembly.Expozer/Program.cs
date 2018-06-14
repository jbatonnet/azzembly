using System;
using System.Collections.Generic;
using System.IO;

using Mono.Cecil;

namespace Azzembly.Expozer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
                return;

            // [0] is source
            // [1] is target directory

            // List assemblies to expose
            List<string> assemblies = new List<string>();

            if (Directory.Exists(args[0]))
            {
                assemblies.AddRange(Directory.GetFiles(args[0], "*.*", SearchOption.AllDirectories));
            }
            else if (File.Exists(args[0]))
            {
                assemblies.Add(Path.GetFullPath(args[0]));
                args[0] = Path.GetDirectoryName(args[0]);
            }
            else
                return;

            if (assemblies.Count == 0)
                return;

            // Expose assemblies
            foreach (string assemblyPath in assemblies)
            {
                DefaultAssemblyResolver assemblyResolver = new DefaultAssemblyResolver();
                assemblyResolver.AddSearchDirectory(Path.GetDirectoryName(assemblyPath));

                string newAssemblyPath = assemblyPath.Replace(args[0], args[1]);
                string newAssemblyDirectory = Path.GetDirectoryName(newAssemblyPath);

                if (!Directory.Exists(newAssemblyDirectory))
                    Directory.CreateDirectory(newAssemblyDirectory);

                if (!assemblyPath.Contains("SourceTree.exe"))
                {
                    if (File.Exists(newAssemblyPath))
                        File.Delete(newAssemblyPath);

                    File.Copy(assemblyPath, newAssemblyPath);

                    continue;
                }

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

                    assembly.Write(newAssemblyPath);
                }
                catch
                {
                    if (File.Exists(newAssemblyPath))
                        File.Delete(newAssemblyPath);

                    File.Copy(assemblyPath, newAssemblyPath);
                }
            }
        }
    }
}
