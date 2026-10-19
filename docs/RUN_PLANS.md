# Планы прогона

| План | Как запустить |
|---|---|
| Smoke | `dotnet test --settings runsettings/smoke.runsettings` |
| Регрессия | `dotnet test --settings runsettings/regression.runsettings` |
| Параллельный (4 потока) | `dotnet test --settings runsettings/parallel.runsettings` |
| Последовательный (1 поток, видимый браузер) | `dotnet test --settings runsettings/sequential.runsettings` |
| Один браузер | `dotnet test --settings runsettings/firefox.runsettings --filter "Category=ui"` |
| Кросс-браузерный локально | `./scripts/run-cross-browser.sh` или `.\scripts\run-cross-browser.ps1` |
| Кросс-браузерный в Jenkins | параметр `BROWSERS = Chrome Firefox` |
| Кросс-браузерный в GitHub Actions | матрица Chrome / Firefox / Edge (браузеры идут одновременно) |
| Только API | `dotnet test --filter "Category=api"` |
| По фиче / важности | `--filter "Category=story:Exercise_Database"`, `--filter "Category=critical"` |

**Параллельность.** `[assembly: Parallelizable(ParallelScope.Fixtures)]`: классы тестов и feature-файлы выполняются одновременно, тесты внутри — последовательно. Число потоков — `NumberOfTestWorkers` в `.runsettings` (в Jenkins — параметр `THREADS`). Для защиты удобно показать разницу: запустить `sequential` и `parallel` и сравнить время, а в Allure открыть вкладку Timeline.

**Кросс-браузерность.** Браузер задаётся переменной `JEFIT_TestSettings__BrowserType`: в `chrome/firefox/edge.runsettings`, в Jenkins через `withEnv`, в GitHub Actions через матрицу. В Allure у UI-тестов есть параметр `Browser`, поэтому результаты разных браузеров не смешиваются.
