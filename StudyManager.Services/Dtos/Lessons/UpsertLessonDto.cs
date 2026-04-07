using StudyManager.Storage;

namespace StudyManager.Services.Dtos.Lessons;

public sealed class UpsertLessonDto
{
    public Guid SubjectId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string Topic { get; set; } = string.Empty;
    public LessonType Type { get; set; }
}
