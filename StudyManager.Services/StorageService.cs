using StudyManager.Storage;

namespace StudyManager.Services;

public sealed class StorageService : IStorageService
{
    public IReadOnlyList<SubjectData> GetSubjects() => FakeStorage.Subjects;

    public IReadOnlyList<LessonData> GetLessonsBySubjectId(Guid subjectId) =>
        FakeStorage.Lessons.Where(l => l.SubjectId == subjectId).ToList();
}