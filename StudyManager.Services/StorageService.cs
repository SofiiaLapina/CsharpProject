using StudyManager.Repositories;
using StudyManager.Storage;

namespace StudyManager.Services;

public sealed class StorageService : IStorageService
{
    private readonly ISubjectRepository _subjects;
    private readonly ILessonRepository _lessons;

    public StorageService(ISubjectRepository subjects, ILessonRepository lessons)
    {
        _subjects = subjects;
        _lessons = lessons;
    }

    public IReadOnlyList<SubjectData> GetSubjects() => _subjects.GetAll();

    public IReadOnlyList<LessonData> GetLessonsBySubjectId(Guid subjectId) =>
        _lessons.GetBySubjectId(subjectId);
}