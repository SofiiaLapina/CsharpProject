using StudyManager.Repositories;
using StudyManager.Services.Dtos.Lessons;
using StudyManager.Services.Interfaces;
using StudyManager.Storage;

namespace StudyManager.Services;

public sealed class LessonService : ILessonService
{
    private readonly ILessonRepository _lessons;
    private readonly ISubjectRepository _subjects;

    public LessonService(ILessonRepository lessons, ISubjectRepository subjects)
    {
        _lessons = lessons;
        _subjects = subjects;
    }

    public LessonDetailsDto GetLessonDetails(Guid lessonId)
    {
        var lesson = _lessons.GetById(lessonId)
            ?? throw new InvalidOperationException($"Lesson not found: {lessonId}");

        return MapLessonDetails(lesson);
    }

    public async Task<LessonDetailsDto> GetLessonDetailsAsync(Guid lessonId, CancellationToken cancellationToken = default)
    {
        var lesson = await _lessons.GetByIdAsync(lessonId, cancellationToken)
            ?? throw new InvalidOperationException($"Lesson not found: {lessonId}");

        return MapLessonDetails(lesson);
    }

    public async Task<LessonDetailsDto> CreateLessonAsync(UpsertLessonDto lesson, CancellationToken cancellationToken = default)
    {
        ValidateLesson(lesson);

        var subject = await _subjects.GetByIdAsync(lesson.SubjectId, cancellationToken);
        if (subject is null)
        {
            throw new InvalidOperationException($"Subject not found: {lesson.SubjectId}");
        }

        var data = new LessonData(
            Guid.NewGuid(),
            lesson.SubjectId,
            lesson.Date,
            lesson.StartTime,
            lesson.EndTime,
            lesson.Topic.Trim(),
            lesson.Type);

        await _lessons.AddAsync(data, cancellationToken);
        return await GetLessonDetailsAsync(data.Id, cancellationToken);
    }

    public async Task<LessonDetailsDto> UpdateLessonAsync(Guid lessonId, UpsertLessonDto lesson, CancellationToken cancellationToken = default)
    {
        ValidateLesson(lesson);

        var existing = await _lessons.GetByIdAsync(lessonId, cancellationToken)
            ?? throw new InvalidOperationException($"Lesson not found: {lessonId}");

        if (lesson.SubjectId != Guid.Empty && lesson.SubjectId != existing.SubjectId)
        {
            throw new InvalidOperationException("Changing lesson subject is not supported.");
        }

        existing.Update(
            lesson.Date,
            lesson.StartTime,
            lesson.EndTime,
            lesson.Topic.Trim(),
            lesson.Type);

        await _lessons.UpdateAsync(existing, cancellationToken);
        return await GetLessonDetailsAsync(existing.Id, cancellationToken);
    }

    public async Task DeleteLessonAsync(Guid lessonId, CancellationToken cancellationToken = default)
    {
        var existing = await _lessons.GetByIdAsync(lessonId, cancellationToken)
            ?? throw new InvalidOperationException($"Lesson not found: {lessonId}");

        await _lessons.DeleteAsync(existing.Id, cancellationToken);
    }

    public async Task<LessonEditDto> GetLessonForEditAsync(Guid lessonId, CancellationToken cancellationToken = default)
    {
        var lesson = await _lessons.GetByIdAsync(lessonId, cancellationToken)
            ?? throw new InvalidOperationException($"Lesson not found: {lessonId}");

        return new LessonEditDto
        {
            Id = lesson.Id,
            SubjectId = lesson.SubjectId,
            Date = lesson.Date,
            StartTime = lesson.StartTime,
            EndTime = lesson.EndTime,
            Topic = lesson.Topic,
            Type = lesson.Type
        };
    }

    public IReadOnlyList<LessonType> GetLessonTypes()
    {
        return Enum.GetValues<LessonType>();
    }

    private static LessonDetailsDto MapLessonDetails(LessonData lesson)
    {
        return new LessonDetailsDto
        {
            Id = lesson.Id,
            SubjectId = lesson.SubjectId,
            Date = lesson.Date,
            StartTime = lesson.StartTime,
            EndTime = lesson.EndTime,
            Topic = lesson.Topic,
            Type = lesson.Type,
            Duration = CalcDuration(lesson.StartTime, lesson.EndTime)
        };
    }

    private static TimeSpan CalcDuration(TimeOnly start, TimeOnly end)
    {
        var startSpan = start.ToTimeSpan();
        var endSpan = end.ToTimeSpan();

        return endSpan > startSpan ? endSpan - startSpan : TimeSpan.Zero;
    }

    private static void ValidateLesson(UpsertLessonDto lesson)
    {
        if (lesson.SubjectId == Guid.Empty)
        {
            throw new ArgumentException("SubjectId is required.");
        }

        if (string.IsNullOrWhiteSpace(lesson.Topic))
        {
            throw new ArgumentException("Lesson topic cannot be empty.");
        }

        if (lesson.EndTime <= lesson.StartTime)
        {
            throw new ArgumentException("Lesson end time must be greater than start time.");
        }
    }
}