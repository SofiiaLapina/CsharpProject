namespace StudyManager.Storage;

public sealed class LessonData
{
    public Guid Id { get; }
    public Guid SubjectId { get; }

    public DateOnly Date { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public string Topic { get; private set; }
    public LessonType Type { get; private set; }

    public LessonData(
        Guid id,
        Guid subjectId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        string topic,
        LessonType type)
    {
        Id = id;
        SubjectId = subjectId;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        Topic = topic;
        Type = type;
    }

    public void Update(DateOnly date, TimeOnly startTime, TimeOnly endTime, string topic, LessonType type)
    {
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        Topic = topic;
        Type = type;
    }
}
