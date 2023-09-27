using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using OpCodes = Mono.Cecil.Cil.OpCodes;

namespace DynamicMachine
{
    public static class ILConvertion
    {
        public static void CreateDynamicMethod(this ILProcessor proc, MethodDefinition method)
        {
            proc.Emit(OpCodes.Ldstr, "DynamicMachine" + new Random().Next());
            proc.Emit(OpCodes.Ldc_I4, 0x16);
            proc.Emit(OpCodes.Ldc_I4_1);
            proc.EmitType(method.ReturnType);
            proc.EmitTypeArray(method.Parameters);
            proc.EmitType(method.DeclaringType);
            proc.Emit(OpCodes.Ldc_I4_0);
            proc.Emit(OpCodes.Newobj, Program.MetRef["DynamicMethodConstructor"]);
        }

        public static void EmitMethodGetter(this ILProcessor proc, MethodDefinition method)
        {
            proc.EmitType(method.DeclaringType);

            if (method.IsConstructor)
            {

                proc.EmitTypeArray(method.Parameters);

                proc.Emit(OpCodes.Callvirt, Program.MetRef["GetConstructorInfoTypes"]);

                return;

            }

            method.IsPrivate = false;
            method.IsPublic = true;

            proc.Emit(OpCodes.Ldstr, method.Name);
            proc.Emit(OpCodes.Ldc_I4, ReflectionUtils.GetMethodBindingFlags(method));
            proc.Emit(OpCodes.Ldnull);
            proc.EmitTypeArray(method.Parameters);
            proc.Emit(OpCodes.Ldnull);

            proc.Emit(OpCodes.Callvirt, Program.MetRef["GetMethodInfo"]);
        }

        public static void EmitTypeArray(this ILProcessor proc, Collection<ParameterDefinition> parameters)
        {
            proc.Emit(OpCodes.Ldc_I4_S, Convert.ToSByte(parameters.Count));
            proc.Emit(OpCodes.Newarr, Program.TypeRef["Type"]);

            for (int pI = 0; pI < parameters.Count; pI++)
            {
                proc.Emit(OpCodes.Dup);
                proc.Emit(OpCodes.Ldc_I4, pI);
                proc.EmitType(parameters[pI].ParameterType);
                proc.Emit(OpCodes.Stelem_Ref);
            }
        }

        public static void EmitType(this ILProcessor proc, TypeReference reference)
        {
            proc.Emit(OpCodes.Ldtoken, reference);
            proc.Emit(OpCodes.Call, Program.MetRef["GetTypeFromHandle"]);
        }

        public static void EmitMarkLabel(this ILProcessor processor, VariableDefinition label)
        {
            processor.Emit(OpCodes.Ldloc, label);
            processor.Emit(OpCodes.Callvirt, Program.MetRef["MarkLabel"]);
        }

        public static void EmitMethodGetter(this ILProcessor proc, MethodReference method)
        {
            proc.EmitType(method.DeclaringType);

            if (method.Name == ".ctor")
            {

                proc.EmitTypeArray(method.Parameters);

                proc.Emit(OpCodes.Call, Program.MetRef["GetConstructorInfoTypes"]);

                return;

            }

            proc.Emit(OpCodes.Ldstr, method.Name);
            proc.EmitTypeArray(method.Parameters);

            proc.Emit(OpCodes.Call, Program.MetRef["GetMethodInfoTypes"]);
        }

        public static void EmitFieldGetter(this ILProcessor proc, FieldDefinition field)
        {
            field.IsPrivate = false;
            field.IsPublic = true;

            proc.EmitType(field.DeclaringType);
            proc.Emit(OpCodes.Ldstr, field.Name);

            proc.Emit(OpCodes.Ldc_I4, ReflectionUtils.GetFieldBindingFlags(field));

            proc.Emit(OpCodes.Callvirt, Program.MetRef["GetFieldInfo"]);
        }
    }
}
