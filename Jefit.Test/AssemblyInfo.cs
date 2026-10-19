using NUnit.Framework;

// Параллельный запуск: каждый тестовый класс / feature-файл — отдельный поток.
// Тесты внутри одного класса идут последовательно. Число потоков меняется
// без перекомпиляции через NumberOfTestWorkers в .runsettings.
[assembly: Parallelizable(ParallelScope.Fixtures)]
[assembly: LevelOfParallelism(4)]
