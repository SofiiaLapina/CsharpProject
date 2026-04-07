using StudyManager.Storage;

namespace StudyManager.Repositories;

public sealed class SubjectRepository : ISubjectRepository
{
    private static readonly JsonStorage Storage = new();

    public IReadOnlyList<SubjectData> GetAll()
    {
        return GetAllAsync().GetAwaiter().GetResult();
    }

    public SubjectData? GetById(Guid id)
    {
        return GetByIdAsync(id).GetAwaiter().GetResult();
    }

    public async Task<IReadOnlyList<SubjectData>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var snapshot = await LoadSnapshotAsync(cancellationToken).ConfigureAwait(false);
        return snapshot.Subjects.ToList();
    }

    public async Task<SubjectData?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var snapshot = await LoadSnapshotAsync(cancellationToken).ConfigureAwait(false);
        return snapshot.Subjects.FirstOrDefault(s => s.Id == id);
    }

    public async Task<SubjectData> AddAsync(SubjectData subject, CancellationToken cancellationToken = default)
    {
        var snapshot = await LoadSnapshotAsync(cancellationToken).ConfigureAwait(false);

        if (snapshot.Subjects.Any(s => s.Id == subject.Id))
        {
            throw new InvalidOperationException($"Subject with id '{subject.Id}' already exists.");
        }

        snapshot.Subjects.Add(subject);
        await Storage.SaveAsync(snapshot, cancellationToken).ConfigureAwait(false);

        return subject;
    }

    public async Task UpdateAsync(SubjectData subject, CancellationToken cancellationToken = default)
    {
        var snapshot = await LoadSnapshotAsync(cancellationToken).ConfigureAwait(false);

        var existing = snapshot.Subjects.FirstOrDefault(s => s.Id == subject.Id)
            ?? throw new InvalidOperationException($"Subject with id '{subject.Id}' was not found.");

        existing.Update(subject.Name, subject.EctsCredits, subject.Area);
        await Storage.SaveAsync(snapshot, cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var snapshot = await LoadSnapshotAsync(cancellationToken).ConfigureAwait(false);

        var existing = snapshot.Subjects.FirstOrDefault(s => s.Id == id)
            ?? throw new InvalidOperationException($"Subject with id '{id}' was not found.");

        snapshot.Subjects.Remove(existing);
        await Storage.SaveAsync(snapshot, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<StorageSnapshot> LoadSnapshotAsync(CancellationToken cancellationToken)
    {
        await Storage.EnsureCreatedWithSeedAsync(SeedDataFactory.Create, cancellationToken).ConfigureAwait(false);
        return await Storage.LoadAsync(cancellationToken).ConfigureAwait(false);
    }
}