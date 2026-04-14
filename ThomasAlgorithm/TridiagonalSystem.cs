using System;

namespace ThomasAlgorithm
{
    public class TridiagonalSystem
    {
        public int N { get; }
        public double[] A { get; }
        public double[] C { get; }
        public double[] B { get; }
        public double[] F { get; }

        public TridiagonalSystem(int n, double[] a, double[] c, double[] b, double[] f, double eps)
        {
            if (n < 2)
            {
                throw new ArgumentException("Помилка: кількість рівнянь має бути >= 2.");
            }

            if (a == null || c == null || b == null || f == null)
            {
                throw new ArgumentException("Помилка: один або кілька масивів не задано.");
            }

            if (a.Length != n || c.Length != n || b.Length != n || f.Length != n)
            {
                string message = "Помилка: неправильні розміри масивів.\n";

                if (a.Length != n)
                    message += $"Масив A має {a.Length}, а треба {n}.\n";

                if (c.Length != n)
                    message += $"Масив C має {c.Length}, а треба {n}.\n";

                if (b.Length != n)
                    message += $"Масив B має {b.Length}, а треба {n}.\n";

                if (f.Length != n)
                    message += $"Масив F має {f.Length}, а треба {n}.\n";

                throw new ArgumentException(message);
            }

            if (Math.Abs(a[0]) > eps)
            {
                throw new ArgumentException("Помилка: перший елемент масиву A має бути 0.");
            }

            if (Math.Abs(b[n - 1]) > eps)
            {
                throw new ArgumentException("Помилка: останній елемент масиву B має бути 0.");
            }

            N = n;
            A = CopyArray(a);
            C = CopyArray(c);
            B = CopyArray(b);
            F = CopyArray(f);
        }

        private static double[] CopyArray(double[] source) // для конструктора
        {
            double[] copy = new double[source.Length];

            for (int i = 0; i < source.Length; i++)
            {
                copy[i] = source[i];
            }

            return copy;
        }
    }
}
