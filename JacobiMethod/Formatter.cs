using System;
using System.Globalization;

namespace JacobiMethod;

internal class Formatter
{

    public static string FormatMatrixNumber(double value)
    {
        if (Math.Abs(value - Math.Round(value)) < Constants.eps)
            return Math.Round(value).ToString(CultureInfo.InvariantCulture);

        return value.ToString("0.00", CultureInfo.InvariantCulture);
    }

    public static string FormatPreciseNumber(double value)
    {
        if (Math.Abs(value - Math.Round(value)) < Constants.eps)
            return Math.Round(value).ToString(CultureInfo.InvariantCulture);

        return value.ToString("0.##########", CultureInfo.InvariantCulture);
    }

    public static void WriteLineToBoth(string text, StreamWriter writer)
    {
        Console.WriteLine(text);
        writer.WriteLine(text);
    }

    public static void WriteToBoth(string text, StreamWriter writer)
    {
        Console.Write(text);
        writer.Write(text);
    }

    public static void PrintVector(string name, double[] vector, StreamWriter writer)
    {
        WriteToBoth(name + " = (", writer);

        for (int i = 0; i < vector.Length; i++)
        {
            WriteToBoth(FormatPreciseNumber(vector[i]), writer);

            if (i != vector.Length - 1)
                WriteToBoth("; ", writer);
        }

        WriteLineToBoth(")", writer);
    }
}