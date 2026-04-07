using StudyManager.Services.Dtos.Lessons;
using StudyManager.Storage;

namespace StudyManager.Services.Interfaces;

public interface ILessonService
{
    LessonDetailsDto GetLessonDetails(Guid lessonId);

    Task<LessonDetailsDto> GetLessonDetailsAsync(Guid lessonId, CancellationToken cancellationToken = default);
    Task<LessonDetailsDto> CreateLessonAsync(UpsertLessonDto lesson, CancellationToken cancellationToken = default);
    Task<LessonDetailsDto> UpdateLessonAsync(Guid lessonId, UpsertLessonDto lesson, CancellationToken cancellationToken = default);
    Task DeleteLessonAsync(Guid lessonId, CancellationToken cancellationToken = default);

    Task<LessonEditDto> GetLessonForEditAsync(Guid lessonId, CancellationToken cancellationToken = default);
    IReadOnlyList<LessonType> GetLessonTypes();
}