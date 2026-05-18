using System;
using System.IO;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        string projectDirectory = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));

        string inputFileName = Path.Combine(projectDirectory, "input.txt");
        string outputFileName = Path.Combine(projectDirectory, "output.txt");

        using StreamWriter writer = new StreamWriter(outputFileName);

        try
        {
            PMInputData data = PMInputData.ReadFrom(inputFileName);
            int digits = Formatter.CountDigitsAfterPoint(data.EpsilonText);

            PMResult result = PMSolver.Solve(data);
            VerificationResult verification = Verifier.Verify(data, result);

            data.Print(writer);
            result.Print(writer, digits);
            verification.Print(writer, digits);
        }
        catch (Exception ex)
        {
            Formatter.WriteLineTo("Помилка: " + ex.Message, writer);
        }
    }
}