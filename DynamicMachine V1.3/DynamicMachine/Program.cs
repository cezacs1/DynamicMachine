using dnlib.DotNet;
using dnlib.DotNet.Writer;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using CustomAttribute = Mono.Cecil.CustomAttribute;
using ExceptionHandler = Mono.Cecil.Cil.ExceptionHandler;
using MethodBody = Mono.Cecil.Cil.MethodBody;
using OpCodes = Mono.Cecil.Cil.OpCodes;

namespace DynamicMachine
{
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

    public class Program
    {
        public static ModuleDefinition Module = null;
        public static Dictionary<string, MethodReference> MetRef = new Dictionary<string, MethodReference>();
        public static Dictionary<string, TypeReference> TypeRef = new Dictionary<string, TypeReference>();

        static void Main()
        {
            string assembly = "testapp.exe";

            ModuleContext modCtx = ModuleDef.CreateModuleContext();
            var module = ModuleDefMD.Load(assembly, modCtx);
            RenameProtector.Execute(module);
            Console.WriteLine("Saving for renamer..");
            if (assembly.EndsWith(".exe"))
            {
                string path = assembly.Remove(assembly.Length - 4) + ".renamed.exe";
                module.Write(path, new ModuleWriterOptions(module)
                {
                    Logger = DummyLogger.NoThrowInstance
                });

                Console.WriteLine("Done.");
            }

            assembly = "testapp.renamed.exe";

            Module = ModuleDefinition.ReadModule(assembly);

            MetRef.Add("FinallyBlock", Module.ImportReference(typeof(System.Reflection.Emit.ILGenerator).GetMethod("BeginFinallyBlock", new Type[0])));
            MetRef.Add("CatchBlock", Module.ImportReference(typeof(System.Reflection.Emit.ILGenerator).GetMethod("BeginCatchBlock", new Type[] { typeof(Type) })));
            MetRef.Add("TryEnd", Module.ImportReference(typeof(System.Reflection.Emit.ILGenerator).GetMethod("EndExceptionBlock", new Type[0])));
            MetRef.Add("TryStart", Module.ImportReference(typeof(System.Reflection.Emit.ILGenerator).GetMethod("BeginExceptionBlock", new Type[0])));
            MetRef.Add("DeclareLocal", Module.ImportReference(typeof(System.Reflection.Emit.ILGenerator).GetMethod("DeclareLocal", new Type[] { typeof(Type) })));
            MetRef.Add("MarkLabel", Module.ImportReference(typeof(System.Reflection.Emit.ILGenerator).GetMethod("MarkLabel", new Type[] { typeof(System.Reflection.Emit.Label) })));
            MetRef.Add("DefineLabel", Module.ImportReference(typeof(System.Reflection.Emit.ILGenerator).GetMethod("DefineLabel")));
            MetRef.Add("GetMethodInfo", Module.ImportReference(typeof(Type).GetMethod("GetMethod", new Type[] { typeof(string), typeof(System.Reflection.BindingFlags), typeof(System.Reflection.Binder), typeof(Type[]), typeof(System.Reflection.ParameterModifier[]) })));
            MetRef.Add("GetMethodInfoTypes", Module.ImportReference(typeof(Type).GetMethod("GetMethod", new Type[] { typeof(string), typeof(Type[]) })));
            MetRef.Add("GetTypeFromHandle", Module.ImportReference(typeof(System.Type).GetMethod("GetTypeFromHandle")));
            MetRef.Add("DynamicMethodConstructor", Module.ImportReference(typeof(System.Reflection.Emit.DynamicMethod).GetConstructor(new Type[] { typeof(string), typeof(System.Reflection.MethodAttributes), typeof(System.Reflection.CallingConventions), typeof(Type), typeof(Type[]), typeof(Type), typeof(bool) })));
            MetRef.Add("GetILGenerator", Module.ImportReference(typeof(System.Reflection.Emit.DynamicMethod).GetMethod("GetILGenerator", new Type[] { })));
            MetRef.Add("GetFieldInfo", Module.ImportReference(typeof(Type).GetMethod("GetField", new Type[] { typeof(string), typeof(System.Reflection.BindingFlags) })));
            MetRef.Add("GetConstructorInfoTypes", Module.ImportReference(typeof(Type).GetMethod("GetConstructor", new Type[] { typeof(Type[]) })));
            MetRef.Add("Invoker", Module.ImportReference(typeof(System.Reflection.MethodBase).GetMethod("Invoke", new Type[] { typeof(object), typeof(object[]) })));
            MetRef.Add("EmitCall", Module.ImportReference(typeof(System.Reflection.Emit.ILGenerator).GetMethod("EmitCall", new Type[] { typeof(System.Reflection.Emit.OpCode), typeof(System.Reflection.MethodInfo), typeof(Type[]) })));

            TypeRef.Add("Type", Module.ImportReference(typeof(Type)));
            TypeRef.Add("Label", Module.ImportReference(typeof(System.Reflection.Emit.Label)));
            TypeRef.Add("LocalBuilder", Module.ImportReference(typeof(System.Reflection.Emit.LocalBuilder)));
            TypeRef.Add("ILGenerator", Module.ImportReference(typeof(System.Reflection.Emit.ILGenerator)));
            TypeRef.Add("DynamicMethod", Module.ImportReference(typeof(System.Reflection.Emit.DynamicMethod)));

            initDynamicMachine(Module);

            Console.ReadKey();
            assembly = "testapp.renamed_dynamized.exe";
            Module.Write(assembly);

            //

            ModuleContext modCtx2 = ModuleDef.CreateModuleContext();
            var module2 = ModuleDefMD.Load(assembly, modCtx2);
            //InitDynamicMachine(module2);

            // galaxy protections
            StringStrongerEnc.Execute(module2);
            IntConfusion.Execute(module2);
            IntProxy.Execute(module2);
            InvalidMetadata.Execute(module2.Assembly);
            SUFconfusion.Execute(module2);
            LocalField.Execute(module2);
            Console.ReadKey();


            // save
            Console.WriteLine("Saving protected file..");
            if (assembly.EndsWith(".exe"))
            {
                string path = assembly.Remove(assembly.Length - 4) + ".virtualized.exe";
                module2.Write(path, new ModuleWriterOptions(module2)
                {
                    Logger = DummyLogger.NoThrowInstance
                });

                Console.WriteLine("Done.");
            }
        }

        private static void initDynamicMachine(ModuleDefinition Module)
        {
            foreach (TypeDefinition type in Module.Types)
            {
                foreach (MethodDefinition method in type.Methods)
                {
                    if (method.Name == ".cctor" || method.Name == ".ctor" || !method.HasBody)
                        continue;

                    method.Body = MethodBodyGeneration(method);

                    CustomAttribute customAttribute = new CustomAttribute(
                    Module.Assembly.MainModule.ImportReference(typeof(CEZA).GetConstructor(new Type[] { typeof(string) })));
                    customAttribute.ConstructorArguments.Add(new CustomAttributeArgument(Module.Assembly.MainModule.ImportReference(typeof(string)), "C E Z A#6466"));
                    method.CustomAttributes.Add(customAttribute);
                    type.IsNotPublic = false;
                    type.IsPublic = true;
                }
            }
        }

        private static MethodBody MethodBodyGeneration(MethodDefinition method)
        {
            MethodBody mbody = new MethodBody(method);
            ILProcessor proc = mbody.GetILProcessor();
            Dictionary<Instruction, VariableDefinition> Branches = new Dictionary<Instruction, VariableDefinition>();
            Dictionary<int, VariableDefinition> Locals = new Dictionary<int, VariableDefinition>();
            proc.CreateDynamicMethod(method);
            VariableDefinition dynamicMethod = new VariableDefinition(TypeRef["DynamicMethod"]);
            proc.Body.Variables.Add(dynamicMethod);
            proc.Emit(OpCodes.Stloc, dynamicMethod);
            proc.Emit(OpCodes.Ldloc, dynamicMethod);
            VariableDefinition ilgenerator = new VariableDefinition(TypeRef["ILGenerator"]);
            proc.Body.Variables.Add(ilgenerator);
            proc.Emit(OpCodes.Callvirt, MetRef["GetILGenerator"]);
            proc.Emit(OpCodes.Stloc, ilgenerator);

            foreach (Instruction instruction in method.Body.Instructions)
            {
                if (instruction.Operand is Instruction)
                {
                    if (instruction.OpCode == OpCodes.Leave_S)
                        continue;

                    Instruction target = instruction.Operand as Instruction;
                    if (Branches.ContainsKey(target))
                        continue;

                    VariableDefinition label = new VariableDefinition(TypeRef["Label"]);
                    proc.Body.Variables.Add(label);
                    proc.Emit(OpCodes.Ldloc, ilgenerator);
                    proc.Emit(OpCodes.Callvirt, MetRef["DefineLabel"]);
                    proc.Emit(OpCodes.Stloc, label);

                    Branches.Add(target, label);
                }

            }

            for (int vI = 0; vI < method.Body.Variables.Count; vI++)
            {

                VariableDefinition local = new VariableDefinition(TypeRef["LocalBuilder"]);
                proc.Body.Variables.Add(local);
                TypeReference variableType = method.Body.Variables[vI].VariableType;
                proc.Emit(OpCodes.Ldloc, ilgenerator);
                proc.EmitType(variableType);
                proc.Emit(OpCodes.Callvirt, MetRef["DeclareLocal"]);
                proc.Emit(OpCodes.Stloc, local);

                Locals.Add(vI, local);
            }


            // iterate through instructions, writer ILGenerator.Emit calls
            for (int iI = 0; iI < method.Body.Instructions.Count; iI++)
            {

                // the current instruction
                Instruction instruction = method.Body.Instructions[iI];

                foreach (ExceptionHandler exH in method.Body.ExceptionHandlers)
                {
                    if (exH.TryStart == instruction)
                    {
                        proc.Emit(OpCodes.Ldloc, ilgenerator);
                        proc.Emit(OpCodes.Callvirt, MetRef["TryStart"]);
                        proc.Emit(OpCodes.Pop); // pop TryStart return value from eval stack
                    }
                    else if (exH.HandlerStart == instruction)
                    {
                        proc.Emit(OpCodes.Ldloc, ilgenerator);
                        if (exH.HandlerType == ExceptionHandlerType.Catch)
                        {
                            proc.EmitType(exH.CatchType);
                            proc.Emit(OpCodes.Callvirt, MetRef["CatchBlock"]);
                        }
                        else if (exH.HandlerType == ExceptionHandlerType.Finally)
                        {
                            proc.Emit(OpCodes.Callvirt, MetRef["FinallyBlock"]);
                        }
                    }
                    else if (exH.TryEnd == instruction || exH.HandlerEnd == instruction)
                    {
                        proc.Emit(OpCodes.Ldloc, ilgenerator);
                        proc.Emit(OpCodes.Callvirt, MetRef["TryEnd"]);
                    }
                }

                if (instruction.OpCode == OpCodes.Leave_S)
                    continue;

                if (Branches.ContainsKey(instruction))
                {
                    proc.Emit(OpCodes.Ldloc, ilgenerator);
                    proc.EmitMarkLabel(Branches[instruction]);
                }

                proc.Emit(OpCodes.Ldloc, ilgenerator);

                int stlocIndex = instruction.GetStlocIndex();
                int ldlocIndex = instruction.GetLdlocIndex();

                if (stlocIndex > -1 || ldlocIndex > -1)
                {
                    bool isStloc = stlocIndex > -1;
                    instruction.OpCode = isStloc ? OpCodes.Stloc : OpCodes.Ldloc;
                    int localIndex = isStloc ? stlocIndex : ldlocIndex;

                    proc.Emit(OpCodes.Ldsfld, ReflectionUtils.GetReflectedOpCode(instruction));
                    proc.Emit(OpCodes.Ldloc, Locals[localIndex]);
                    proc.Emit(OpCodes.Callvirt, ReflectionUtils.GetILGeneratorEmitter(typeof(System.Reflection.Emit.LocalBuilder)));
                    continue;
                }

                proc.Emit(OpCodes.Ldsfld, ReflectionUtils.GetReflectedOpCode(instruction));

                Type EmitType = null;
                if (instruction.Operand != null)
                {
                    if (instruction.Operand is FieldDefinition)
                    {
                        FieldDefinition fieldDefinition = instruction.Operand as FieldDefinition;
                        proc.EmitFieldGetter(fieldDefinition);
                        EmitType = typeof(System.Reflection.FieldInfo);
                    }
                    else if (instruction.Operand is MethodDefinition)
                    {
                        MethodDefinition methodDefinition = instruction.Operand as MethodDefinition;
                        proc.EmitMethodGetter(methodDefinition);
                        EmitType = methodDefinition.IsConstructor ? typeof(System.Reflection.ConstructorInfo) : typeof(System.Reflection.MethodInfo);
                    }
                    else if (instruction.Operand is MethodReference)
                    {
                        MethodReference methodReference = instruction.Operand as MethodReference;
                        proc.EmitMethodGetter(methodReference);
                        EmitType = typeof(System.Reflection.MethodInfo);
                    }
                    else if (instruction.Operand.GetType() == typeof(sbyte))
                    {
                        sbyte value = Convert.ToSByte(instruction.Operand);
                        proc.Emit(OpCodes.Ldc_I4_S, value);
                        EmitType = typeof(sbyte);
                    }
                    else if (instruction.Operand.GetType() == typeof(string))
                    {
                        string value = Convert.ToString(instruction.Operand);
                        proc.Emit(OpCodes.Ldstr, value);
                        EmitType = typeof(string);
                    }
                    else if (instruction.Operand.GetType() == typeof(int))
                    {
                        int value = Convert.ToInt32(instruction.Operand);
                        proc.Emit(OpCodes.Ldc_I4, value);
                        EmitType = typeof(int);
                    }
                    else if (instruction.Operand.GetType() == typeof(float))
                    {
                        float value = Convert.ToSingle(instruction.Operand);
                        proc.Emit(OpCodes.Ldc_R4, value);
                        EmitType = typeof(float);
                    }
                    else if (instruction.Operand is Instruction)
                    {
                        Instruction targetInstruction = instruction.Operand as Instruction;
                        proc.Emit(OpCodes.Ldloc, Branches[targetInstruction]);
                        EmitType = typeof(System.Reflection.Emit.Label);
                    }
                    else if (instruction.Operand is TypeReference)
                    {
                        TypeReference typeReference = instruction.Operand as TypeReference;
                        proc.EmitType(typeReference);
                        EmitType = typeof(Type);
                    }
                    else
                    {
                        Console.WriteLine("Internal error. opcode = " + instruction.OpCode.Name + ", type = " + instruction.Operand.GetType());
                    }
                }

                if (instruction.OpCode == OpCodes.Call || instruction.OpCode == OpCodes.Callvirt)
                {
                    proc.Emit(OpCodes.Ldnull);
                    proc.Emit(OpCodes.Callvirt, MetRef["EmitCall"]);
                    continue;
                }

                proc.Emit(OpCodes.Callvirt, ReflectionUtils.GetILGeneratorEmitter(EmitType));
            }

            proc.Emit(OpCodes.Ldloc, dynamicMethod);
            proc.Emit(OpCodes.Ldnull);
            proc.Emit(OpCodes.Ldc_I4_S, Convert.ToSByte(method.Parameters.Count));
            proc.Emit(OpCodes.Newarr, Module.TypeSystem.Object);

            for (int pI = 0; pI < method.Parameters.Count; pI++)
            {
                ParameterDefinition parameter = method.Parameters[pI];
                proc.Emit(OpCodes.Dup);
                proc.Emit(OpCodes.Ldc_I4_S, Convert.ToSByte(pI));
                proc.Emit(OpCodes.Ldarg_S, parameter);
                proc.Emit(OpCodes.Box, parameter.ParameterType);
                proc.Emit(OpCodes.Stelem_Ref);
            }

            proc.Emit(OpCodes.Callvirt, MetRef["Invoker"]);

            if (method.ReturnType != Module.TypeSystem.Void)
            {
                proc.Emit(OpCodes.Unbox_Any, method.ReturnType);
            }
            else
            {
                proc.Emit(OpCodes.Pop);
            }

            proc.Emit(OpCodes.Ret);

            return mbody;
        }

    }

}
