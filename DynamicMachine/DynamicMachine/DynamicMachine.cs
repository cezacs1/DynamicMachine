using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection.Emit;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Server;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Xml.Linq;

public static class DynamicMachine
{
    [DllImport("ntdll.dll", CharSet = CharSet.Auto)]
    public static extern int NtQueryInformationProcess(IntPtr test, int test2, int[] test3, int test4, ref int test5);

    private static void Initialize()
    {
        if (Debugger.IsLogging())
        { Environment.Exit(0); }
        if (Debugger.IsAttached)
        { Environment.Exit(0); }
        if (Environment.GetEnvironmentVariable("complus_profapi_profilercompatibilitysetting") != null)
        { Environment.Exit(0); }
        if (string.Compare(Environment.GetEnvironmentVariable("COR_ENABLE_PROFILING"), "1", StringComparison.Ordinal) == 0)
        { Environment.Exit(0); }

        if (Environment.OSVersion.Platform != PlatformID.Win32NT) return;
        var array = new int[6];
        var num = 0;
        var intPtr = Process.GetCurrentProcess().Handle;
        if (NtQueryInformationProcess(intPtr, 31, array, 4, ref num) == 0 && array[0] != 1)
        {
            Environment.Exit(0);
        }
        if (NtQueryInformationProcess(intPtr, 30, array, 4, ref num) == 0 && array[0] != 0)
        {
            Environment.Exit(0);
        }

        if (NtQueryInformationProcess(intPtr, 0, array, 24, ref num) != 0) return;
        intPtr = Marshal.ReadIntPtr(Marshal.ReadIntPtr((IntPtr)array[1], 12), 12);
        Marshal.WriteInt32(intPtr, 32, 0);
        var intPtr2 = Marshal.ReadIntPtr(intPtr, 0);
        var ptr = intPtr2;
        do
        {
            ptr = Marshal.ReadIntPtr(ptr, 0);
            if (Marshal.ReadInt32(ptr, 44) != 1572886 ||
                Marshal.ReadInt32(Marshal.ReadIntPtr(ptr, 48), 0) != 7536749) continue;
            var intPtr3 = Marshal.ReadIntPtr(ptr, 8);
            var intPtr4 = Marshal.ReadIntPtr(ptr, 12);
            Marshal.WriteInt32(intPtr4, 0, (int)intPtr3);
            Marshal.WriteInt32(intPtr3, 4, (int)intPtr4);
        }
        while (!ptr.Equals(intPtr2));

        Entry();
    }

    public static void savemodule()
    {
        // Assembly ve Modül oluştur
        AssemblyName assemblyName = new AssemblyName("ExternalProgram1");
        AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
        ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("ExternalModule", "ExternalProgram1.dll");

        // Tip oluştur
        TypeBuilder typeBuilder = moduleBuilder.DefineType("ExternalType", System.Reflection.TypeAttributes.Public);

        // Dinamik yöntemi oluştur
        MethodBuilder methodBuilder = typeBuilder.DefineMethod("DynamicMethod", System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.Static, typeof(void), new Type[] { });

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

    public static void Entry()
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
        TypeBuilder typeBuilder = moduleBuilder.DefineType("DynamicType", System.Reflection.TypeAttributes.Public);

        // Metot oluştur
        MethodBuilder methodBuilder = typeBuilder.DefineMethod("DynamicMethod", System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.Static, typeof(void), null);

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

    static Type lastOperandType = null; // line'in 2. kısmındaki text operand'dır. hangi tür olduğunu işlerken algılarız.

    public static void emitter(MethodBuilder methodBuilder, string[] instructions)
    {
        ILGenerator ilGenerator = methodBuilder.GetILGenerator();      

        foreach (string instruction in instructions)
        {
            OpCode opcode = GetOpCode(instruction);

            Type operandType = Operands.GetOperandType(instruction);
            Console.WriteLine("operandType -> " + operandType);

            if (opcode == OpCodes.Ldstr)
            {
                string operand = Operands.GetOperand_string(instruction);

                ilGenerator.Emit(opcode, operand);
            }
            else if (opcode == OpCodes.Ldc_I4_S)
            {
                int operand = Operands.GetOperand_Int32(instruction);

                ilGenerator.Emit(opcode, operand);
                continue;

                /*
                if (operandType != typeof(int))
                {
                    ilGenerator.Emit(OpCodes.Ldstr, operand.ToString());
                }
                else
                {
                    ilGenerator.Emit(opcode, operand);
                }
                */
            }
            else if (opcode == OpCodes.Call)
            {
                if (instruction.Contains("Console"))
                {
                    // call'i işlerken konsolu çağır
                    MethodInfo writeLineMethod = typeof(Console).GetMethod("WriteLine", new Type[] { operandType });
                    ilGenerator.EmitCall(OpCodes.Call, writeLineMethod, null);
                }
                else
                {
                    //string instruction = "call System.Void ExternalProgram.Class1::Method2()";

                    // Komut dizesini işlemek için düzenli ifade kullanımı
                    Regex regex = new Regex(@"call\s+(.*?)\s+(.*?)::(.*?)\(\)");
                    Match match = regex.Match(instruction);

                    if (match.Success)
                    {
                        string returnTypeStr = match.Groups[1].Value;
                        string namespaceTypeStr = match.Groups[2].Value;
                        string methodName = match.Groups[3].Value;

                        Console.WriteLine("[!] Return Type     -> " + returnTypeStr);
                        Console.WriteLine("[!] Namespace.Type  -> " + namespaceTypeStr);
                        Console.WriteLine("[!] Method Name     -> " + methodName);

                        // bu kod ExternalProgram.dll içindeki yöntemi çağırmak içindi.
                        // MethodInfo newMethod = typeof(Class1).GetMethod("Method2");


                        // Bu ise current program için. Korunacak programa inject olduğu için hedef programın kendisi oluyor.
                        // Çalışma zamanında kendi içindeki yöntemleri arayan (ör: test.exe) doğru yöntemi buluyor.
                        Type type = Type.GetType(namespaceTypeStr);
                        MethodInfo newMethod = Type.GetType(namespaceTypeStr)?.GetMethod(methodName);

                        ilGenerator.EmitCall(OpCodes.Call, newMethod, null);
                    }
                    else
                    {
                        Console.WriteLine("Komut dizesi işlenemedi.");
                    }
                }
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

                // bu ikisi tam bir orospu cocugu nopluyorum amk
            case "stloc.0":
                return OpCodes.Nop;
            case "ldloc.0":
                return OpCodes.Nop;
            case "stloc.1":
                return OpCodes.Nop;
            case "ldloc.1":
                return OpCodes.Nop;


            case "call":
                return OpCodes.Call;
            case "ldarg.0":
                return OpCodes.Ldarg_0;
            case "ldc.i4.s":
                return OpCodes.Ldc_I4_S;
            case "pop":
                return OpCodes.Pop;
            case "ldc.i4.1":
                return OpCodes.Ldc_I4_1;
            case "ldc.i4.2":
                return OpCodes.Ldc_I4_2;
            case "ldc.i4.3":
                return OpCodes.Ldc_I4_3;
            case "ldc.i4.4":
                return OpCodes.Ldc_I4_4;
            case "ldc.i4.5":
                return OpCodes.Ldc_I4_5;
            case "ldc.i4.6":
                return OpCodes.Ldc_I4_6;
            case "ldc.i4.7":
                return OpCodes.Ldc_I4_7;
            case "ldc.i4.8":
                return OpCodes.Ldc_I4_8;
            case "ldc.i4.0":
                return OpCodes.Ldc_I4_0;


            case "ret":
                return OpCodes.Ret;

            default:
                throw new Exception("invalid opcode -> " + instruction);
        }
    }

    public static string[] ReadInstructionsFromFile(string filePath)
    {
        // ins.txt, InstructionToTextWriter projesinde içine instructions yazdığımız dosyadır.
        // Onun tüm satırlarını okuyup yöntemi dinamik olarak çalıştırırız.
        // İlerde instruction'ların farklı şekilde alınması üzerine çalışabilirim.
        // Ve mutlaka dosyaya yazılan kodlar şifrelenmelidir. Ben biraz acele yaptım.

        return File.ReadAllLines(filePath);
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class CEZA : Attribute
    {
        public string Value1 { get; }
        /*

        public int Value2 { get; }

        public int Value3 { get; }
        public int Value4 { get; }
        */

        public CEZA(string value1/*, int value2, int value3, int value4*/)
        {
            Value1 = value1;
            /*
            Value2 = value2;

            Value3 = value3;
            Value4 = value4;
            */
        }
    }

    public class Operands
    {
        public static Type GetOperandType(string instruction)
        {
            // İlgili komutun sonundaki türü al
            //int typeIndex = instruction.LastIndexOf(" ") + 1;
            //string typeStr = instruction.Substring(typeIndex).Trim();

            // Türü doğru bir .NET Type nesnesine dönüştür
            string instruc = instruction.ToLower();

            if (instruc.Contains("system.string"))
            {
                return typeof(string);
            }
            if (instruc.Contains("system.ınt16"))
            {
                return typeof(Int16);
            }
            if (instruc.Contains("system.ınt32"))
            {
                return typeof(Int32);
            }
            if (instruc.Contains("system.ınt64"))
            {
                return typeof(Int64);
            }
            if (instruc.Contains("system.uınt16"))
            {
                return typeof(UInt16);
            }
            if (instruc.Contains("system.uınt32"))
            {
                return typeof(UInt32);
            }
            if (instruc.Contains("system.uınt64"))
            {
                return typeof(UInt64);
            }
            if (instruc.Contains("system.boolean"))
            {
                return typeof(bool);
            }

            return typeof(string);
        }

        public static string GetOperand_string(string instruction)
        {
            // İnstruction'ın sağ tarafındaki stringi ayıkla
            int startIndex = instruction.IndexOf(" ") + 1;
            string operand = instruction.Substring(startIndex).Trim();

            return operand;
        }

        public static Int32 GetOperand_Int32(string instruction)
        {
            // İnstruction'ın sağ tarafındaki stringi ayıkla
            int startIndex = instruction.IndexOf(" ") + 1;
            Int32 operand = Convert.ToInt32(instruction.Substring(startIndex).Trim());

            return operand;
        }
    }
}

