using System;

namespace OS_LAB2;

public static class Matrix
{
    public static double[,] CreateRandom(int rows, int columns)
    {
        Random random = new Random();
        double[,] matrix = new double[rows, columns];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                matrix[i, j] = random.Next(0, 10);
            }
        }

        return matrix;
    }

    public static double Checksum(double[,] matrix)
    {
        double sum = 0;

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                sum += matrix[i, j];
            }
        }

        return sum;
    }

    public static void Print(double[,] matrix)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                Console.Write($"{matrix[i, j],8:F2}");
            }

            Console.WriteLine();
        }
    }

    // Однопоточное умножение для проверки результата
    public static double[,] MultiplySequential(double[,] A, double[,] B)
    {
        int rowsA = A.GetLength(0);
        int columnsA = A.GetLength(1);
        int columnsB = B.GetLength(1);

        double[,] result = new double[rowsA, columnsB];

        for (int i = 0; i < rowsA; i++)
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
        }

        return result;
    }

    // Поэлементное сравнение двух матриц
    public static bool AreEqual(double[,] A, double[,] B)
    {
        if (A.GetLength(0) != B.GetLength(0) ||
            A.GetLength(1) != B.GetLength(1))
        {
            return false;
        }

        for (int i = 0; i < A.GetLength(0); i++)
        {
            for (int j = 0; j < A.GetLength(1); j++)
            {
                if (Math.Abs(A[i, j] - B[i, j]) > 0.000001)
                {
                    return false;
                }
            }
        }

        return true;
    }
}