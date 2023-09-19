using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using dnlib.W32Resources;
using Microsoft.SqlServer.Server;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using OpCode = dnlib.DotNet.Emit.OpCode;

class Program
{
    static void Main(string[] args)
    {
        string file_to_obf = /*"test.exe"*/args[0];

        InstructionToFileWriter.WriterMain(file_to_obf);

        ModuleContext modCtx = ModuleDef.CreateModuleContext();
        var module = ModuleDefMD.Load(file_to_obf, modCtx);

        //Execute(module);
        ExecuteDynamicMachine(module, File.ReadAllText("ins.txt"));

        //save
        Console.WriteLine("Trying to save to file.");
        if (file_to_obf.EndsWith(".exe"))
        {
            var path = file_to_obf.Remove(file_to_obf.Length - 4) + ".virtualized.exe";
            module.Write(path, new ModuleWriterOptions(module) { Logger = DummyLogger.NoThrowInstance });
            Console.WriteLine("Done!");
        }


        Console.ReadKey();
    }

    public static void ExecuteDynamicMachine(ModuleDef module, string resourcetexts)
    {
        var typeModule = ModuleDefMD.Load(typeof(DynamicMachine).Module);
        var cctor = module.GlobalType.FindOrCreateStaticConstructor();
        var typeDef = typeModule.ResolveTypeDef(MDToken.ToRID(typeof(DynamicMachine).MetadataToken));
        var members = InjectHelper.Inject(typeDef, module.GlobalType, module);
        var init = (MethodDef)members.Single(method => method.Name == "Initialize");
        cctor.Body.Instructions.Insert(0, dnlib.DotNet.Emit.Instruction.Create(dnlib.DotNet.Emit.OpCodes.Call, init));
        foreach (var md in module.GlobalType.Methods)
        {
            if (md.Name != ".ctor") continue;
            module.GlobalType.Remove(md);
            break;
        }
        Console.WriteLine("Dynamic Machine added!");


        string amk = /*"amk"*/resourcetexts;
        byte[] stringBytes = System.Text.Encoding.UTF8.GetBytes(amk);

        var resource = new EmbeddedResource("testres", stringBytes, ManifestResourceAttributes.Public);
        module.Resources.Add(resource);

        Console.WriteLine("Resource added!");
    }
}

public class InstructionToFileWriter
{
    public static void WriterMain(string file)
    {
        string assemblyPath = file;

        ModuleDefMD module = ModuleDefMD.Load(assemblyPath);

        string dosyaYolu = "ins.txt";

        try
        {
            // önceden oluşturulmuş dosyayı sil
            File.Delete(dosyaYolu);
        }
        catch (Exception ex)
        {
            Console.WriteLine(dosyaYolu + " silinemedi :(\n" + ex);
        }

        using (StreamWriter writer = new StreamWriter(dosyaYolu, true))
        {
            foreach (TypeDef type in module.GetTypes())
            {
                foreach (MethodDef method in type.Methods)
                {
                    if (method.Name != "Main") continue;

                    if (method.HasBody)
                    {
                        Console.WriteLine($"Method: {type.FullName}.{method.Name}");

                        foreach (Instruction instruction in method.Body.Instructions)
                        {
                            OpCode opcode = instruction.OpCode;
                            string operand = instruction.Operand != null ? instruction.Operand.ToString() : "";

                            string line = operand.Length > 0 ? $"{opcode} {operand}" : opcode.ToString();

                            /*
                            if (line.Contains("ldstr"))
                            {
                                line += " string";
                            }
                            if (line.Contains("ldc.i4.s"))
                            {
                                line += " int32";
                            }
                            */

                            Console.WriteLine(line);
                            writer.WriteLine(line);
                        }
                    }
                }
            }
        }

        Console.ReadKey();

    }
}
