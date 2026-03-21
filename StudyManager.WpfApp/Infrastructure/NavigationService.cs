using System;
using System.Windows.Controls;

namespace StudyManager.WpfApp.Infrastructure;

public sealed class NavigationService : INavigationService
{
    private readonly Frame _frame;
    private readonly Func<Guid, Page>? _subjectDetailsPageFactory;
    private readonly Func<Guid, Page>? _lessonDetailsPageFactory;

    public NavigationService(
        Frame frame,
        Func<Guid, Page>? subjectDetailsPageFactory = null,
        Func<Guid, Page>? lessonDetailsPageFactory = null)
    {
        _frame = frame;
        _subjectDetailsPageFactory = subjectDetailsPageFactory;
        _lessonDetailsPageFactory = lessonDetailsPageFactory;
    }

    public void NavigateToSubjectDetails(Guid subjectId)
    {
        if (_subjectDetailsPageFactory is null)
            throw new InvalidOperationException("SubjectDetailsPage factory is not configured.");

        _frame.Navigate(_subjectDetailsPageFactory(subjectId));
    }

    public void NavigateToLessonDetails(Guid lessonId)
    {
        if (_lessonDetailsPageFactory is null)
            throw new InvalidOperationException("LessonDetailsPage factory is not configured.");

        _frame.Navigate(_lessonDetailsPageFactory(lessonId));
    }

    public void GoBack()
    {
        if (_frame.CanGoBack)
            _frame.GoBack();
    }
}
