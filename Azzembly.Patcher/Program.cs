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

            Options.TryGetValue("patch", out string patchAssemblyPath);
            Options.TryGetValue("source", out string sourceAssemblyPath);
            Options.TryGetValue("target", out string targetAssemblyPath);

            if (!File.Exists(patchAssemblyPath))
                return;
            if (!File.Exists(sourceAssemblyPath))
                return;

            if (targetAssemblyPath == null)
                targetAssemblyPath = sourceAssemblyPath;

            //string patchAssemblyPath = @"C:\Projects\jbatonnet\Azzembly\SourceTree\bin\Debug\SourceTree.dll";
            //string sourceAssemblyPath = @"C:\Projects\Tests\SourceTree\app-2.5.5\SourceTree.exe";
            //string targetAssemblyPath = @"C:\Projects\Tests\FinalSourceTree\SourceTree.exe";

            DefaultAssemblyResolver assemblyResolver = new DefaultAssemblyResolver();
            assemblyResolver.AddSearchDirectory(Path.GetDirectoryName(sourceAssemblyPath));

            AssemblyDefinition patchAssembly = AssemblyDefinition.ReadAssembly(patchAssemblyPath, new ReaderParameters() { AssemblyResolver = assemblyResolver, ReadSymbols = true });
            AssemblyDefinition sourceAssembly = AssemblyDefinition.ReadAssembly(sourceAssemblyPath, new ReaderParameters() { AssemblyResolver = assemblyResolver });

            string patchAttributeFullName = typeof(PatchAttribute).FullName;

            Dictionary<TypeDefinition, TypeDefinition> newTypesMapping = new Dictionary<TypeDefinition, TypeDefinition>();
            Dictionary<FieldDefinition, FieldDefinition> newFieldsMapping = new Dictionary<FieldDefinition, FieldDefinition>();
            Dictionary<MethodDefinition, MethodDefinition> newMethodsMapping = new Dictionary<MethodDefinition, MethodDefinition>();

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

                    PatchType(sourceAssembly.MainModule, patchType, targetType);
                }
            }

            sourceAssembly.Write(targetAssemblyPath + ".temp", new WriterParameters()
            {
                SymbolWriterProvider = new Mono.Cecil.Pdb.PdbWriterProvider(),
                WriteSymbols = true
            });

            sourceAssembly.Dispose();
            patchAssembly.Dispose();
            assemblyResolver.Dispose();

            if (File.Exists(targetAssemblyPath))
                File.Delete(targetAssemblyPath);
            File.Move(targetAssemblyPath + ".temp", targetAssemblyPath);
        }

        private static void PatchType(ModuleDefinition module, TypeDefinition patchType, TypeDefinition targetType)
        {
            Dictionary<TypeDefinition, TypeDefinition> newTypesMapping = new Dictionary<TypeDefinition, TypeDefinition>();
            Dictionary<FieldDefinition, FieldDefinition> newFieldsMapping = new Dictionary<FieldDefinition, FieldDefinition>();
            Dictionary<MethodDefinition, MethodDefinition> newMethodsMapping = new Dictionary<MethodDefinition, MethodDefinition>();

            // Create all missing nested types
            foreach (TypeDefinition patchNestedType in patchType.NestedTypes)
            {
                TypeDefinition targetNestedType = targetType.NestedTypes.FirstOrDefault(m => m.Name == patchNestedType.Name);

                if (targetNestedType != null)
                {
                    PatchType(module, patchNestedType, targetNestedType);

                    foreach (FieldDefinition patchField in patchNestedType.Fields)
                    {
                        FieldDefinition targetField = targetNestedType.Fields.FirstOrDefault(m => m.Name == patchField.Name);
                        newFieldsMapping.Add(patchField, targetField);
                    }

                    foreach (MethodDefinition patchMethod in patchNestedType.Methods)
                    {
                        MethodDefinition targetMethod = targetNestedType.Methods.FirstOrDefault(m => m.Name == patchMethod.Name);
                        newMethodsMapping.Add(patchMethod, targetMethod);
                    }
                }
                else
                {
                    //targetNestedType = new TypeDefinition(patchField.Name, patchField.Attributes, patchField.FieldType);
                    targetType.NestedTypes.Add(patchNestedType);

                    //newFieldsMapping.Add(patchNestedType, targetNestedType);
                }
            }

            // Create all missing fields
            foreach (FieldDefinition patchField in patchType.Fields)
            {
                FieldDefinition targetField = targetType.Fields.FirstOrDefault(m => m.Name == patchField.Name);
                if (targetField != null)
                    continue;

                TypeReference fieldType = module.ImportReference(patchField.FieldType);

                targetField = new FieldDefinition(patchField.Name, patchField.Attributes, fieldType);
                targetType.Fields.Add(targetField);

                newFieldsMapping.Add(patchField, targetField);
            }

            // Create all missing methods
            foreach (MethodDefinition patchMethod in patchType.Methods)
            {
                if (patchMethod.Name.Contains("ctor"))
                    continue;

                MethodDefinition targetMethod = targetType.Methods.FirstOrDefault(m => m.Name == patchMethod.Name);
                if (targetMethod == null)
                {
                    TypeReference returnType = module.ImportReference(patchMethod.ReturnType);

                    targetMethod = new MethodDefinition(patchMethod.Name, patchMethod.Attributes, returnType);
                    foreach (ParameterDefinition parameter in patchMethod.Parameters)
                        targetMethod.Parameters.Add(parameter);

                    targetType.Methods.Add(targetMethod);
                }

                newMethodsMapping.Add(patchMethod, targetMethod);
            }

            // Replace method bodies
            foreach (MethodDefinition patchMethod in patchType.Methods)
            {
                if (patchMethod.Name.Contains("ctor"))
                    continue;

                MethodDefinition targetMethod = targetType.Methods.FirstOrDefault(m => m.Name == patchMethod.Name);
                targetMethod.Body = patchMethod.Body;

                // Copy debug information
                targetMethod.DebugInformation.Scope = patchMethod.DebugInformation.Scope;
                targetMethod.DebugInformation.StateMachineKickOffMethod = patchMethod.DebugInformation.StateMachineKickOffMethod;
                targetMethod.DebugInformation.MetadataToken = patchMethod.DebugInformation.MetadataToken;

                targetMethod.DebugInformation.SequencePoints.Clear();
                foreach (SequencePoint sequencePoint in patchMethod.DebugInformation.SequencePoints)
                    targetMethod.DebugInformation.SequencePoints.Add(sequencePoint);

                // Patch variables and instructions
                foreach (VariableDefinition variable in targetMethod.Body.Variables)
                {
                    variable.VariableType = module.ImportReference(variable.VariableType);
                }
                
                foreach (Instruction instruction in targetMethod.Body.Instructions)
                {
                    if (instruction.Operand is TypeReference typeReference)
                        instruction.Operand = module.ImportReference(typeReference);
                    else if (instruction.Operand is FieldReference fieldReference)
                    {
                        FieldDefinition fieldDefinition = fieldReference.Resolve();
                        if (newFieldsMapping.TryGetValue(fieldDefinition, out FieldDefinition mappedFieldDefinition))
                            fieldDefinition = mappedFieldDefinition;

                        instruction.Operand = module.ImportReference(fieldDefinition);
                    }
                    else if (instruction.Operand is MethodReference methodReference)
                    {
                        if (methodReference.Module != module)
                        {
                            MethodDefinition methodDefinition = methodReference.Resolve();
                            if (newMethodsMapping.TryGetValue(methodDefinition, out MethodDefinition mappedMethodDefinition))
                                methodReference = mappedMethodDefinition;
                        }

                        instruction.Operand = module.ImportReference(methodReference);
                    }
                }

                foreach (ExceptionHandler exceptionHandler in patchMethod.Body.ExceptionHandlers)
                {
                    if (exceptionHandler.CatchType != null)
                        exceptionHandler.CatchType = module.ImportReference(exceptionHandler.CatchType);
                }
            }
        }
    }
}
