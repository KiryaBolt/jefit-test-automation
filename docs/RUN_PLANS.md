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

## Как понять по результатам, какой набор запускали

Число тестов в каждой задаче берётся из итоговой строки `Total` в логе шага `Run ... tests` (Actions → запуск → задача → шаг).

| Запуск | API tests | UI tests (chrome) | UI tests (firefox) | Запусков в отчёте Allure |
|---|---|---|---|---|
| Только smoke (`Category=smoke`) | 10 | 6 | 6 | 22 |
| Все тесты (поле пустое) | 28 | 31 | 31 | 90 |

В отчёте Allure на вкладке Suites или Behaviors список тестов должен совпадать с набором. В smoke-запуске есть только 16 тестов: 8 вариантов `PublicPage_ShouldBeAvailable`, `ExerciseList_ShouldContainExercises`, `RoutineList_ShouldContainPlans` и 6 UI-сценариев (главная страница, Get Started, база упражнений, открытие упражнения, каталог планов, форма входа), каждый в Chrome и в Firefox. Любой другой тест, например `RobotsTxt_ShouldBeAvailable` или ссылки подвала, означает, что запуск был полным.

Число 90 больше числа тестов (59), потому что UI-сценарии выполняются в двух браузерах: 28 + 31 × 2 = 90. В Allure у UI-теста есть параметр `Browser`, по нему запуски разных браузеров различаются.

## История запусков

Каждый запуск workflow добавляет точку в историю отчёта: на главной странице отчёта виден график Trend, у каждого теста есть вкладка History со статусами прошлых запусков. История берётся из ветки `gh-pages` и обновляется при каждой публикации отчёта. Запуски с разными фильтрами попадают в одну историю.

## Как устроена параллельность

Уровень параллельности: набор тестов (`ParallelScope.Fixtures`). Классы тестов и feature-файлы выполняются одновременно, тесты внутри набора последовательно. Каждый сценарий получает свой браузер и свои объекты через внедрение контекста SpecFlow, поэтому потоки не мешают друг другу. Число потоков задаётся `NumberOfTestWorkers` в `.runsettings`.

Для демонстрации на защите запустите `sequential.runsettings` и `parallel.runsettings` и сравните общее время в выводе `dotnet test`.

## Как устроен кросс-браузерный прогон

Браузер задаётся переменной `JEFIT_TestSettings__BrowserType` (Chrome или Firefox). В файлах `chrome.runsettings` и `firefox.runsettings` она прописана, в GitHub Actions задаётся матрицей. В Allure у каждого UI-теста есть параметр `Browser`, поэтому результаты браузеров не смешиваются.
