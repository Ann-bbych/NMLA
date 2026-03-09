using System;
using System.Globalization;
using System.IO;


namespace GaussMethod
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string inputFileName = "input.txt";
            const string outputFileName = "output.txt";

            using StreamWriter writer = new StreamWriter(outputFileName);

            try
            {
                int size;
                double[,] matrix = ReadFrom(inputFileName, out size);
                double[,] originalMatrix = GaussSolver.Copy(matrix);

                WriteLineTo("Початкова розширена матриця:", writer);
                PrintOn(matrix, size, writer);
                WriteLineTo("", writer);

                int swapCount = 0;
                GaussSolver.ForwardElimination(matrix, size, ref swapCount);

                WriteLineTo("Східчаста розширена матриця після прямого ходу:", writer);
                PrintOn(matrix, size, writer);
                WriteLineTo("", writer);

                double[] solution = GaussSolver.BackSubstitution(matrix, size);

                WriteLineTo("Розв'язок СЛАР:", writer);
                PrintSolution(solution, writer);
                WriteLineTo("", writer);

                double[] checkResult = GaussSolver.CheckSolution(originalMatrix, solution, size);

                WriteLineTo("Перевірка:", writer);
                PrintVerification(originalMatrix, solution, checkResult, size, writer);
                WriteLineTo("", writer);

                double determinant = GaussSolver.CalculateDeterminant(matrix, size, swapCount);

                WriteLineTo("Визначник матриці:", writer);
                WriteLineTo(FormatNumber(determinant), writer);
            }
            catch (Exception ex)
            {
                WriteLineTo(ex.Message, writer);
            }
        }

        static double[,] ReadFrom(string fileName, out int size)
        { // повертаю матрицю та розмір через out-параметр
           
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("Файл input.txt не знайдено.");
            }

            string[] lines = File.ReadAllLines(fileName);

            if (lines.Length == 0)
            {
                throw new Exception("Файл input.txt порожній.");
            }

            if (!int.TryParse(lines[0].Trim(), out size) || size <= 0)
            {
                throw new Exception("Некоректно задано розмір матриці.");
            }

            if (lines.Length < size + 1)
            {
                throw new Exception("Недостатньо рядків для зчитування матриці.");
            }

            double[,] matrix = new double[size, size + 1];

            for (int row = 0; row < size; row++)
            {
                string[] parts = lines[row + 1].Split(new char[] { ' ', '\t' },
                    StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != size + 1)
                {
                    throw new Exception($"У рядку {row + 2} має бути {size + 1} чисел.");
                }

                for (int column = 0; column < size + 1; column++)
                {
                    if (!double.TryParse(parts[column], NumberStyles.Float,
                        CultureInfo.InvariantCulture, out matrix[row, column]))
                    {
                        throw new Exception($"Некоректне число у рядку {row + 2}, стовпці {column + 1}.");
                    }
                }
            }

            return matrix;
        }

        static void PrintOn(double[,] matrix, int size, StreamWriter writer)
        {
            for (int row = 0; row < size; row++)
            {
                for (int column = 0; column < size + 1; column++)
                {
                    string value = FormatNumber(matrix[row, column]);

                    if (column == size - 1)
                    {
                        WriteTo(value.PadLeft(10) + " |", writer);
                    }
                    else
                    {
                        WriteTo(value.PadLeft(10), writer);
                    }
                }

                WriteLineTo("", writer);
            }
        }

        static void WriteLineTo(string text, StreamWriter writer)
        {
            Console.WriteLine(text);
            writer.WriteLine(text);
        }

        static void WriteTo(string text, StreamWriter writer)
        {
            Console.Write(text);
            writer.Write(text);
        }

        static string FormatNumber(double value)
        {
            if (Math.Abs(value - Math.Round(value)) < GaussSolver.EPS)
            {
                return ((long)Math.Round(value)).ToString();
            }

            return value.ToString("0.##", CultureInfo.InvariantCulture);
        }

        static void PrintSolution(double[] solution, StreamWriter writer)
        {
            for (int index = 0; index < solution.Length; index++)
            {
                WriteLineTo($"x{index + 1} = {FormatNumber(solution[index])}", writer);
            }
        }

        static void PrintVerification(double[,] originalMatrix, double[] solution,
            double[] checkResult, int size, StreamWriter writer)
        {
            for (int row = 0; row < size; row++)
            {
                for (int column = 0; column < size; column++)
                {
                    string part = $"{FormatNumber(originalMatrix[row, column])}*{FormatNumber(solution[column])}";

                    if (column < size - 1)
                    {
                        part += " + ";
                    }
                    else
                    {
                        part += " = ";
                    }

                    WriteTo(part, writer);
                }

                WriteLineTo(FormatNumber(originalMatrix[row, size]), writer);
            }

            bool isCorrect = true;

            for (int row = 0; row < size; row++)
            {
                if (Math.Abs(checkResult[row] - originalMatrix[row, size]) >= GaussSolver.EPS * 100)
                {
                    isCorrect = false;
                    break;
                }
            }

            if (isCorrect)
            {
                WriteLineTo("Перевірка пройдена.", writer);
            }
            else
            {
                WriteLineTo("Перевірка не пройдена.", writer);
            }
        }
    }
}