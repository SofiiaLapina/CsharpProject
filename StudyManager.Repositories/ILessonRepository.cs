using StudyManager.Storage;

namespace StudyManager.Repositories;

public interface ILessonRepository
{
    IReadOnlyList<LessonData> GetBySubjectId(Guid subjectId);
    LessonData? GetById(Guid id);

    Task<IReadOnlyList<LessonData>> GetBySubjectIdAsync(Guid subjectId, CancellationToken cancellationToken = default);
    Task<LessonData?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<LessonData> AddAsync(LessonData lesson, CancellationToken cancellationToken = default);
    Task UpdateAsync(LessonData lesson, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteBySubjectIdAsync(Guid subjectId, CancellationToken cancellationToken = default);
}
