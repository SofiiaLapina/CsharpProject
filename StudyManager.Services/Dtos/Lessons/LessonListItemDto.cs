using StudyManager.Storage;

namespace StudyManager.Services.Dtos.Lessons;

public sealed class LessonListItemDto
{
    public Guid Id { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public string Topic { get; init; } = string.Empty;
    public LessonType Type { get; init; }

    public TimeSpan Duration { get; init; }
}