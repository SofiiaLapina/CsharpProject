using StudyManager.Services.Dtos.Subjects;
using StudyManager.Storage;

namespace StudyManager.Services.Interfaces;

public interface ISubjectService
{
    IReadOnlyList<SubjectListItemDto> GetSubjects();
    SubjectDetailsDto GetSubjectDetails(Guid subjectId);

    Task<IReadOnlyList<SubjectListItemDto>> GetSubjectsAsync(CancellationToken cancellationToken = default);
    Task<SubjectDetailsDto> GetSubjectDetailsAsync(Guid subjectId, CancellationToken cancellationToken = default);

    Task<SubjectDetailsDto> CreateSubjectAsync(UpsertSubjectDto subject, CancellationToken cancellationToken = default);
    Task<SubjectDetailsDto> UpdateSubjectAsync(Guid subjectId, UpsertSubjectDto subject, CancellationToken cancellationToken = default);
    Task DeleteSubjectAsync(Guid subjectId, CancellationToken cancellationToken = default);

    Task<SubjectEditDto> GetSubjectForEditAsync(Guid subjectId, CancellationToken cancellationToken = default);
    IReadOnlyList<KnowledgeArea> GetKnowledgeAreas();
}