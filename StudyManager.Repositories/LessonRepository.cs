using StudyManager.Storage;

namespace StudyManager.Repositories;

public sealed class LessonRepository : ILessonRepository
{
    private static readonly JsonStorage Storage = new();

    public IReadOnlyList<LessonData> GetBySubjectId(Guid subjectId)
    {
        return GetBySubjectIdAsync(subjectId).GetAwaiter().GetResult();
    }

    public LessonData? GetById(Guid id)
    {
        return GetByIdAsync(id).GetAwaiter().GetResult();
    }

    public async Task<IReadOnlyList<LessonData>> GetBySubjectIdAsync(Guid subjectId, CancellationToken cancellationToken = default)
    {
        var snapshot = await LoadSnapshotAsync(cancellationToken).ConfigureAwait(false);

        return snapshot.Lessons
            .Where(l => l.SubjectId == subjectId)
            .ToList();
    }

    public async Task<LessonData?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var snapshot = await LoadSnapshotAsync(cancellationToken).ConfigureAwait(false);
        return snapshot.Lessons.FirstOrDefault(l => l.Id == id);
    }

    public async Task<LessonData> AddAsync(LessonData lesson, CancellationToken cancellationToken = default)
    {
        var snapshot = await LoadSnapshotAsync(cancellationToken).ConfigureAwait(false);

        if (snapshot.Lessons.Any(l => l.Id == lesson.Id))
        {
            throw new InvalidOperationException($"Lesson with id '{lesson.Id}' already exists.");
        }

        snapshot.Lessons.Add(lesson);
        await Storage.SaveAsync(snapshot, cancellationToken).ConfigureAwait(false);

        return lesson;
    }

    public async Task UpdateAsync(LessonData lesson, CancellationToken cancellationToken = default)
    {
        var snapshot = await LoadSnapshotAsync(cancellationToken).ConfigureAwait(false);

        var existing = snapshot.Lessons.FirstOrDefault(l => l.Id == lesson.Id)
            ?? throw new InvalidOperationException($"Lesson with id '{lesson.Id}' was not found.");

        existing.Update(lesson.Date, lesson.StartTime, lesson.EndTime, lesson.Topic, lesson.Type);
        await Storage.SaveAsync(snapshot, cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var snapshot = await LoadSnapshotAsync(cancellationToken).ConfigureAwait(false);

        var existing = snapshot.Lessons.FirstOrDefault(l => l.Id == id)
            ?? throw new InvalidOperationException($"Lesson with id '{id}' was not found.");

        snapshot.Lessons.Remove(existing);
        await Storage.SaveAsync(snapshot, cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteBySubjectIdAsync(Guid subjectId, CancellationToken cancellationToken = default)
    {
        var snapshot = await LoadSnapshotAsync(cancellationToken).ConfigureAwait(false);

        snapshot.Lessons.RemoveAll(l => l.SubjectId == subjectId);
        await Storage.SaveAsync(snapshot, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<StorageSnapshot> LoadSnapshotAsync(CancellationToken cancellationToken)
    {
        await Storage.EnsureCreatedWithSeedAsync(SeedDataFactory.Create, cancellationToken).ConfigureAwait(false);
        return await Storage.LoadAsync(cancellationToken).ConfigureAwait(false);
    }
}