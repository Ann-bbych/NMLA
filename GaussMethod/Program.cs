using System;
using System.Globalization;
using System.IO; // для File, StreamReader, StreamWriter 


namespace GaussMethod
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // правильно відкрити файл:
            string projectDirectory = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));

            string inputFileName = Path.Combine(projectDirectory, "input.txt");
            string outputFileName = Path.Combine(projectDirectory, "output.txt");

            using StreamWriter writer = new StreamWriter(outputFileName);
            // об'єкт для запису в файл, який також буде використовуватись для виводу на консоль
            // після використання StreamWriter автоматично закриється після завершення Main
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

                WriteLineTo("Східчаста розширена матриця:", writer);
                PrintOn(matrix, size, writer);
                WriteLineTo("", writer);

                double[] solution = GaussSolver.BackSubstitution(matrix, size);

                WriteLineTo("Розв'язок СЛАР:", writer);
                PrintSolution(solution, writer);
                WriteLineTo("", writer);
                // масив лівих частин рівнянь з підставленими розв'язками:
                double[] leftResults = GaussSolver.CheckSolution(originalMatrix, solution, size);

                WriteLineTo("Перевірка:", writer);
                PrintVerification(originalMatrix, solution, leftResults, size, writer);
                WriteLineTo("", writer);

                double determinant = GaussSolver.CalculateDeterminant(matrix, size, swapCount);

                WriteLineTo("Визначник:", writer);
                WriteLineTo(FormatNumber(determinant), writer);
            }
            catch (Exception ex)
            {
                WriteLineTo(ex.Message, writer);
            }
        }

        static double[,] ReadFrom(string fileName, out int size)
        { // повертаю розмір через out-параметр
           
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("Не бачу input.txt .");
            }

            string[] lines = File.ReadAllLines(fileName);

            if (lines.Length == 0)
            {
                throw new Exception("У input.txt нічого нема.");
            }
            // Trim() видаляє зайві пробіли
            // int.TryParse поверне true якщо перетворить текст на число і запише в size
            if (!int.TryParse(lines[0].Trim(), out size) || size <= 0)
            {
                throw new Exception("Неправильний розмір матриці.");
            }

            if (lines.Length < size + 1)
            {
                throw new Exception("Замало рядків для матриці.");
            }

            double[,] matrix = new double[size, size + 1];

            for (int row = 0; row < size; row++)
            {
                // розбиваю перший рядок матриці на числа, кожне стає елементом parts[]
                string[] parts = lines[row + 1].Split(new char[] { ' ', '\t' },
                    StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != size + 1)
                {
                    throw new Exception($"У рядку {row + 2} має бути {size + 1} чисел.");
                }

                for (int column = 0; column < size + 1; column++)
                {
                    // перетворюю текстові елементи з parts[] на тип double і записую в матрицю
                    if (!double.TryParse(parts[column], NumberStyles.Float,
                        CultureInfo.InvariantCulture, out matrix[row, column]))
                    {
                        throw new Exception($"Неправильне число у {row + 2} рядку, {column + 1} стовпці.");
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
                        WriteTo(value.PadLeft(15) + "   |", writer);
                    }
                    else if (column == size)
                    {
                        WriteTo(value.PadLeft(15), writer);
                    }
                    else
                    {
                        WriteTo(value.PadLeft(15), writer);
                    }
                }

                WriteLineTo("", writer);
            }
        }

        static void WriteLineTo(string text, StreamWriter writer)
        { // переходить на новий рядок 
            Console.WriteLine(text);
            writer.WriteLine(text);
        }

        static void WriteTo(string text, StreamWriter writer)
        { // не переходить на новий рядок 
            Console.Write(text);
            writer.Write(text);
        }

        static string FormatNumber(double value)
        {
            // якщо число практично ціле:
            if (Math.Abs(value - Math.Round(value)) < GaussSolver.EPS)
            { // заокруглюємо, робимо цілого числового типу long і перетворюємо на текст 
                return ((long)Math.Round(value)).ToString();
            }
            // інакше виводимо з максимум 2 знаками після крапки
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
            double[] leftResults, int size, StreamWriter writer)
        { // запис + реальна перевірка

            for (int row = 0; row < size; row++)
            {
                for (int column = 0; column < size; column++)
                { // записую ліву частину з підставленими розв'язками:
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
                // записую праву частину:
                WriteLineTo(FormatNumber(originalMatrix[row, size]), writer);
            }

            bool isCorrect = true;

            for (int row = 0; row < size; row++)
            { // для кожного рядка порівнюємо обчислену ліву частину з даною правою, враховуючи можливу похибку
                if (Math.Abs(leftResults[row] - originalMatrix[row, size]) >= GaussSolver.EPS*100)
                // EPS * 100 = 1e-7 щоб перевірка проходилась легше
                {
                    isCorrect = false;
                    break;
                }
            }

            if (isCorrect)
            {
                WriteLineTo("Все гуд.", writer);
            }
            else
            {
                WriteLineTo("Неправильно обчислюю...", writer);
            }
        }
    }
}