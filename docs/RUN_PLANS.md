# Планы прогона

## Наборы по тегам

| Набор | Фильтр | Что входит |
|---|---|---|
| Smoke | `Category=smoke` | 16 тестов: 10 API и 6 UI, ключевые проверки |
| Регрессия | `Category=regression` | Все 59 тестов, включая smoke |
| Только API | `Category=api` | 28 тестов, браузер не нужен |
| Только UI | `Category=ui` | 31 сценарий |
| С аккаунтом | `Category=auth` | 4 теста, нужен тестовый аккаунт |

Регрессия включает smoke: тег `regression` стоит на всех тестах, а `smoke` только на ключевых.

## Файлы настроек (локально)

| План | Команда | Что делает |
|---|---|---|
| Smoke | `dotnet test --settings runsettings/smoke.runsettings` | Только smoke, 4 потока |
| Регрессия | `dotnet test --settings runsettings/regression.runsettings` | Фильтр `Category=regression`, 4 потока |
| Параллельный | `dotnet test --settings runsettings/parallel.runsettings` | 4 потока, свой браузер у сценария |
| Последовательный | `dotnet test --settings runsettings/sequential.runsettings` | 1 поток, видимый браузер: отладка и демонстрация |
| Chrome | `dotnet test --settings runsettings/chrome.runsettings --filter "Category=ui"` | UI-тесты в Chrome |
| Firefox | `dotnet test --settings runsettings/firefox.runsettings --filter "Category=ui"` | UI-тесты в Firefox |
| Оба браузера подряд | `./scripts/run-cross-browser.sh` или `.\scripts\run-cross-browser.ps1` | Chrome, затем Firefox, общий каталог результатов Allure |

Edge в проекте не используется: в облачном CI он открывал страницы сайта дольше минуты.

## Запуск в GitHub Actions

Запуск только вручную: **Actions → Jefit autotests → Run workflow**.

| Что запустить | Поле «Extra NUnit filter» |
|---|---|
| Все тесты | пусто |
| Только smoke | `Category=smoke` |

Задачи: API-тесты, UI-тесты в Chrome, UI-тесты в Firefox (идут одновременно на разных машинах), затем сборка Allure-отчёта и публикация на GitHub Pages.

При запуске только smoke в отчёте будет 10 API-тестов и по 6 UI-тестов в Chrome и в Firefox: UI-тесты идут в двух браузерах, поэтому запусков больше, чем тестов.

## Как устроена параллельность

Уровень параллельности: набор тестов (`ParallelScope.Fixtures`). Классы тестов и feature-файлы выполняются одновременно, тесты внутри набора последовательно. Каждый сценарий получает свой браузер и свои объекты через внедрение контекста SpecFlow, поэтому потоки не мешают друг другу. Число потоков задаётся `NumberOfTestWorkers` в `.runsettings`.

Для демонстрации на защите запустите `sequential.runsettings` и `parallel.runsettings` и сравните общее время в выводе `dotnet test`.

## Как устроен кросс-браузерный прогон

Браузер задаётся переменной `JEFIT_TestSettings__BrowserType` (Chrome или Firefox). В файлах `chrome.runsettings` и `firefox.runsettings` она прописана, в GitHub Actions задаётся матрицей. В Allure у каждого UI-теста есть параметр `Browser`, поэтому результаты браузеров не смешиваются.
