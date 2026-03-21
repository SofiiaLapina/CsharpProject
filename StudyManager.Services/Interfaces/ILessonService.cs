using StudyManager.Services.Dtos.Lessons;

namespace StudyManager.Services.Interfaces;

public interface ILessonService
{
    LessonDetailsDto GetLessonDetails(Guid lessonId);
}