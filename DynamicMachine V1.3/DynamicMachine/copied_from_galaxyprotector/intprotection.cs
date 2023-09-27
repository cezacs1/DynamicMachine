﻿using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;

namespace DynamicMachine
{
    public static class IntConfusion
    {
        public static void Execute(ModuleDef md)
        {
            foreach (var type in md.GetTypes())
            {
                if (type.IsGlobalModuleType) continue;
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody) continue;
                    {
                        for (var i = 0; i < method.Body.Instructions.Count; i++)
                        {
                            if (!method.Body.Instructions[i].IsLdcI4()) continue;
                            var numorig = new Random(Guid.NewGuid().GetHashCode()).Next();
                            var div = new Random(Guid.NewGuid().GetHashCode()).Next();
                            var num = numorig ^ div;

                            var nop = OpCodes.Nop.ToInstruction();

                            var local = new Local(method.Module.ImportAsTypeSig(typeof(int)));
                            method.Body.Variables.Add(local);


                            method.Body.Instructions.Insert(i + 1, OpCodes.Stloc.ToInstruction(local));
                            method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Ldc_I4, method.Body.Instructions[i].GetLdcI4Value() - sizeof(float)));
                            method.Body.Instructions.Insert(i + 3, Instruction.Create(OpCodes.Ldc_I4, num));
                            method.Body.Instructions.Insert(i + 4, Instruction.Create(OpCodes.Ldc_I4, div));
                            method.Body.Instructions.Insert(i + 5, Instruction.Create(OpCodes.Xor));
                            method.Body.Instructions.Insert(i + 6, Instruction.Create(OpCodes.Ldc_I4, numorig));
                            method.Body.Instructions.Insert(i + 7, Instruction.Create(OpCodes.Bne_Un, nop));
                            method.Body.Instructions.Insert(i + 8, Instruction.Create(OpCodes.Ldc_I4, 2));
                            method.Body.Instructions.Insert(i + 9, OpCodes.Stloc.ToInstruction(local));
                            method.Body.Instructions.Insert(i + 10, Instruction.Create(OpCodes.Sizeof, method.Module.Import(typeof(float))));
                            method.Body.Instructions.Insert(i + 11, Instruction.Create(OpCodes.Add));
                            method.Body.Instructions.Insert(i + 12, nop);
                            method.Body.Instructions.Insert(i + 13, OpCodes.Stloc.ToInstruction(local));
                            method.Body.Instructions.Insert(i + 14, Instruction.Create(OpCodes.Ldc_I4, method.Body.Instructions[i].GetLdcI4Value() - sizeof(float)));
                            method.Body.Instructions.Insert(i + 15, Instruction.Create(OpCodes.Ldc_I4, num));
                            method.Body.Instructions.Insert(i + 16, Instruction.Create(OpCodes.Ldc_I4, div));
                            method.Body.Instructions.Insert(i + 17, Instruction.Create(OpCodes.Xor));
                            method.Body.Instructions.Insert(i + 18, Instruction.Create(OpCodes.Ldc_I4, numorig));
                            method.Body.Instructions.Insert(i + 19, Instruction.Create(OpCodes.Bne_Un, nop));
                            method.Body.Instructions.Insert(i + 20, Instruction.Create(OpCodes.Ldc_I4, 2));
                            method.Body.Instructions.Insert(i + 21, OpCodes.Stloc.ToInstruction(local));
                            method.Body.Instructions.Insert(i + 22, Instruction.Create(OpCodes.Sizeof, method.Module.Import(typeof(float))));
                            method.Body.Instructions.Insert(i + 23, Instruction.Create(OpCodes.Add));
                            method.Body.Instructions.Insert(i + 24, nop);
                            method.Body.Instructions.Insert(i + 25, OpCodes.Stloc.ToInstruction(local));
                            method.Body.Instructions.Insert(i + 26, Instruction.Create(OpCodes.Ldc_I4, method.Body.Instructions[i].GetLdcI4Value() - sizeof(float)));
                            method.Body.Instructions.Insert(i + 27, Instruction.Create(OpCodes.Ldc_I4, num));
                            method.Body.Instructions.Insert(i + 28, Instruction.Create(OpCodes.Ldc_I4, div));
                            method.Body.Instructions.Insert(i + 29, Instruction.Create(OpCodes.Xor));
                            method.Body.Instructions.Insert(i + 30, Instruction.Create(OpCodes.Ldc_I4, numorig));
                            method.Body.Instructions.Insert(i + 31, Instruction.Create(OpCodes.Bne_Un, nop));
                            method.Body.Instructions.Insert(i + 32, Instruction.Create(OpCodes.Ldc_I4, 2));
                            method.Body.Instructions.Insert(i + 33, OpCodes.Stloc.ToInstruction(local));
                            method.Body.Instructions.Insert(i + 34, Instruction.Create(OpCodes.Sizeof, method.Module.Import(typeof(float))));
                            method.Body.Instructions.Insert(i + 35, Instruction.Create(OpCodes.Add));
                            method.Body.Instructions.Insert(i + 36, nop);

                            i += 36;
                        }
                        method.Body.SimplifyBranches();
                    }
                }
            }
            Console.WriteLine("Ints obfuscated.");
            //Form1.Print("Int Confusion Done!");
        }
    }
}
