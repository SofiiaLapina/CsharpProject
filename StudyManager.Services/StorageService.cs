using StudyManager.Storage;

namespace StudyManager.Services;

public sealed class StorageService
{
    public IReadOnlyList<SubjectData> GetSubjects() => FakeStorage.Subjects;
    public IReadOnlyList<LessonData> GetLessonsBySubjectId(Guid subjectId) =>
        FakeStorage.Lessons.Where(l => l.SubjectId == subjectId).ToList();
}