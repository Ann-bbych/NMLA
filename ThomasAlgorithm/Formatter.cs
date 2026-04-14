using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace ThomasAlgorithm
{
    public static class Formatter
    {
        public static void WriteLineToBoth(string text, StreamWriter writer)
        { // з переходом на новий рядок
            Console.WriteLine(text);
            writer.WriteLine(text);
        }

        public static void WriteToBoth(string text, StreamWriter writer)
        { // без переходу на новий рядок
            Console.Write(text);
            writer.Write(text);
        }

        // форматований вивід чисел (для режиму 1):
        // десяткові -> округлення до сотих
        // цілі -> без крапки
        public static string FormatNumber(double value, double eps)
        {
            if (Math.Abs(value) < eps)
            {
                return "0";
            }

            double roundedInteger = Math.Round(value);
            if (Math.Abs(value - roundedInteger) < eps)
            {
                return ((int)roundedInteger).ToString(CultureInfo.InvariantCulture);
            }

            double rounded = Math.Round(value, 2);
            if (Math.Abs(rounded) < eps)
            {
                return "0";
            }

            return rounded.ToString("0.00", CultureInfo.InvariantCulture);
        }

        // форматування чисел (для режиму 2) 
        // без жорсткого округлення, з повною видимістю малих значень
        public static string FormatNumberMode2(double value, double eps)
        {
            if (Math.Abs(value) < eps)
            {
                return "0";
            }

            return value.ToString("0.################", CultureInfo.InvariantCulture);
        }

        // форматований вивід похибок (для режиму 1)
        public static string FormatError(double value, double eps)
        {
            if (Math.Abs(value) < eps)
            {
                return "0";
            }

            value = Math.Abs(value);

            for (int digits = 2; digits <= 12; digits++)
            {
                double rounded = Math.Round(value, digits);

                if (rounded >= eps)
                {
                    string format = "0." + new string('0', digits);
                    return rounded.ToString(format, CultureInfo.InvariantCulture);
                }
            }

            return value.ToString("0.############", CultureInfo.InvariantCulture);
        }


        // виведення СЛАР у вигляді рівнянь (для режиму 1)
        public static void PrintSystem(TridiagonalSystem system, StreamWriter writer, string title, double eps)
        {
            WriteLineToBoth(title, writer);

            for (int i = 0; i < system.N; i++)
            {
                string equation = BuildEquation(system, i, eps);
                WriteLineToBoth(equation, writer);
            }

            WriteLineToBoth(string.Empty, writer);
        }

        // виведення СЛАР у вигляді рівнянь (для режиму 2)
        public static void PrintSystemMode2(TridiagonalSystem system, StreamWriter writer, string title, double eps)
        {
            WriteLineToBoth(title, writer);

            for (int i = 0; i < system.N; i++)
            {
                string equation = BuildEquationMode2(system, i, eps);
                WriteLineToBoth(equation, writer);
            }

            WriteLineToBoth(string.Empty, writer);
        }

        public static void PrintSolution(double[] solution, StreamWriter writer, string title, double eps)
        {
            WriteLineToBoth(title, writer);

            for (int i = 0; i < solution.Length; i++)
            {
                WriteLineToBoth($"y{i} = {FormatNumber(solution[i], eps)}", writer);
            }

            WriteLineToBoth(string.Empty, writer);
        }

        // вивід розв'язку (для режиму 2)
        public static void PrintSolutionMode2(double[] solution, StreamWriter writer, string title, double eps)
        {
            WriteLineToBoth(title, writer);

            for (int i = 0; i < solution.Length; i++)
            {
                WriteLineToBoth($"y{i} = {FormatNumberMode2(solution[i], eps)}", writer);
            }

            WriteLineToBoth(string.Empty, writer);
        }

        // вивід перевірки необідних умов для лівої прогонки
        public static void PrintConditionsResult(string message, StreamWriter writer)
        {
            WriteLineToBoth("Перевірка необхідних умов:", writer);
            WriteLineToBoth(message, writer);
            WriteLineToBoth(string.Empty, writer);
        }

        // вивід крайової задачі і точного розв'язку 
        public static void PrintBoundaryValueProblem(StreamWriter writer)
        {
            WriteLineToBoth("Крайова задача:", writer);
            WriteLineToBoth("y'' - y = 2 - x^2, 0 < x < 1", writer);
            WriteLineToBoth("y(0) = 0", writer);
            WriteLineToBoth("y(1) = 1", writer);
            WriteLineToBoth(string.Empty, writer);
            WriteLineToBoth("Точний розв'язок:", writer);
            WriteLineToBoth("y(x) = x^2", writer);
            WriteLineToBoth(string.Empty, writer);
        }

        // вивід похибки між точним і чисельним розв'язками
        public static void PrintErrors(double[] exactValues, double[] numericalValues, double[] errors, StreamWriter writer, double eps)
        {
            WriteLineToBoth("Похибка:", writer);

            for (int i = 0; i < errors.Length; i++)
            {
                WriteLineToBoth(
                    $"|{FormatNumberMode2(exactValues[i], eps)} - {FormatNumberMode2(numericalValues[i], eps)}| = {FormatNumberMode2(errors[i], eps)}",
                    writer);
            }

            WriteLineToBoth(string.Empty, writer);
        }

        private static string BuildEquation(TridiagonalSystem system, int row, double eps)
        { // для виведення СЛАР рівняннями
            StringBuilder builder = new StringBuilder();

            if (row > 0 && Math.Abs(system.A[row]) >= eps)
            {
                builder.Append(FormatNumber(system.A[row], eps));
                builder.Append(" * y");
                builder.Append(row - 1);
            }

            if (Math.Abs(system.C[row]) >= eps)
            {
                if (builder.Length > 0 && system.C[row] >= 0)
                {
                    builder.Append(" + ");
                }
                else if (builder.Length > 0 && system.C[row] < 0)
                {
                    builder.Append(" - ");
                    builder.Append(FormatNumber(Math.Abs(system.C[row]), eps));
                    builder.Append(" * y");
                    builder.Append(row);
                    goto UpperPart;
                }

                builder.Append(FormatNumber(system.C[row], eps));
                builder.Append(" * y");
                builder.Append(row);
            }

        UpperPart:
            if (row < system.N - 1 && Math.Abs(system.B[row]) >= eps)
            {
                if (system.B[row] >= 0)
                {
                    builder.Append(" + ");
                    builder.Append(FormatNumber(system.B[row], eps));
                }
                else
                {
                    builder.Append(" - ");
                    builder.Append(FormatNumber(Math.Abs(system.B[row]), eps));
                }

                builder.Append(" * y");
                builder.Append(row + 1);
            }

            builder.Append(" = ");
            builder.Append(FormatNumber(system.F[row], eps));

            return builder.ToString();
        }

        private static string BuildEquationMode2(TridiagonalSystem system, int row, double eps)
        { // для виведення СЛАР рівняннями у 2-му режимі
            StringBuilder builder = new StringBuilder();

            if (row > 0 && Math.Abs(system.A[row]) >= eps)
            {
                builder.Append(FormatNumberMode2(system.A[row], eps));
                builder.Append(" * y");
                builder.Append(row - 1);
            }

            if (Math.Abs(system.C[row]) >= eps)
            {
                if (builder.Length > 0 && system.C[row] >= 0)
                {
                    builder.Append(" + ");
                }
                else if (builder.Length > 0 && system.C[row] < 0)
                {
                    builder.Append(" - ");
                    builder.Append(FormatNumberMode2(Math.Abs(system.C[row]), eps));
                    builder.Append(" * y");
                    builder.Append(row);
                    goto UpperPart;
                }

                builder.Append(FormatNumberMode2(system.C[row], eps));
                builder.Append(" * y");
                builder.Append(row);
            }

        UpperPart:
            if (row < system.N - 1 && Math.Abs(system.B[row]) >= eps)
            {
                if (system.B[row] >= 0)
                {
                    builder.Append(" + ");
                    builder.Append(FormatNumberMode2(system.B[row], eps));
                }
                else
                {
                    builder.Append(" - ");
                    builder.Append(FormatNumberMode2(Math.Abs(system.B[row]), eps));
                }

                builder.Append(" * y");
                builder.Append(row + 1);
            }

            builder.Append(" = ");
            builder.Append(FormatNumberMode2(system.F[row], eps));

            return builder.ToString();
        }
    }
}
    

