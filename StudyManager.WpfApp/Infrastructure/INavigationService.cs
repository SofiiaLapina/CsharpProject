using System;
using System.Collections.Generic;
using System.Text;

namespace StudyManager.WpfApp.Infrastructure;

public interface INavigationService
{
    void NavigateToSubjectDetails(Guid subjectId);
    void NavigateToLessonDetails(Guid lessonId);
    void GoBack();
}
