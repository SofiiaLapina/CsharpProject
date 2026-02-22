using StudyManager.Storage;

namespace StudyManager.Presentation;

public sealed class SubjectViewModel
{
    private readonly SubjectData _data;
    private readonly List<LessonViewModel> _lessons = new();

    public Guid Id => _data.Id;
    public string Name => _data.Name;
    public int EctsCredits => _data.EctsCredits;
    public KnowledgeArea Area => _data.Area;

    public IReadOnlyList<LessonViewModel> Lessons => _lessons;
    public bool LessonsLoaded { get; private set; }

    public TimeSpan TotalDuration => _lessons.Aggregate(TimeSpan.Zero, (acc, l) => acc + l.Duration);

    public SubjectViewModel(SubjectData data) => _data = data;

    public void LoadLessons(IEnumerable<LessonData> lessons)
    {
        if (LessonsLoaded) return;

        _lessons.AddRange(lessons.Select(l => new LessonViewModel(l)));
        LessonsLoaded = true;
    }
}
