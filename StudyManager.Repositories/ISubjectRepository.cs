using StudyManager.Storage;

namespace StudyManager.Repositories;

public interface ISubjectRepository
{
    IReadOnlyList<SubjectData> GetAll();
    SubjectData? GetById(Guid id);
}