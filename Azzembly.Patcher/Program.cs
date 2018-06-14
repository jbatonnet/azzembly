using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Azzembly.Patcher
{
    class Program
    {
        static void Main(string[] args)
        {
            string patchAssemblyPath = @"C:\Projects\jbatonnet\Azzembly\SourceTree\bin\Debug\SourceTree.dll";
            string sourceAssemblyPath = @"C:\Projects\Tests\SourceTree\app-2.5.5\SourceTree.exe";
            string targetAssemblyPath = @"C:\Projects\Tests\FinalSourceTree\SourceTree.exe";

            DefaultAssemblyResolver assemblyResolver = new DefaultAssemblyResolver();
            assemblyResolver.AddSearchDirectory(Path.GetDirectoryName(sourceAssemblyPath));

            AssemblyDefinition patchAssembly = AssemblyDefinition.ReadAssembly(patchAssemblyPath, new ReaderParameters() { AssemblyResolver = assemblyResolver });
            AssemblyDefinition sourceAssembly = AssemblyDefinition.ReadAssembly(sourceAssemblyPath, new ReaderParameters() { AssemblyResolver = assemblyResolver });

            string patchAttributeFullName = typeof(PatchAttribute).FullName;

            foreach (TypeDefinition patchType in patchAssembly.MainModule.Types)
            {
                CustomAttribute patchAttribute = patchType.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == patchAttributeFullName);

                if (patchAttribute == null)
                {
                    //sourceAssembly.MainModule.Types.Add(type);
                }
                else
                {
                    CustomAttributeArgument patchAttributeArgument = patchAttribute.ConstructorArguments[0];
                    TypeReference sourceTypeReference = patchAttributeArgument.Value as TypeReference;

                    TypeDefinition sourceType = sourceTypeReference.Resolve();
                    TypeDefinition targetType = sourceAssembly.MainModule.Types.FirstOrDefault(t => t.FullName == sourceType.FullName);

                    foreach (MethodDefinition patchMethod in patchType.Methods)
                    {
                        if (patchMethod.Name.Contains("ctor"))
                            continue;

                        MethodDefinition targetMethod = targetType.Methods.FirstOrDefault(m => m.Name == patchMethod.Name);
                        targetMethod.Body = patchMethod.Body;

                        foreach (VariableDefinition variable in targetMethod.Body.Variables)
                        {
                            variable.VariableType = sourceAssembly.MainModule.ImportReference(variable.VariableType);
                        }

                        foreach (Instruction instruction in targetMethod.Body.Instructions)
                        {
                            if (instruction.Operand is TypeReference typeReference)
                                instruction.Operand = sourceAssembly.MainModule.ImportReference(typeReference);
                            else if (instruction.Operand is MethodReference methodReference)
                                instruction.Operand = sourceAssembly.MainModule.ImportReference(methodReference);
                            else if (instruction.Operand is FieldReference fieldReference)
                                instruction.Operand = sourceAssembly.MainModule.ImportReference(fieldReference);
                        }

                        /*foreach (ExceptionHandler exceptionHandler in patchMethod.Body.ExceptionHandlers)
                        {
                            if (exceptionHandler.CatchType != null)
                                exceptionHandler.CatchType = sourceAssembly.MainModule.ImportReference(exceptionHandler.CatchType);
                        }*/
                    }
                }
            }

            sourceAssembly.Write(targetAssemblyPath);
        }
    }
}
