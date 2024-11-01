using System.Diagnostics;

namespace TraceTesting;

internal sealed class Traced
{
    public static int TwoTimes(int parameter)
    {
        Debug.WriteLine($"That's a debug line for traced with parameter: {parameter}");
        Debug.Assert(parameter < 1, "parameter is >= 1");

        return parameter * 2;
    }
}