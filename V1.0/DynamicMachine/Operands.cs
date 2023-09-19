using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

public class Operands
{
    public static Type GetOperandType(string instruction)
    {
        // İlgili komutun sonundaki türü al
        int typeIndex = instruction.LastIndexOf(" ") + 1;
        string typeStr = instruction.Substring(typeIndex).Trim();

        // Türü doğru bir .NET Type nesnesine dönüştür
        switch (typeStr.ToLower())
        {
            case "ldc.i4.s":
                return typeof(int);
            case "ldstr":
                return typeof(string);
            // Diğer türler için gerektiğinde ekleyebilirsiniz


            default:
                return typeof(string);
                //throw new Exception("invalid operand type -> " + typeStr);
        }
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