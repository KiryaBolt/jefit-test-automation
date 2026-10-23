# Описание тестов и видов тестирования

Всего 59 тестов: 31 UI-сценарий (SpecFlow) и 28 API-тестов (NUnit). Smoke: 16 тестов. Регрессия: все 59, она включает smoke. Четыре теста требуют тестовый аккаунт (помечены *).

## Виды тестирования

| Вид | Как представлен |
|---|---|
| Функциональное UI | Навигация, база упражнений, фильтры, пагинация, планы тренировок, вход |
| API / HTTP | Статусы, заголовки, редиректы, содержимое HTML, вход через API |
| Smoke | Тег `smoke`: ключевые страницы и функции |
| Регрессионное | Тег `regression`: полный набор, включая smoke |
| Позитивное и негативное | Успешные переходы и вход, неверный вход, пустая форма, страница 404 |
| Кросс-браузерное | Те же UI-тесты в Chrome и Firefox |
| Нефункциональное | Время ответа страниц, редирект с HTTP на HTTPS |
| SEO | title, og:title, canonical, заголовок H1 |
| Данные из параметров | `Scenario Outline` в SpecFlow и `[TestCase]` в NUnit |
| Интеграционное API + UI | Данные получены или созданы через API и проверены в интерфейсе |

Техники тест-дизайна: классы эквивалентности (разделы сайта, группы мышц, категории планов), негативные данные, случайные данные (Bogus, случайный элемент из ответа API).

Важность тестов: `blocker`, `critical`, `normal`, `minor`.

## UI-тесты (31)

В скобках указано, как тест называется в окне тестов Rider.

### Навигация (12)
| Тест | Теги |
|---|---|
| Главная страница открывается с главным заголовком (`HomePageOpensWithTheMainHeading`) | smoke, critical |
| Кнопка «Get Started» ведёт на страницу регистрации (`GetStartedButtonLeadsToTheSignUpPage`) | smoke, critical |
| Ссылки в подвале открывают нужные страницы, 6 вариантов: база упражнений, планы тренировок, вход, регистрация, блог, «Часто задаваемые вопросы» (подпись на сайте FAQ) (`FooterLinksOpenTheCorrespondingPages`) | normal |
| В подвале есть ссылки на соцсети Jefit, 4 варианта: Facebook, Instagram, X, Reddit (`FooterContainsLinksToJefitSocialNetworks`) | minor |

### База упражнений (7)
| Тест | Теги |
|---|---|
| Страница базы упражнений показывает упражнения (`ExerciseDatabasePageDisplaysExercises`) | smoke, critical |
| Фильтр по группе мышц, 3 варианта: грудь, спина, бицепс (`FilteringExercisesByMuscleGroup`) | normal |
| Переход на вторую страницу упражнений (`NavigatingToTheSecondPageOfExercises`) | normal |
| Открытие упражнения из списка (`OpeningExerciseDetailsFromTheList`) | smoke, critical |
| Страница упражнения содержит все информационные блоки (`ExerciseDetailsPageContainsAllInformationBlocks`) | normal |

### Планы тренировок (5)
| Тест | Теги |
|---|---|
| Страница каталога планов показывает планы (`WorkoutPlansPageDisplaysPlans`) | smoke, critical |
| Открытие категории планов, 3 варианта: для начинающих, домашние, набор массы (`OpeningAWorkoutPlanCategory`) | normal |
| Открытие плана из списка (`OpeningWorkoutPlanDetailsFromTheList`) | normal |

### Вход (4)
| Тест | Теги |
|---|---|
| Форма входа отображается (`LoginFormIsDisplayed`) | smoke, critical |
| Вход с неверными данными отклоняется (`LoginWithInvalidCredentialsIsRejected`) | critical |
| Вход с пустыми полями невозможен (`LoginWithEmptyFieldsIsNotPossible`) | normal |
| Успешный вход с верными данными (`SuccessfulLoginWithValidCredentials`) * | auth, blocker |

### Данные из API (2)
| Тест | Теги |
|---|---|
| Случайное упражнение, полученное через API, корректно отображается в интерфейсе (`RandomExerciseReceivedViaAPIIsDisplayedCorrectlyInUI`) | normal |
| Случайный план, полученный через API, корректно отображается в интерфейсе (`RandomWorkoutPlanReceivedViaAPIIsDisplayedCorrectlyInUI`) | normal |

### Аккаунт (1)
| Тест | Теги |
|---|---|
| План, добавленный через API, виден в «Моих планах» (`WorkoutPlanAddedViaAPIIsVisibleInUI`) * | auth, normal |

Все UI-сценарии дополнительно помечены тегами `ui` и `regression`.

## API-тесты (28)

Названия в окне тестов совпадают с названиями методов. Все API-тесты помечены тегами `api` и `regression`.

### Публичные страницы (11)
| Тест | Теги |
|---|---|
| Страница доступна, 8 вариантов: `/`, `/exercises`, `/routines`, `/login`, `/signup`, `/elite`, `/blog`, `/support/faq` (`PublicPage_ShouldBeAvailable`) | smoke, critical |
| Несуществующая страница возвращает 404 (`NotExistingPage_ShouldReturn404`) | normal |
| Адрес по http перенаправляет на https (`HttpRequest_ShouldRedirectToHttps`) | critical |
| Файл robots.txt доступен (`RobotsTxt_ShouldBeAvailable`) | minor |

### База упражнений (9)
| Тест | Теги |
|---|---|
| Список упражнений содержит упражнения (`ExerciseList_ShouldContainExercises`) | smoke, critical |
| У страницы упражнения верные SEO-данные, 3 упражнения (`ExercisePage_ShouldHaveCorrectSeoData`) | normal |
| Пагинация возвращает разные упражнения (`Pagination_ShouldReturnDifferentExercises`) | normal |
| Страницы групп мышц содержат упражнения, 4 варианта (`MuscleGroupPage_ShouldContainExercises`) | normal |

### Планы тренировок (6)
| Тест | Теги |
|---|---|
| Список планов содержит планы (`RoutineList_ShouldContainPlans`) | smoke, critical |
| Категории планов содержат планы, 5 вариантов (`RoutineCategory_ShouldContainPlans`) | normal |

### Аккаунт (2)
| Тест | Теги |
|---|---|
| Вход через API с верными данными проходит успешно (`Login_WithValidCredentials_ShouldSucceed`) * | auth, critical |
| Вход через API со случайными данными отклоняется (`Login_WithRandomCredentials_ShouldBeRejected`) * | auth, normal |
