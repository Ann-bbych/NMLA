using System;
using System.IO;

namespace ThomasAlgorithm
{
    public class Program
    {
        private const double EPS = 1e-9;

        public static void Main()
        {
            try
            {
                Console.Write("Режими роботи:\n");
                Console.Write("1 - розв'язання СЛАР з файлу\n");
                Console.Write("2 - перевірка на крайовій задачі з відомим розв'язком, обчислення похибки\n");
                Console.Write("Виберіть режим: ");
                string? input = Console.ReadLine();
                Console.WriteLine();

                if (!int.TryParse(input, out int mode))
                {
                    Console.WriteLine("Помилка: потрібно ввести число 1 або 2.");
                    return;
                }

                switch (mode)
                {
                    case 1:
                        using (StreamWriter writer = new StreamWriter(FileManager.GetOutput1FileName()))
                        {
                            RunMode1(writer);
                        }
                        break;

                    case 2:
                        using (StreamWriter writer = new StreamWriter(FileManager.GetOutput2FileName()))
                        {
                            RunMode2(writer);
                        }
                        break;

                    default:
                        Console.WriteLine("Помилка: неправильно введений вибір.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void RunMode1(StreamWriter writer)
        {
            string inputFileName = FileManager.GetInputFileName();

            TridiagonalSystem system = FileManager.ReadSystemFromFile(inputFileName, EPS);

            Formatter.PrintSystem(system, writer, "Вхідна СЛАР:", EPS);

            bool conditionsSatisfied = LeftSweepSolver.CheckNecessaryConditions(system, EPS, out string message);
            Formatter.PrintConditionsResult(message, writer);

            if (!conditionsSatisfied)
            {
                return;
            }

            double[] solution = LeftSweepSolver.Solve(system, EPS);

            Formatter.PrintSolution(solution, writer, "Розв'язок СЛАР:", EPS);

            Verifier.PrintVerification(system, solution, writer, EPS);
        }

        private static void RunMode2(StreamWriter writer)
        {
            Formatter.PrintBoundaryValueProblem(writer);

            TridiagonalSystem system = BoundaryValueProblem.BuildSystem(EPS);
            Formatter.PrintSystemMode2(system, writer, "СЛАР для крайової задачі:", EPS);

            double[] exactSolution = BoundaryValueProblem.GetExactSolutionVector();
            Formatter.PrintSolutionMode2(exactSolution, writer, "Точний розв'язок СЛАР:", EPS);

            bool conditionsSatisfied = LeftSweepSolver.CheckNecessaryConditions(system, EPS, out string message);
            Formatter.PrintConditionsResult(message, writer);

            if (!conditionsSatisfied)
            {
                return;
            }

            double[] numericalSolution = LeftSweepSolver.Solve(system, EPS);
            double[] errors = BoundaryValueProblem.GetErrors(exactSolution, numericalSolution);

            Formatter.PrintSolutionMode2(numericalSolution, writer, "Програмний розв'язок СЛАР:", EPS);
            Formatter.PrintErrors(exactSolution, numericalSolution, errors, writer, EPS);
        }
    }
}
