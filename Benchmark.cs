using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace OS_LAB2;

public static class Benchmark
{
    public static void Run()
    {
        int logicalCores = Environment.ProcessorCount;

        // Три размера квадратных матриц
        int[] sizes = { 500, 1000, 2000 };

        // Количество потоков: от 1 до 2x логических процессоров,
        // дополнительно 4x и 8x
        List<int> threadCounts = Enumerable
            .Range(1, logicalCores * 2)
            .ToList();

        threadCounts.Add(logicalCores * 4);
        threadCounts.Add(logicalCores * 8);

        threadCounts = threadCounts
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        Console.WriteLine();
        Console.WriteLine("ИССЛЕДОВАНИЕ ПРОИЗВОДИТЕЛЬНОСТИ");
        Console.WriteLine();

        Console.WriteLine($"Логических процессоров: {logicalCores}");
        Console.WriteLine($"Размеры матриц: {string.Join(", ", sizes)}");
        Console.WriteLine(
            $"Количество потоков: {string.Join(", ", threadCounts)}");

        // Создание папки для результатов
        Directory.CreateDirectory("Report");

        string filePath = Path.Combine("Report", "benchmark.csv");

        using StreamWriter writer = new StreamWriter(filePath);

        // Заголовок CSV
        writer.WriteLine(
            "Size;Threads;Time1;Time2;Time3;Time4;Time5;" +
            "AverageTimeSeconds;Speedup;Efficiency"
        );

        foreach (int size in sizes)
        {
            Console.WriteLine();
            Console.WriteLine($"--- Размер {size} x {size} ---");

            // Создание матриц вне измеряемого участка
            double[,] A = Matrix.CreateRandom(size, size);
            double[,] B = Matrix.CreateRandom(size, size);

            // Прогрев JIT перед измерениями
            MatrixMultiplier.Multiply(A, B, 1, false);

            double singleThreadTime = 0;

            foreach (int threadCount in threadCounts)
            {
                List<double> measurements = new List<double>();

                Console.WriteLine();
                Console.WriteLine($"Количество потоков: {threadCount}");

                // Пять измерений
                for (int repetition = 1; repetition <= 5; repetition++)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();

                    double[,] result = MatrixMultiplier.Multiply(
                        A,
                        B,
                        threadCount,
                        false
                    );

                    stopwatch.Stop();

                    double seconds = stopwatch.Elapsed.TotalSeconds;

                    // Проверка результата вне измеряемого участка
                    double checksum = Matrix.Checksum(result);

                    measurements.Add(seconds);

                    Console.WriteLine(
                        $"Замер {repetition}: {seconds:F4} сек. | " +
                        $"Контрольная сумма: {checksum:F2}"
                    );
                }

                // Сохраняем все пять исходных результатов
                List<double> rawMeasurements =
                    new List<double>(measurements);

                // Сортировка результатов
                measurements.Sort();

                // Удаление минимального и максимального значений
                measurements.RemoveAt(measurements.Count - 1);
                measurements.RemoveAt(0);

                // Среднее значение трёх оставшихся измерений
                double average = measurements.Average();

                // Время однопоточного выполнения
                if (threadCount == 1)
                {
                    singleThreadTime = average;
                }

                // Расчёт ускорения и эффективности
                double speedup = singleThreadTime / average;
                double efficiency = speedup / threadCount;

                Console.WriteLine(
                    $"Среднее время: {average:F4} сек."
                );

                Console.WriteLine(
                    $"Ускорение: {speedup:F2}"
                );

                Console.WriteLine(
                    $"Эффективность: {efficiency:F2}"
                );

                // Формирование строки CSV
                string csvLine = string.Join(";",
                    new[]
                    {
                        size.ToString(CultureInfo.InvariantCulture),
                        threadCount.ToString(CultureInfo.InvariantCulture),

                        string.Join(";",
                            rawMeasurements.Select(x =>
                                x.ToString("F6", CultureInfo.InvariantCulture))),

                        average.ToString("F6", CultureInfo.InvariantCulture),
                        speedup.ToString("F4", CultureInfo.InvariantCulture),
                        efficiency.ToString("F4", CultureInfo.InvariantCulture)
                    }
                );

                writer.WriteLine(csvLine);
                writer.Flush();
            }
        }

        Console.WriteLine();
        Console.WriteLine("ИССЛЕДОВАНИЕ ЗАВЕРШЕНО");
        Console.WriteLine($"Результаты сохранены: {filePath}");
        Console.WriteLine();
    }
}