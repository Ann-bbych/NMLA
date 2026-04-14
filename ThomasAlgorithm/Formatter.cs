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

        // форматований вивід чисел:
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

        // форматований вивід похибок: 
        // округлення до стільки знаків після коми, скільки потрібно для видимості похибки
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

        // виведення СЛАР у вигляді рівнянь
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

        public static void PrintSolution(double[] solution, StreamWriter writer, string title, double eps)
        {
            WriteLineToBoth(title, writer);

            for (int i = 0; i < solution.Length; i++)
            {
                WriteLineToBoth($"y{i} = {FormatNumber(solution[i], eps)}", writer);
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
            WriteLineToBoth("y` - y = 2 - x^2, 0 < x < 1", writer);
            WriteLineToBoth("y(0) = 0", writer);
            WriteLineToBoth("y(1) = 1", writer);
            WriteLineToBoth(string.Empty, writer);
            WriteLineToBoth("Точний розв'язок:", writer);
            WriteLineToBoth("y(x) = x^2", writer);
            WriteLineToBoth(string.Empty, writer);
        }

        // вивід порівняння точного, чисельного розв'язків, похибки таблицею
        public static void PrintExactAndNumericalSolutions(
            double[] grid,
            double[] exactValues,
            double[] numericalValues,
            double[] errors,
            StreamWriter writer,
            double eps)
        {
            WriteLineToBoth("Точний і чисельний розв'язки:", writer);
            WriteLineToBoth(
                "i\t x_i\t\t y_точне\t y_чисельне\t похибка",
                writer);

            for (int i = 0; i < grid.Length; i++)
            {
                string line =
                    i.ToString(CultureInfo.InvariantCulture) + "\t " +
                    FormatNumber(grid[i], eps) + "\t\t " +
                    FormatNumber(exactValues[i], eps) + "\t\t " +
                    FormatNumber(numericalValues[i], eps) + "\t\t " +
                    FormatError(errors[i], eps);

                WriteLineToBoth(line, writer);
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
    }
}
