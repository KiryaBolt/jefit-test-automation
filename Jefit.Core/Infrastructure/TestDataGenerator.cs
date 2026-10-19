using Bogus;

namespace Jefit.Core.Infrastructure;

public record UserCredentials(string Email, string Password);

/// <summary>Генерация уникальных тестовых данных (Bogus). Новый Faker на вызов — безопасно для параллельного запуска.</summary>
public static class TestDataGenerator
{
    public static UserCredentials GenerateInvalidUser()
    {
        var faker = new Faker("en");
        var email = faker.Internet.Email(faker.Name.FirstName(), faker.Name.LastName(), "qa-invalid.example");
        var password = faker.Internet.Password(14, memorable: false, prefix: "Qa1!");
        return new UserCredentials(email, password);
    }

    public static string GenerateWorkoutPlanName()
    {
        var faker = new Faker("en");
        return $"QA {faker.Hacker.Adjective()} plan {DateTime.UtcNow:yyMMddHHmmss}";
    }

    public static T PickRandom<T>(IReadOnlyList<T> items)
    {
        if (items.Count == 0) throw new ArgumentException("Список пуст — выбирать не из чего.", nameof(items));
        return items[Random.Shared.Next(items.Count)];
    }
}
