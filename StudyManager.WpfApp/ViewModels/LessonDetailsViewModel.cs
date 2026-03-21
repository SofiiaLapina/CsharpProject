using System;
using StudyManager.Services.Dtos.Lessons;
using StudyManager.Services.Interfaces;
using StudyManager.WpfApp.Infrastructure;

namespace StudyManager.WpfApp.ViewModels;

public sealed class LessonDetailsViewModel : ViewModelBase
{
    private readonly ILessonService _lessonService;
    private readonly INavigationService _navigation;

    public LessonDetailsDto Lesson { get; }

    public RelayCommand BackCommand { get; }

    public LessonDetailsViewModel(
        Guid lessonId,
        ILessonService lessonService,
        INavigationService navigation)
    {
        _lessonService = lessonService;
        _navigation = navigation;

        Lesson = _lessonService.GetLessonDetails(lessonId);
        BackCommand = new RelayCommand(() => _navigation.GoBack());
    }
}
