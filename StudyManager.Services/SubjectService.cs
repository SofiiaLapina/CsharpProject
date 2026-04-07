using StudyManager.Repositories;
using StudyManager.Services.Dtos.Lessons;
using StudyManager.Services.Dtos.Subjects;
using StudyManager.Services.Interfaces;
using StudyManager.Storage;

namespace StudyManager.Services;

public sealed class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _subjects;
    private readonly ILessonRepository _lessons;

    public SubjectService(ISubjectRepository subjects, ILessonRepository lessons)
    {
        _subjects = subjects;
        _lessons = lessons;
    }

    public IReadOnlyList<SubjectListItemDto> GetSubjects()
    {
        return _subjects.GetAll()
            .Select(MapSubjectListItem)
            .ToList();
    }

    public SubjectDetailsDto GetSubjectDetails(Guid subjectId)
    {
        var subject = _subjects.GetById(subjectId)
            ?? throw new InvalidOperationException($"Subject not found: {subjectId}");

        var lessons = _lessons.GetBySubjectId(subjectId)
            .Select(MapLessonListItem)
            .ToList();

        return BuildSubjectDetails(subject, lessons);
    }

    public async Task<IReadOnlyList<SubjectListItemDto>> GetSubjectsAsync(CancellationToken cancellationToken = default)
    {
        var subjects = await _subjects.GetAllAsync(cancellationToken);

        return subjects
            .Select(MapSubjectListItem)
            .ToList();
    }

    public async Task<SubjectDetailsDto> GetSubjectDetailsAsync(Guid subjectId, CancellationToken cancellationToken = default)
    {
        var subject = await _subjects.GetByIdAsync(subjectId, cancellationToken)
            ?? throw new InvalidOperationException($"Subject not found: {subjectId}");

        var lessons = await _lessons.GetBySubjectIdAsync(subjectId, cancellationToken);

        var lessonDtos = lessons
            .Select(MapLessonListItem)
            .ToList();

        return BuildSubjectDetails(subject, lessonDtos);
    }

    public async Task<SubjectDetailsDto> CreateSubjectAsync(UpsertSubjectDto subject, CancellationToken cancellationToken = default)
    {
        ValidateSubject(subject);

        var data = new SubjectData(
            Guid.NewGuid(),
            subject.Name.Trim(),
            subject.EctsCredits,
            subject.Area);

        await _subjects.AddAsync(data, cancellationToken);
        return await GetSubjectDetailsAsync(data.Id, cancellationToken);
    }

    public async Task<SubjectDetailsDto> UpdateSubjectAsync(Guid subjectId, UpsertSubjectDto subject, CancellationToken cancellationToken = default)
    {
        ValidateSubject(subject);

        var existing = await _subjects.GetByIdAsync(subjectId, cancellationToken)
            ?? throw new InvalidOperationException($"Subject not found: {subjectId}");

        existing.Update(subject.Name.Trim(), subject.EctsCredits, subject.Area);

        await _subjects.UpdateAsync(existing, cancellationToken);
        return await GetSubjectDetailsAsync(subjectId, cancellationToken);
    }

    public async Task DeleteSubjectAsync(Guid subjectId, CancellationToken cancellationToken = default)
    {
        var existing = await _subjects.GetByIdAsync(subjectId, cancellationToken)
            ?? throw new InvalidOperationException($"Subject not found: {subjectId}");

        await _lessons.DeleteBySubjectIdAsync(existing.Id, cancellationToken);
        await _subjects.DeleteAsync(existing.Id, cancellationToken);
    }

    private static SubjectListItemDto MapSubjectListItem(SubjectData subject)
    {
        return new SubjectListItemDto
        {
            Id = subject.Id,
            Name = subject.Name,
            EctsCredits = subject.EctsCredits,
            Area = subject.Area
        };
    }

    private static LessonListItemDto MapLessonListItem(LessonData lesson)
    {
        return new LessonListItemDto
        {
            Id = lesson.Id,
            Date = lesson.Date,
            StartTime = lesson.StartTime,
            EndTime = lesson.EndTime,
            Topic = lesson.Topic,
            Type = lesson.Type,
            Duration = CalcDuration(lesson.StartTime, lesson.EndTime)
        };
    }

    private static SubjectDetailsDto BuildSubjectDetails(SubjectData subject, IReadOnlyList<LessonListItemDto> lessons)
    {
        var totalDuration = lessons.Aggregate(TimeSpan.Zero, (acc, lesson) => acc + lesson.Duration);

        return new SubjectDetailsDto
        {
            Id = subject.Id,
            Name = subject.Name,
            EctsCredits = subject.EctsCredits,
            Area = subject.Area,
            TotalDuration = totalDuration,
            Lessons = lessons
        };
    }

    private static TimeSpan CalcDuration(TimeOnly start, TimeOnly end)
    {
        var startSpan = start.ToTimeSpan();
        var endSpan = end.ToTimeSpan();

        return endSpan > startSpan ? endSpan - startSpan : TimeSpan.Zero;
    }

    private static void ValidateSubject(UpsertSubjectDto subject)
    {
        if (string.IsNullOrWhiteSpace(subject.Name))
        {
            throw new ArgumentException("Subject name cannot be empty.");
        }

        if (subject.EctsCredits <= 0)
        {
            throw new ArgumentException("ECTS credits must be greater than zero.");
        }
    }
}