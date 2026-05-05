using System;

namespace JacobiMethod;

internal class Program
{
    static void Main()
    {
        string projectDirectory = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));

        string inputFileName = Path.Combine(projectDirectory, "input.txt");
        string outputFileName = Path.Combine(projectDirectory, "output.txt");

        using StreamWriter writer = new StreamWriter(outputFileName);

        try
        {
            AugmentedMatrix matrix = AugmentedMatrix.ReadFromFile(inputFileName);

            Formatter.WriteLineToBoth("", writer);
            Formatter.WriteLineToBoth("МЕТОД ЯКОБІ для розв'язування СЛАР", writer);
            Formatter.WriteLineToBoth("", writer);

            matrix.Print(writer);
            Formatter.WriteLineToBoth("", writer);

            matrix.CheckDegenerate();

            bool canUseJacobi = JacobiSolver.CheckDiagonalDominance(matrix, writer);

            if (!canUseJacobi)
            {
                Formatter.WriteLineToBoth("", writer);
                Formatter.WriteLineToBoth("Умови діагонального переважання не виконуються.", writer);
                Formatter.WriteLineToBoth("Не можна реалізовувати метод Якобі :(", writer);
                return;
            }

            Formatter.WriteLineToBoth("", writer);
            Formatter.WriteLineToBoth("Умови діагонального переважання виконуються.", writer);
            Formatter.WriteLineToBoth("Можна реалізовувати метод Якобі ;)", writer);
            Formatter.WriteLineToBoth("", writer);

            JacobiResult result = JacobiSolver.Solve(matrix, writer);

            JacobiSolver.PrintResult(result, writer);
        }
        catch (Exception ex)
        {
            Formatter.WriteLineToBoth("Помилка: " + ex.Message, writer);
        }
    }
}