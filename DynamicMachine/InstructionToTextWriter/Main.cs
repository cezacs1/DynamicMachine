using System;
using System.IO;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

class InstructionToFileWriter
{
    static void Main()
    {
        string assemblyPath = "ExternalProgram.dll";

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
                    if (method.Name != "Method1") continue;

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
