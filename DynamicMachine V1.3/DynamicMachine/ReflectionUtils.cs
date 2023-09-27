using System;
using System.Reflection;
using System.Linq;
using System.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;
using FieldAttributes = Mono.Cecil.FieldAttributes;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using OpCodes = System.Reflection.Emit.OpCodes;
using OpCode = System.Reflection.Emit.OpCode;

namespace DynamicMachine
{
    public static class ReflectionUtils
    {
        public static int GetMethodBindingFlags(MethodDefinition method)
        {
            int flags = 0;

            if ((method.Attributes & MethodAttributes.Static) != 0)
                flags |= (int)BindingFlags.Static;
            else
                flags |= (int)BindingFlags.Instance;

            if ((method.Attributes & MethodAttributes.Public) != 0)
                flags |= (int)BindingFlags.Public;
            else
                flags |= (int)BindingFlags.NonPublic;

            return flags;
        }

        public static int GetFieldBindingFlags(FieldDefinition field)
        {
            int flags = 0;

            if ((field.Attributes & FieldAttributes.Static) != 0)
                flags |= (int)BindingFlags.Static;
            else
                flags |= (int)BindingFlags.Instance;

            if ((field.Attributes & FieldAttributes.Public) != 0)
                flags |= (int)BindingFlags.Public;
            else
                flags |= (int)BindingFlags.NonPublic;

            return flags;
        }

        public static FieldReference GetReflectedOpCode(Instruction instruction)
        {
            string opCodeName = instruction.OpCode.Code.ToString();

            if (opCodeName == "Ldelem_Any")
                opCodeName = "Ldelem";
            else if (opCodeName == "Stelem_Any")
                opCodeName = "Stelem";

            FieldInfo opCodeFieldInfo = typeof(OpCodes).GetField(opCodeName);

            return Program.Module.ImportReference(typeof(OpCodes).GetField(opCodeName));
        }

        public static int GetStlocIndex(this Instruction instruction)
        {
            OpCode[] list = {
                OpCodes.Stloc_0,
                OpCodes.Stloc_1,
                OpCodes.Stloc_2,
                OpCodes.Stloc_3,
                OpCodes.Stloc_S,
                OpCodes.Stloc
            };
            int index = Array.IndexOf(list, instruction.OpCode);
            if (index > 3)
                return Convert.ToInt32(instruction.Operand);
            else
                return index;
        }

        public static int GetLdlocIndex(this Instruction instruction)
        {
            OpCode[] list = {
                OpCodes.Ldloc_0,
                OpCodes.Ldloc_1,
                OpCodes.Ldloc_2,
                OpCodes.Ldloc_3,
                OpCodes.Ldloc_S,
                OpCodes.Ldloc
            };
            int index = Array.IndexOf(list, instruction.OpCode);
            return index > 3 ? Convert.ToInt32(instruction.Operand) : index;
        }

        public static MethodReference GetILGeneratorEmitter(Type t = null)
        {
            if (t == null)
                return Program.Module.ImportReference(typeof(ILGenerator).GetMethod("Emit", new Type[] { typeof(OpCode) }));
            else
                return Program.Module.ImportReference(typeof(ILGenerator).GetMethod("Emit", new Type[] { typeof(OpCode), t }));
        }
    }
}
