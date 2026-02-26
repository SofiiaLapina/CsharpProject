using StudyManager.Storage;
using System;
using System.Collections.Generic;

namespace StudyManager.Services;

public interface IStorageService
{
    IReadOnlyList<SubjectData> GetSubjects();
    IReadOnlyList<LessonData> GetLessonsBySubjectId(Guid subjectId);
}