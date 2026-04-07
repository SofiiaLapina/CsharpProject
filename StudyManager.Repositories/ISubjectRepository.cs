using StudyManager.Storage;

namespace StudyManager.Repositories;

public interface ISubjectRepository
{
    IReadOnlyList<SubjectData> GetAll();
    SubjectData? GetById(Guid id);

    Task<IReadOnlyList<SubjectData>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SubjectData?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SubjectData> AddAsync(SubjectData subject, CancellationToken cancellationToken = default);
    Task UpdateAsync(SubjectData subject, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
