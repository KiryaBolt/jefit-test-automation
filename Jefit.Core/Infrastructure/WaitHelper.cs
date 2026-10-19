using System.Diagnostics;

namespace Jefit.Core.Infrastructure;

public static class WaitHelper
{
    /// <summary>
    /// Проверяет, что условие остаётся верным весь период
    /// (например, «после отправки формы пользователь всё ещё на странице логина»).
    /// </summary>
    public static bool ConditionHoldsFor(Func<bool> condition, TimeSpan duration)
    {
        var stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed < duration)
        {
            if (!condition()) return false;
            Thread.Sleep(250);
        }

        return condition();
    }
}
