using StudyManager.Storage;

namespace StudyManager.Services.Dtos.Subjects;

public sealed class SubjectEditDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int EctsCredits { get; set; }
    public KnowledgeArea Area { get; set; }
}