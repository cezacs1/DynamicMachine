using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Remoting.Contexts;
using System.Xml.Linq;

class Program
{
    static void savemodule()
    {
        // Assembly ve Modül oluştur
        AssemblyName assemblyName = new AssemblyName("ExternalProgram1");
        AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
        ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("ExternalModule", "ExternalProgram1.dll");

        // Tip oluştur
        TypeBuilder typeBuilder = moduleBuilder.DefineType("ExternalType", TypeAttributes.Public);

        // Dinamik yöntemi oluştur
        MethodBuilder methodBuilder = typeBuilder.DefineMethod("DynamicMethod", MethodAttributes.Public | MethodAttributes.Static, typeof(void), new Type[] { });

        // Özel nitelikleri yönteme uygula
        methodBuilder.SetCustomAttribute(new CustomAttributeBuilder(
            typeof(CEZA).GetConstructor(new Type[] { typeof(string)/*, typeof(int), typeof(int), typeof(int)*/ }),
            new object[] { "C E Z A#6466"/*, 1, 2, new Random().Next(0, 9999999)*/ }
        ));

        // Get IL instructions from a file
        string[] instructions = ReadInstructionsFromFile("ins.txt");

        emitter(methodBuilder, instructions);

        // Tipi tamamla ve kullanılabilir hale getir
        Type externalType = typeBuilder.CreateType();

        // ExternalProgram.dll'yi kaydet
        assemblyBuilder.Save("ExternalProgram1.dll");

        // Oluşturulan dinamik yöntemi çağır
        MethodInfo dynamicMethod = externalType.GetMethod("DynamicMethod");
        dynamicMethod.Invoke(null, null);

        Console.WriteLine("Ended.");
        Console.ReadKey();
        Environment.Exit(0);
    }

    static void Main()
    {
        // savemodule yöntemini sadece kodu test ederken kullanın.
        // Zaten programın amacı runtime işlenen mekanizma ile orjinal kodu korumaktır.
        // Program tamamen bittiğinde açığı çok az olacaktır. (dumplanamaz / cracklenmesi çok zor)
        savemodule();

        // assembly ve modül oluştur
        AssemblyName assemblyName = new AssemblyName("DynamicAssembly");
        AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");

        // tip oluştur
        TypeBuilder typeBuilder = moduleBuilder.DefineType("DynamicType", TypeAttributes.Public);

        // Metot oluştur
        MethodBuilder methodBuilder = typeBuilder.DefineMethod("DynamicMethod", MethodAttributes.Public | MethodAttributes.Static, typeof(void), null);

        // Özel nitelikleri yönteme uygula
        methodBuilder.SetCustomAttribute(new CustomAttributeBuilder(
            typeof(CEZA).GetConstructor(new Type[] { typeof(string)/*, typeof(int), typeof(int), typeof(int)*/ }),
            new object[] { "C E Z A#6466"/*, 1, 2, new Random().Next(0, 9999999)*/ }
        ));

        // il talimatlarını, önceden girilen bir dosyadan al.
        string[] instructions = ReadInstructionsFromFile("ins.txt");

        emitter(methodBuilder, instructions);

        // tipi tamamla ve kullanılabilir hale getir
        Type dynamicType = typeBuilder.CreateType();

        // metodu / yöntemi çağır
        MethodInfo dynamicMethod = dynamicType.GetMethod("DynamicMethod");
        dynamicMethod.Invoke(null, null);

        Console.WriteLine("Enter to exit");
        Console.ReadKey();
    }

    static void emitter(MethodBuilder methodBuilder, string[] instructions)
    {

        ILGenerator ilGenerator = methodBuilder.GetILGenerator();
        Type lastOperandType = null; // line'in 2. kısmındaki text operand'dır. hangi tür olduğunu işlerken algılarız.

        foreach (string instruction in instructions)
        {
            OpCode opcode = GetOpCode(instruction);

            Type operandType = Operands.GetOperandType(instruction);

            if (opcode == OpCodes.Ldstr)
            {
                string operand = Operands.GetOperand_string(instruction);

                ilGenerator.Emit(opcode, operand);
            }
            else if (opcode == OpCodes.Ldc_I4_S)
            {
                int operand = Operands.GetOperand_Int32(instruction);

                if (operandType != typeof(int))
                {
                    ilGenerator.Emit(OpCodes.Ldstr, operand.ToString());
                }
                else
                {
                    ilGenerator.Emit(opcode, operand);
                }
            }
            else if (opcode == OpCodes.Call)
            {
                // call'i işlerken konsolu çağır
                MethodInfo writeLineMethod = typeof(Console).GetMethod("WriteLine", new Type[] { operandType });
                ilGenerator.EmitCall(OpCodes.Call, writeLineMethod, null);
            }
            else
            {
                // single kodlar
                ilGenerator.Emit(opcode);
            }

            // Son işlenen operand türünü güncelle
            lastOperandType = operandType;
        }
    }

    public static OpCode GetOpCode(string instruction)
    {
        // metoda göre doğru OpCode'u al
        switch (instruction.Split(' ')[0].ToLower())
        {
            case "add":
                return OpCodes.Add;
            case "sub":
                return OpCodes.Sub;
            case "nop":
                return OpCodes.Nop;
            case "ldstr":
                return OpCodes.Ldstr;
            case "stloc.0":
                return OpCodes.Stloc_0;
            case "ldloc.0":
                return OpCodes.Ldloc_0;
            case "call":
                return OpCodes.Call;
            case "ldarg.0":
                return OpCodes.Ldarg_0;
            case "ldc.i4.s":
                return OpCodes.Ldc_I4_S;
            case "ret":
                return OpCodes.Ret;

            default:
                throw new Exception("invalid opcode -> " + instruction);
        }
    }

    static string[] ReadInstructionsFromFile(string filePath)
    {
        // ins.txt, InstructionToTextWriter projesinde içine instructions yazdığımız dosyadır.
        // Onun tüm satırlarını okuyup yöntemi dinamik olarak çalıştırırız.
        // İlerde instruction'ların farklı şekilde alınması üzerine çalışabilirim.
        // Ve mutlaka dosyaya yazılan kodlar şifrelenmelidir. Ben biraz acele yaptım.

        return File.ReadAllLines(filePath);
    }
}