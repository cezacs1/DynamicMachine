using System;
using System.Linq;
using System.Text.RegularExpressions;
using dnlib;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using DynamicMachine;
using Microsoft.SqlServer.Server;

namespace DynamicMachine
{
    static class RenameProtector
    {
        public static void Execute(ModuleDef module)
        {
            module.Name = Undetected.RandomString();
            foreach (TypeDef type in module.Types)
            {
                if (type.IsGlobalModuleType || type.IsRuntimeSpecialName || type.IsSpecialName || type.IsWindowsRuntime || type.IsInterface)
                    continue;
                else
                {

                    type.Name = /*RandomString(40)*/Undetected.RandomStringStrong(40);
                    type.Namespace = "";

                    foreach (PropertyDef property in type.Properties)
                    {
                        property.Name = Undetected.RandomStringStrong(40);
                    }


                    foreach (FieldDef fields in type.Fields)
                    {
                        fields.Name = Undetected.RandomStringStrong(40);
                    }

                    foreach (EventDef eventdef in type.Events)
                    {
                        eventdef.Name = Undetected.RandomStringStrong(40);
                    }

                    foreach (MethodDef method in type.Methods)
                    {
                        if (method.Name.ToLower().Contains("main")) continue; //DONT TOUCH THIS IS IMPORTANT
                        if (method.IsConstructor) continue;
                        method.Name = Undetected.RandomStringStrong(40);
                    }

                    foreach (var method in type.Methods.ToArray())
                    {
                        MethodDef strings = CreateReturnMethodDef(RandomString2(), method);
                        MethodDef ints = CreateReturnMethodDef(RandomInt(), method);
                        type.Methods.Add(strings);
                        type.Methods.Add(ints);
                    }
                }
            }
            Console.WriteLine("Renamer done.");
        }

        private static MethodDef CreateReturnMethodDef(object value, MethodDef source_method)
        {
            CorLibTypeSig corlib = null;

            if (value is int)
                corlib = source_method.Module.CorLibTypes.Int32;
            else if (value is float)
                corlib = source_method.Module.CorLibTypes.Single;
            else if (value is string)
                corlib = source_method.Module.CorLibTypes.String;
            MethodDef newMethod = new MethodDefUser(Undetected.RandomStringStrong(40),
                    MethodSig.CreateStatic(corlib),
                    MethodImplAttributes.IL | MethodImplAttributes.Managed,
                    MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig)
            {
                Body = new CilBody()
            };
            if (value is int)
                newMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, (int)value));
            else if (value is float)
                newMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldc_R4, (double)value));
            else if (value is string)
                newMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldstr, (string)value));
            newMethod.Body.Instructions.Add(new Instruction(OpCodes.Ret));
            return newMethod;
        }


        private static Random random = new Random(); //STRONG RENAMER CANNOT BE DECOMPILED BY "DE4DOT"
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static int RandomInt()
        {
            var ints = Convert.ToInt32(Regex.Match(Guid.NewGuid().ToString(), @"\d+").Value);
            return new Random(ints).Next(0, 99999999);
        }

        public static string RandomString2()
        {
            const string chars = "ABCD1234";
            return new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[new Random(Guid.NewGuid().GetHashCode()).Next(s.Length)]).ToArray());
        }

    }
}