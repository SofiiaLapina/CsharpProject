using StudyManager.Storage;

namespace StudyManager.Repositories;

internal static class SeedDataFactory
{
    internal static StorageSnapshot Create()
    {
        var snapshot = new StorageSnapshot();

        var subject1 = new SubjectData(Guid.NewGuid(), "C# / .NET Basics", 6, KnowledgeArea.Programming);
        var subject2 = new SubjectData(Guid.NewGuid(), "Discrete Mathematics", 5, KnowledgeArea.Mathematics);
        var subject3 = new SubjectData(Guid.NewGuid(), "Software Engineering", 4, KnowledgeArea.Engineering);

        snapshot.Subjects.AddRange(new[]
        {
            subject1,
            subject2,
            subject3
        });

        snapshot.Lessons.AddRange(new[]
        {
            new LessonData(Guid.NewGuid(), subject1.Id, new DateOnly(2026, 2, 3),  new TimeOnly(9, 0),  new TimeOnly(10, 20), "C# syntax, types", LessonType.Lecture),
            new LessonData(Guid.NewGuid(), subject1.Id, new DateOnly(2026, 2, 5),  new TimeOnly(9, 0),  new TimeOnly(10, 20), "OOP basics", LessonType.Lecture),
            new LessonData(Guid.NewGuid(), subject1.Id, new DateOnly(2026, 2, 6),  new TimeOnly(11, 0), new TimeOnly(12, 20), "Properties and validation", LessonType.Lab),
            new LessonData(Guid.NewGuid(), subject1.Id, new DateOnly(2026, 2, 10), new TimeOnly(9, 0),  new TimeOnly(10, 20), "Collections", LessonType.Lecture),
            new LessonData(Guid.NewGuid(), subject1.Id, new DateOnly(2026, 2, 12), new TimeOnly(9, 0),  new TimeOnly(10, 20), "Exceptions", LessonType.Seminar),
            new LessonData(Guid.NewGuid(), subject1.Id, new DateOnly(2026, 2, 13), new TimeOnly(11, 0), new TimeOnly(12, 20), "Console I/O", LessonType.Lab),
            new LessonData(Guid.NewGuid(), subject1.Id, new DateOnly(2026, 2, 17), new TimeOnly(9, 0),  new TimeOnly(10, 20), "Interfaces", LessonType.Lecture),
            new LessonData(Guid.NewGuid(), subject1.Id, new DateOnly(2026, 2, 19), new TimeOnly(9, 0),  new TimeOnly(10, 20), "Layered architecture", LessonType.Seminar),
            new LessonData(Guid.NewGuid(), subject1.Id, new DateOnly(2026, 2, 20), new TimeOnly(11, 0), new TimeOnly(12, 20), "Mapping data", LessonType.Lab),
            new LessonData(Guid.NewGuid(), subject1.Id, new DateOnly(2026, 2, 24), new TimeOnly(9, 0),  new TimeOnly(10, 20), "Review", LessonType.Seminar),

            new LessonData(Guid.NewGuid(), subject2.Id, new DateOnly(2026, 2, 4),  new TimeOnly(13, 0), new TimeOnly(14, 20), "Sets and relations", LessonType.Lecture),
            new LessonData(Guid.NewGuid(), subject2.Id, new DateOnly(2026, 2, 11), new TimeOnly(13, 0), new TimeOnly(14, 20), "Graphs basics", LessonType.Seminar)
        });

        return snapshot;
    }
}