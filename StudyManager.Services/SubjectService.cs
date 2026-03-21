using StudyManager.Repositories;
using StudyManager.Services.Dtos.Lessons;
using StudyManager.Services.Dtos.Subjects;
using StudyManager.Services.Interfaces;

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

    public IReadOnlyList<SubjectListItemDto> GetSubjects() =>
        _subjects.GetAll()
            .Select(s => new SubjectListItemDto
            {
                Id = s.Id,
                Name = s.Name,
                EctsCredits = s.EctsCredits,
                Area = s.Area
            })
            .ToList();

    public SubjectDetailsDto GetSubjectDetails(Guid subjectId)
    {
        var subject = _subjects.GetById(subjectId)
            ?? throw new InvalidOperationException($"Subject not found: {subjectId}");

        var lessons = _lessons.GetBySubjectId(subjectId)
            .Select(l => new LessonListItemDto
            {
                Id = l.Id,
                Date = l.Date,
                StartTime = l.StartTime,
                EndTime = l.EndTime,
                Topic = l.Topic,
                Type = l.Type,
                Duration = CalcDuration(l.StartTime, l.EndTime)
            })
            .ToList();

        var total = lessons.Aggregate(TimeSpan.Zero, (acc, x) => acc + x.Duration);

        return new SubjectDetailsDto
        {
            Id = subject.Id,
            Name = subject.Name,
            EctsCredits = subject.EctsCredits,
            Area = subject.Area,
            TotalDuration = total,
            Lessons = lessons
        };
    }

    private static TimeSpan CalcDuration(TimeOnly start, TimeOnly end)
    {
        var s = start.ToTimeSpan();
        var e = end.ToTimeSpan();
        return e > s ? e - s : TimeSpan.Zero;
    }
}