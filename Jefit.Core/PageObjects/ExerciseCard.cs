using Jefit.Core.Api.Models;

namespace Jefit.Core.PageObjects;

/// <summary>Снимок карточки упражнения (данные, а не живой WebElement — не устаревает при перерисовке).</summary>
public record ExerciseCard(ExerciseInfo Info, string Text);
