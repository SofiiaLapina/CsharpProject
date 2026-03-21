using StudyManager.Storage;

namespace StudyManager.Services.Dtos.Subjects;

public sealed class SubjectListItemDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int EctsCredits { get; init; }
    public KnowledgeArea Area { get; init; }
}