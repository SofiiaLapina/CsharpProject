using StudyManager.Storage;

namespace StudyManager.Presentation;

public sealed class LessonViewModel
{
    private readonly LessonData _data;

    public Guid Id => _data.Id;
    public Guid SubjectId => _data.SubjectId;
    public DateOnly Date => _data.Date;
    public TimeOnly StartTime => _data.StartTime;
    public TimeOnly EndTime => _data.EndTime;
    public string Topic => _data.Topic;
    public LessonType Type => _data.Type;

    public TimeSpan Duration
    {
        get
        {
            var start = _data.StartTime.ToTimeSpan();
            var end = _data.EndTime.ToTimeSpan();
            return end > start ? end - start : TimeSpan.Zero;
        }
    }

    public LessonViewModel(LessonData data) => _data = data;
}
