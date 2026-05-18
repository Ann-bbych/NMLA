using System;
using System.Globalization;
using System.IO;

class Formatter
{
    private const double integerTolerance = 1e-12;

    public static void WriteLineTo(string text, StreamWriter writer)
    {
        Console.WriteLine(text);
        writer.WriteLine(text);
    }

    public static void WriteTo(string text, StreamWriter writer)
    {
        Console.Write(text);
        writer.Write(text);
    }

    public static string FormatMatrixNumber(double value)
    {
        if (Math.Abs(value - Math.Round(value)) < integerTolerance)
            return ((int)Math.Round(value)).ToString(CultureInfo.InvariantCulture);

        return value.ToString("0.00", CultureInfo.InvariantCulture);
    }

    public static string FormatInputNumber(double value)
    {
        if (Math.Abs(value - Math.Round(value)) < integerTolerance)
            return ((int)Math.Round(value)).ToString(CultureInfo.InvariantCulture);

        return value.ToString("G17", CultureInfo.InvariantCulture);
    }

    public static string FormatResultNumber(double value, int digits)
    {
        string format = "0";

        if (digits > 0)
            format += "." + new string('0', digits);

        return value.ToString(format, CultureInfo.InvariantCulture);
    }

    public static string FormatFullNumber(double value)
    {
        return value.ToString("G17", CultureInfo.InvariantCulture);
    }

    public static string FormatCheckNumber(double value, int digits)
    {
        int checkDigits = digits + 1;

        string format = "0";

        if (checkDigits > 0)
            format += "." + new string('0', checkDigits);

        return value.ToString(format, CultureInfo.InvariantCulture);
    }
    public static string FormatCheckVector(double[] vector, int digits)
    {
        string result = "(";

        for (int i = 0; i < vector.Length; i++)
        {
            if (double.IsNaN(vector[i]))
                result += "-";
            else
                result += FormatCheckNumber(vector[i], digits);

            if (i != vector.Length - 1)
                result += "; ";
        }

        result += ")";

        return result;
    }

    public static int CountDigitsAfterPoint(string value)
    {
        int index = value.IndexOf('.');

        if (index == -1)
            return 0;

        return value.Length - index - 1;
    }

    public static string FormatVector(double[] vector, int digits)
    {
        string result = "(";

        for (int i = 0; i < vector.Length; i++)
        {
            if (double.IsNaN(vector[i]))
                result += "-";
            else
                result += FormatResultNumber(vector[i], digits);

            if (i != vector.Length - 1)
                result += "; ";
        }

        result += ")";

        return result;
    }

    public static string FormatInputVector(double[] vector)
    {
        string result = "(";

        for (int i = 0; i < vector.Length; i++)
        {
            result += FormatInputNumber(vector[i]);

            if (i != vector.Length - 1)
                result += "; ";
        }

        result += ")";

        return result;
    }

    public static void PrintMatrix(double[,] matrix, StreamWriter writer)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            string line = "";

            for (int j = 0; j < cols; j++)
                line += FormatMatrixNumber(matrix[i, j]) + " ";

            WriteLineTo(line.TrimEnd(), writer);
        }
    }
}