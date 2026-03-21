using System;
using System.Windows.Controls;

namespace StudyManager.WpfApp.Infrastructure;

public sealed class NavigationService : INavigationService
{
    private readonly Frame _frame;

    public NavigationService(Frame frame)
    {
        _frame = frame;
    }

    public void NavigateToSubjectDetails(Guid subjectId)
    {
        // потом глянешь
        throw new NotImplementedException("Wire SubjectDetailsPage navigation later");
    }

    public void NavigateToLessonDetails(Guid lessonId)
    {
        // не забудь
        throw new NotImplementedException("Wire LessonDetailsPage navigation later");
    }

    public void GoBack()
    {
        if (_frame.CanGoBack)
            _frame.GoBack();
    }
}
