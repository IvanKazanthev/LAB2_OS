using System;
using System.Threading;

namespace OS_LAB2;

public static class MatrixMultiplier
{
    public static double[,] Multiply(
        double[,] A,
        double[,] B,
        int threadCount,
        bool showProgress = true)
    {
        int rowsA = A.GetLength(0);
        int columnsA = A.GetLength(1);
        int rowsB = B.GetLength(0);
        int columnsB = B.GetLength(1);

        if (columnsA != rowsB)
            throw new ArgumentException("Несовместимые размеры матриц.");

        if (threadCount < 1)
            throw new ArgumentException("Количество потоков должно быть не меньше 1.");

        double[,] result = new double[rowsA, columnsB];

        int completedRows = 0;
        int finishedWorkers = 0;

        Thread[] threads = new Thread[threadCount];

        // Отдельный поток отображения прогресса
        Thread? progressThread = null;

        if (showProgress)
        {
            progressThread = new Thread(() =>
            {
                int lastPercent = -1;

                while (Volatile.Read(ref finishedWorkers) < threadCount)
                {
                    int completed = Volatile.Read(ref completedRows);
                    int percent = completed * 100 / rowsA;

                    if (percent != lastPercent)
                    {
                        Console.WriteLine($"Прогресс: {percent}%");
                        Console.Out.Flush();
                        lastPercent = percent;
                    }

                    Thread.Sleep(200);
                }

                Console.WriteLine("Прогресс: 100%");
                Console.Out.Flush();
            });

            progressThread.Start();
        }

        int baseRows = rowsA / threadCount;
        int remainder = rowsA % threadCount;

        int currentRow = 0;

        for (int t = 0; t < threadCount; t++)
        {
            int startRow = currentRow;
            int rowsForThread = baseRows + (t < remainder ? 1 : 0);
            int endRow = startRow + rowsForThread;

            currentRow = endRow;

            threads[t] = new Thread(() =>
            {
                try
                {
                    for (int i = startRow; i < endRow; i++)
                    {
                        for (int j = 0; j < columnsB; j++)
                        {
                            double sum = 0;

                            for (int k = 0; k < columnsA; k++)
                            {
                                sum += A[i, k] * B[k, j];
                            }

                            result[i, j] = sum;
                        }

                        Interlocked.Increment(ref completedRows);
                    }
                }
                finally
                {
                    Interlocked.Increment(ref finishedWorkers);
                }
            });

            threads[t].Start();
        }

        foreach (Thread thread in threads)
        {
            thread.Join();
        }

        if (progressThread != null)
        {
            progressThread.Join();
        }

        return result;
    }
}