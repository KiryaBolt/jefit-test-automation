# Jefit Automation — дипломный проект (C#)

Фреймворк автоматизированного тестирования сайта [jefit.com](https://www.jefit.com/) — сервиса для планирования и учёта тренировок. UI-тесты написаны на SpecFlow (BDD), API-тесты на NUnit, отчёты строит Allure, запуск в GitHub Actions.

## Что требовалось и где это сделано

| Задание | Где реализовано |
|---|---|
| Фреймворк | `Jefit.Core`: настройки, драйверы, обёртки элементов, Page Objects, API-клиенты, генерация данных, Allure |
| Наборы UI-тестов | `Jefit.Test/Features`: 6 наборов, 31 сценарий (SpecFlow) |
| Наборы API-тестов | `Jefit.Test/Api`: 4 класса, 28 тестов (NUnit) |
| Allure | Скриншоты и HTML страницы при падении, HTTP-запросы в отчёте, категории ошибок, параметр Browser |
| CI/CD | GitHub Actions: `.github/workflows/tests.yml`, отчёт на GitHub Pages |
| *Доп.* Презентация архитектуры | `docs/ARCHITECTURE.md` и отдельная презентация |
| *Доп.* Описание тестов и видов тестирования | `docs/TEST_CASES.md` |
| *Доп.* Планы прогона | `runsettings/`, `scripts/`, `docs/RUN_PLANS.md`: параллельный, кросс-браузерный (Chrome и Firefox), smoke |
| *Доп.* BDD | SpecFlow, сценарии на Gherkin |
| *Доп.* Данные для тестов через API | RestSharp: случайное упражнение и план из API, добавление плана в аккаунт через API |

**Стек:** .NET 8, C#, NUnit 3, SpecFlow 3.9, Selenium WebDriver 4, RestSharp, AngleSharp, Bogus, Allure, GitHub Actions.

## Тесты в цифрах

| | Количество |
|---|---|
| UI-сценарии | 31 |
| API-тесты | 28 |
| **Всего** | **59** |
| Smoke (быстрый набор) | 16: 10 API и 6 UI |
| Регрессия | все 59, **регрессия включает smoke** |
| Нужен тестовый аккаунт | 4 теста (остальные 55 работают без него) |

## Структура проекта

```
JefitAutomation
├── Jefit.Core                        # фреймворк
│   ├── Api/                          # JefitWebClient, JefitAccountClient, BaseApiClient, HtmlPageParser
│   │   └── Models/                   # ExerciseInfo, RoutineInfo, PageMeta, модели аккаунта
│   ├── Configuration/                # TestSettings, ConfigReader
│   ├── Infrastructure/               # DriverFactory, AllureHelper, TestDataGenerator, SessionTransfer ...
│   ├── PageObjects/                  # BasePage, HomePage, ExercisesPage, RoutinesPage, LoginPage ...
│   └── Wrappers/                     # BaseElement, Button, Input, Label, Link
├── Jefit.Test                        # тесты
│   ├── Api/                          # ApiTestBase и API-тесты (NUnit)
│   ├── Features/                     # UI-сценарии (SpecFlow)
│   ├── StepDefinitions/              # шаги сценариев
│   ├── Hooks/                        # браузер, скриншоты, Allure, пропуск тестов с аккаунтом
│   ├── Support/                      # BrowserContext, ScenarioData
│   └── appsettings.json  specflow.json  allureConfig.json  categories.json
├── runsettings/                      # планы прогона
├── scripts/                          # кросс-браузерный запуск
├── docs/                             # архитектура, тесты, планы прогона
├── .github/workflows/tests.yml       # CI
└── JefitAutomation.sln
```

## Запуск

Откройте `JefitAutomation.sln` в Rider или запускайте из терминала в папке проекта:

```bash
dotnet test                                                   # все тесты
dotnet test --filter "Category=smoke"                         # smoke: 16 тестов
dotnet test --filter "Category=regression"                    # регрессия: все 59
dotnet test --filter "Category=api"                           # только API
dotnet test --filter "Category=ui"                            # только UI
dotnet test --filter "Category=smoke&Category=api"            # smoke только API (10)
dotnet test --filter "Category=smoke&Category=ui"             # smoke только UI (6)
dotnet test --filter "FullyQualifiedName~LoginFeature"        # один набор по имени
dotnet test --filter "Category=auth"                          # тесты с аккаунтом
```

Флаг `--list-tests` игнорирует `--filter` и всегда показывает все тесты, поэтому проверять наборы нужно реальным запуском.

Если в Rider кнопка запуска стала неактивной после прерванного прогона, выполните `pkill -f chromedriver` и `pkill -f testhost` или запускайте тесты из терминала.

## Настройки

Секция `TestSettings` в `Jefit.Test/appsettings.json`. Любой параметр переопределяется переменной окружения:

```bash
JEFIT_TestSettings__BrowserType=Firefox JEFIT_TestSettings__IsHeadless=true dotnet test --filter "Category=ui"
```

## Тесты с аккаунтом

Четыре теста работают от имени тестового аккаунта Jefit: успешный вход в UI, вход через API с верными и со случайными данными, добавление плана через API с проверкой в UI. Без аккаунта они автоматически пропускаются.

1. Создайте отдельный тестовый аккаунт Jefit (не личный).
2. В папке `Jefit.Test` создайте файл `appsettings.local.json`:
```json
{
  "TestSettings": {
    "UserEmail": "email тестового аккаунта",
    "UserPassword": "пароль тестового аккаунта"
  }
}
```
Файл указан в `.gitignore` и не попадает в репозиторий. В `Jefit.Test.csproj` должна быть строка `<None Update="appsettings.local.json" CopyToOutputDirectory="PreserveNewest" Condition="Exists('appsettings.local.json')" />`.
3. В GitHub добавьте секреты `JEFIT_EMAIL` и `JEFIT_PASSWORD` (Settings → Secrets and variables → Actions).

Тест с планом добавляет в аккаунт готовый план из каталога, проверяет его в «Моих планах» и удаляет в завершающем хуке, в том числе при падении теста.

## Allure-отчёт

Локально (нужны Allure и Java):
```bash
brew install allure
dotnet test
allure serve Jefit.Test/bin/Debug/net8.0/allure-results
```

В отчёте: группировка по Epic, Feature и Story, важность, шаги, скриншот, адрес и HTML страницы при падении UI-теста, каждый HTTP-запрос с ответом и временем, категории ошибок и параметр Browser. Токены и логины в ответах запросов заменяются на `***`, потому что отчёт публичный.

## CI/CD (GitHub Actions)

Запуск только вручную: **Actions → Jefit autotests → Run workflow**. Поле «Extra NUnit filter»: пустое запускает все тесты, `Category=smoke` только smoke.

Задачи: API-тесты, UI-тесты в Chrome, UI-тесты в Firefox, затем сборка Allure-отчёта и публикация на GitHub Pages (отчёт собирается, даже если тесты упали). Включение страницы отчёта: Settings → Pages → ветка `gh-pages`, папка `/ (root)`.

Отчёт: https://kiryabolt.github.io/jefit-test-automation/

## Ограничения

- У Jefit нет публичного REST API, поэтому API-тесты публичной части проверяют HTTP-контракт сайта: статусы, заголовки, редиректы, SEO-теги, данные в серверном HTML. Эндпоинты входа и планов найдены через инструменты разработчика браузера.
- Сайт иногда отвечает медленно. Поэтому в фреймворке заданы ограничения времени, повтор запроса при отсутствии ответа и перезагрузка страницы, если ключевой элемент не появился.
- Edge не используется: в облачном CI на Linux он открывал страницы дольше минуты. В CI работают Chrome и Firefox.
- SpecFlow закрыт с 31 декабря 2024 года. Его преемник Reqnroll совместим с ним по синтаксису; переход сводится к замене пакетов и пространства имён `TechTalk.SpecFlow` на `Reqnroll`.
