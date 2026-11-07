using System;
using System.Diagnostics;

namespace OS_LAB2;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            // Режим исследования производительности
            if (args.Length == 1 && args[0] == "benchmark")
            {
                Benchmark.Run();
                return;
            }

            int n, m, k, threadCount;

            if (args.Length == 4)
            {
                n = int.Parse(args[0]);
                m = int.Parse(args[1]);
                k = int.Parse(args[2]);
                threadCount = int.Parse(args[3]);
            }
            else
            {
                Console.Write("Количество строк A (n): ");
                n = int.Parse(Console.ReadLine()!);

                Console.Write("Количество столбцов A (m): ");
                m = int.Parse(Console.ReadLine()!);

                Console.Write("Количество столбцов B (k): ");
                k = int.Parse(Console.ReadLine()!);

                Console.Write("Количество потоков (T): ");
                threadCount = int.Parse(Console.ReadLine()!);
            }

            if (n <= 0 || m <= 0 || k <= 0 || threadCount <= 0)
            {
                throw new ArgumentException(
                    "Все размеры и количество потоков должны быть положительными."
                );
            }

            Console.WriteLine("\nСоздание матриц...");

            double[,] A = Matrix.CreateRandom(n, m);
            double[,] B = Matrix.CreateRandom(m, k);

            Console.WriteLine("Матрицы созданы.");

            Console.WriteLine($"Размер A: {n} x {m}");
            Console.WriteLine($"Размер B: {m} x {k}");
            Console.WriteLine($"Количество потоков: {threadCount}");

            Stopwatch stopwatch = Stopwatch.StartNew();

            double[,] C = MatrixMultiplier.Multiply(
                A,
                B,
                threadCount
            );

            stopwatch.Stop();

            // Проверка результата однопоточным алгоритмом
            double[,] reference = Matrix.MultiplySequential(A, B);

            bool isCorrect = Matrix.AreEqual(C, reference);

            Console.WriteLine("\nУмножение завершено!");

            Console.WriteLine(
                $"Время выполнения: {stopwatch.Elapsed.TotalSeconds:F4} сек."
            );

            Console.WriteLine(
                isCorrect
                    ? "Проверка корректности: успешно."
                    : "Ошибка: результаты не совпадают!"
            );

            Console.WriteLine(
                $"Контрольная сумма: {Matrix.Checksum(C):F2}"
            );

            if (n <= 10 && k <= 10)
            {
                Console.WriteLine("\nМатрица A:");
                Matrix.Print(A);

                Console.WriteLine("\nМатрица B:");
                Matrix.Print(B);

                Console.WriteLine("\nРезультат C:");
                Matrix.Print(C);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}