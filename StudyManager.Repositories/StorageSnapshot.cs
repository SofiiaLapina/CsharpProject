using StudyManager.Storage;

namespace StudyManager.Repositories;

internal sealed class StorageSnapshot
{
    public List<SubjectData> Subjects { get; set; } = new();
    public List<LessonData> Lessons { get; set; } = new();
}