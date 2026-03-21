using StudyManager.Storage;

namespace StudyManager.Repositories;

public sealed class LessonRepository : ILessonRepository
{
    public IReadOnlyList<LessonData> GetBySubjectId(Guid subjectId) =>
        FakeStorage.Lessons.Where(l => l.SubjectId == subjectId).ToList();

    public LessonData? GetById(Guid id) =>
        FakeStorage.Lessons.FirstOrDefault(l => l.Id == id);
}