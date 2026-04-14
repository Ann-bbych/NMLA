using System;
using System.Globalization;
using System.IO;

namespace ThomasAlgorithm
{
    public static class FileManager
    {
        public static string GetProjectDirectory()
        {
            return Path.GetFullPath(
                Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
        }

        public static string GetInputFileName()
        {
            return Path.Combine(GetProjectDirectory(), "input1.txt");
        }

        public static string GetOutput1FileName()
        {
            return Path.Combine(GetProjectDirectory(), "output1.txt");
        }

        public static string GetOutput2FileName()
        {
            return Path.Combine(GetProjectDirectory(), "output2.txt");
        }

        public static TridiagonalSystem ReadSystemFromFile(string inputFileName, double eps)
        {
            if (!File.Exists(inputFileName))
            {
                throw new FileNotFoundException($"Помилка: файл {Path.GetFileName(inputFileName)} не знайдено.");
            }

            string[] lines = File.ReadAllLines(inputFileName);

            if (lines.Length < 5)
            {
                throw new ArgumentException(
                    "Помилка: файл має містити 5 рядків: n, масив A, масив C, масив B, масив F.");
            }

            int n;
            if (!int.TryParse(lines[0].Trim(), out n))
            {
                throw new ArgumentException("Помилка: у першому рядку має бути ціле число n.");
            }

            double[] a = ParseDoubleArray(lines[1], n, "A");
            double[] c = ParseDoubleArray(lines[2], n, "C");
            double[] b = ParseDoubleArray(lines[3], n, "B");
            double[] f = ParseDoubleArray(lines[4], n, "F");

            return new TridiagonalSystem(n, a, c, b, f, eps);
        }

        private static double[] ParseDoubleArray(string line, int expectedLength, string arrayName)
        {
            string[] parts = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != expectedLength)
            {
                throw new ArgumentException(
                    $"Помилка: масив {arrayName} має містити рівно {expectedLength} елементів.");
            }

            double[] result = new double[expectedLength];

            for (int i = 0; i < expectedLength; i++)
            {
                if (!double.TryParse(parts[i], NumberStyles.Float, CultureInfo.InvariantCulture, out result[i]))
                {
                    throw new ArgumentException(
                        $"Помилка: у масиві {arrayName} елемент \"{parts[i]}\" не є коректним числом.");
                }
            }

            return result;
        }
    }
}