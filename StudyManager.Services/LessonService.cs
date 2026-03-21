using StudyManager.Repositories;
using StudyManager.Services.Dtos.Lessons;
using StudyManager.Services.Interfaces;

namespace StudyManager.Services;

public sealed class LessonService : ILessonService
{
    private readonly ILessonRepository _lessons;

    public LessonService(ILessonRepository lessons)
    {
        _lessons = lessons;
    }

    public LessonDetailsDto GetLessonDetails(Guid lessonId)
    {
        var lesson = _lessons.GetById(lessonId)
            ?? throw new InvalidOperationException($"Lesson not found: {lessonId}");

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
        var s = start.ToTimeSpan();
        var e = end.ToTimeSpan();
        return e > s ? e - s : TimeSpan.Zero;
    }
}