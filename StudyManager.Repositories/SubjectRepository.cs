using StudyManager.Storage;

namespace StudyManager.Repositories;

public sealed class SubjectRepository : ISubjectRepository
{
    public IReadOnlyList<SubjectData> GetAll() => FakeStorage.Subjects;

    public SubjectData? GetById(Guid id) =>
        FakeStorage.Subjects.FirstOrDefault(s => s.Id == id);
}