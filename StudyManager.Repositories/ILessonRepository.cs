using StudyManager.Storage;

namespace StudyManager.Repositories;

public interface ILessonRepository
{
    IReadOnlyList<LessonData> GetBySubjectId(Guid subjectId);
    LessonData? GetById(Guid id);
}