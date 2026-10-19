# Jefit Automation — дипломный проект (C#)

Фреймворк автоматизированного тестирования сайта [jefit.com](https://www.jefit.com/) — сервиса для планирования и учёта тренировок.

| Задание | Где реализовано |
|---|---|
| Фреймворк | `Jefit.Core` — конфигурация, DriverFactory, обёртки элементов, Page Objects, API-клиенты, Allure |
| Наборы UI-тестов | `Jefit.Test/Features` — 6 feature-файлов (SpecFlow), 31 сценарий |
| Наборы API-тестов | `Jefit.Test/Api` — 4 класса NUnit, 28 тестов |
| Allure | `Allure.NUnit` + `Allure.SpecFlow`: скриншоты при падении, HTTP-запросы во вложениях, категории, environment |
| CI/CD | `Jenkinsfile` + `Dockerfile` (Jenkins), `.github/workflows/tests.yml` (GitHub Actions) |
| *Доп.* Презентация архитектуры | `docs/ARCHITECTURE.md` |
| *Доп.* Описание тестов и видов тестирования | `docs/TEST_CASES.md` |
| *Доп.* Планы прогона: кросс-браузерный, параллельный | `runsettings/`, `scripts/`, параметры Jenkins, матрица GitHub Actions, `docs/RUN_PLANS.md` |
| *Доп.* BDD (SpecFlow) | UI-тесты на Gherkin |
| *Доп.* Данные для тестов через API (RestSharp) | `ApiPreparedData.feature`, `AccountData.feature` |

**Стек:** .NET 8 · NUnit 3 · SpecFlow 3.9 · Selenium WebDriver 4 · RestSharp · AngleSharp · Bogus · Allure · Jenkins / GitHub Actions

## Структура

```
JefitAutomation
├── Jefit.Core                        # фреймворк
│   ├── Api/                          # JefitWebClient, JefitAccountClient, BaseApiClient, HtmlPageParser
│   │   └── Models/                   # ExerciseInfo, RoutineInfo, PageMeta, модели аккаунта
│   ├── Configuration/                # TestSettings, ConfigReader
│   ├── Infrastructure/               # DriverFactory, AllureHelper, TestDataGenerator, XPathHelper ...
│   ├── PageObjects/                  # BasePage, HomePage, ExercisesPage, ExerciseDetailsPage,
│   │                                 # RoutinesPage, RoutineDetailsPage, LoginPage, GenericPage,
│   │                                 # FooterSection, CookieBanner
│   └── Wrappers/                     # BaseElement, Button, Link, Input, Label
├── Jefit.Test                        # тесты
│   ├── Api/                          # ApiTestBase + API-тесты (NUnit)
│   ├── Features/                     # UI-сценарии (SpecFlow)
│   ├── StepDefinitions/              # шаги
│   ├── Hooks/                        # браузер, скриншоты, Allure, пропуск @auth
│   ├── Support/                      # BrowserContext, ScenarioData
│   └── appsettings.json  specflow.json  allureConfig.json  categories.json
├── runsettings/                      # планы прогона
├── scripts/                          # кросс-браузерный запуск
├── docs/
├── Jenkinsfile  Dockerfile
├── .github/workflows/tests.yml
└── JefitAutomation.sln
```

## Запуск

```bash
dotnet build
dotnet test                                    # всё
dotnet test --filter "Category=api"            # API
dotnet test --filter "Category=ui"             # UI
dotnet test --filter "Category=smoke"          # smoke
dotnet test --settings runsettings/firefox.runsettings --filter "Category=ui"
```

Для `.feature`-файлов в Rider нужен плагин **SpecFlow for Rider** (Settings → Plugins).

## Настройки

Секция `TestSettings` в `Jefit.Test/appsettings.json`. Любой параметр переопределяется переменной окружения:

```bash
JEFIT_TestSettings__BrowserType=Firefox JEFIT_TestSettings__IsHeadless=true dotnet test
```

Логин и пароль тестового аккаунта не коммитьте — положите в `Jefit.Test/appsettings.local.json` (он в `.gitignore`) с такой же секцией `TestSettings`.

## Allure

```bash
dotnet test
allure serve Jefit.Test/bin/Debug/net8.0/allure-results
```

## CI/CD

**Jenkins.** Создайте Pipeline job из SCM с `Jenkinsfile`. Агент собирается из `Dockerfile` (.NET 8 SDK, Chromium, Firefox ESR). Нужны плагины Docker Pipeline и Allure Jenkins Plugin, а в *Global Tool Configuration* — установка Allure Commandline. Параметры сборки: набор тестов (all / smoke / api / ui), браузеры (Chrome, Firefox или оба — кросс-браузерный прогон), число потоков. Отчёт Allure появляется на странице сборки.

**GitHub Actions** (альтернатива): API-тесты, UI-тесты в матрице Chrome / Firefox / Edge, отчёт публикуется в GitHub Pages (Settings → Pages → ветка `gh-pages`).

## Тесты с авторизацией (`@auth`)

Пока не задан тестовый аккаунт, такие тесты помечаются **Ignored**. У Jefit нет публичного API, поэтому пути для `JefitAccountClient` берутся из браузера: DevTools (F12) → Network → Fetch/XHR → войти в аккаунт и создать план → скопировать пути запросов в `AccountLoginPath`, `AccountRoutinesPath`, адрес страницы своих планов — в `MyPlansPath`. При другом формате тела запросов поправьте `Api/Models/AccountModels.cs`.

## Ограничения

- Локаторы построены по реальной разметке jefit.com, но сайт меняется — при падении смотрите скриншот и HTML во вложениях Allure.
- Публичного REST API у Jefit нет, поэтому API-тесты проверяют HTTP-контракт сайта: статусы, заголовки, редиректы, SEO-теги и данные в серверном HTML.
- Edge в Jenkins-образ не включён (на Linux он тяжёлый); три браузера гоняются в GitHub Actions.
- SpecFlow больше не развивается, его преемник — Reqnroll (тот же синтаксис).
