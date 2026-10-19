# Описание тестов и видов тестирования

## Виды тестирования

| Вид | Как представлен |
|---|---|
| Функциональное UI (end-to-end) | Навигация, база упражнений, фильтры, пагинация, планы, логин |
| API / HTTP | Статус-коды, Content-Type, заголовки, редиректы, данные в HTML |
| Smoke | Категория `smoke` — ключевые страницы и функции |
| Регрессионное | Полный набор, ночной прогон в CI |
| Позитивное / негативное | Успешные переходы и логин / неверные данные, пустая форма, 404 |
| Кросс-браузерное | Те же UI-сценарии в Chrome, Firefox, Edge |
| Производительность (базово) | Время ответа страниц ≤ `MaxResponseTimeMs` |
| Безопасность (базово) | Редирект HTTP → HTTPS |
| SEO | title, og:title, description, canonical, H1 |
| Data-driven | `[TestCase]` в NUnit, `Scenario Outline` в SpecFlow |
| Интеграционное API + UI | Данные получены/созданы через API и проверены в UI |

Техники тест-дизайна: классы эквивалентности (разделы, группы мышц, категории планов), негативные данные, случайные данные (Bogus, случайный элемент из API).

## UI-тесты (SpecFlow, 31 сценарий)

| Feature | Сценарий | Теги |
|---|---|---|
| Navigation | Главная открывается, заголовок «Track & Plan Workouts» | smoke, critical |
| Navigation | «Get Started» ведёт на регистрацию | smoke, critical |
| Navigation | 6 ссылок футера: Exercise Database, Workout Plans, Login, Sign Up, Blog, FAQ | regression |
| Navigation | 4 ссылки на соцсети | regression, minor |
| Exercise Database | База показывает > 1000 упражнений и ≥ 10 карточек | smoke, critical |
| Exercise Database | Фильтр Chest / Back / Biceps: счётчик уменьшается, все карточки нужной группы | regression |
| Exercise Database | Вторая страница: `page=2`, другие упражнения | regression |
| Exercise Database | Открытие упражнения из списка | smoke, critical |
| Exercise Database | На странице упражнения есть блоки Target Muscle Groups, Equipment, Difficulty, Instructions | regression |
| Workout Plans | > 1000 планов, ≥ 10 карточек | smoke, critical |
| Workout Plans | Категории Beginner, Home, Muscle Building | regression |
| Workout Plans | Открытие плана: заголовок совпадает с карточкой | regression |
| Login | Форма отображается | smoke, critical |
| Login | Случайные неверные данные — остаёмся на странице входа | regression, critical |
| Login | Пустая форма не отправляется | regression |
| Login | Успешный вход (тестовый аккаунт) | auth, blocker |
| API Prepared Data | Случайное упражнение из API отображается в UI | regression |
| API Prepared Data | Случайный план из API отображается в UI | regression |
| Account Data | План, созданный через API, виден в UI, затем удаляется | auth |

## API-тесты (NUnit, 28 тестов)

| Класс | Тест | Категории |
|---|---|---|
| PublicPagesTests | 8 страниц: 200, HTML, title, время ответа | smoke, critical |
| PublicPagesTests | Несуществующая страница → 404 | normal |
| PublicPagesTests | http:// → редирект на https:// | critical |
| PublicPagesTests | robots.txt доступен | minor |
| ExerciseDatabaseApiTests | Список ≥ 10 упражнений с названиями, canonical | smoke, critical |
| ExerciseDatabaseApiTests | SEO трёх упражнений | normal |
| ExerciseDatabaseApiTests | Страницы 1 и 2 различаются | normal |
| ExerciseDatabaseApiTests | 4 страницы групп мышц | normal |
| WorkoutPlansApiTests | Список ≥ 10 планов | smoke, critical |
| WorkoutPlansApiTests | 5 категорий планов | normal |
| AccountApiTests | Логин с верными данными | auth, critical |
| AccountApiTests | Логин со случайными данными отклоняется | auth |
