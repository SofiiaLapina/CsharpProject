using StudyManager.Services.Dtos.Lessons;

namespace StudyManager.Services.Interfaces;

public interface ILessonService
{
    LessonDetailsDto GetLessonDetails(Guid lessonId);

    Task<LessonDetailsDto> GetLessonDetailsAsync(Guid lessonId, CancellationToken cancellationToken = default);
    Task<LessonDetailsDto> CreateLessonAsync(UpsertLessonDto lesson, CancellationToken cancellationToken = default);
    Task<LessonDetailsDto> UpdateLessonAsync(Guid lessonId, UpsertLessonDto lesson, CancellationToken cancellationToken = default);
    Task DeleteLessonAsync(Guid lessonId, CancellationToken cancellationToken = default);
}