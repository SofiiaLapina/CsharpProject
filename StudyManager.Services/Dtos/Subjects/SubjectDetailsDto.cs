using StudyManager.Storage;
using StudyManager.Services.Dtos.Lessons;

namespace StudyManager.Services.Dtos.Subjects;

public sealed class SubjectDetailsDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int EctsCredits { get; init; }
    public KnowledgeArea Area { get; init; }

    public TimeSpan TotalDuration { get; init; }
    public IReadOnlyList<LessonListItemDto> Lessons { get; init; } = Array.Empty<LessonListItemDto>();
}