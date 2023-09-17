using System;

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