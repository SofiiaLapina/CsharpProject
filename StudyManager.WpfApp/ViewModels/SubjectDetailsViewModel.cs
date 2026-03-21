using StudyManager.Services.Dtos.Lessons;
using StudyManager.Services.Dtos.Subjects;
using StudyManager.Services.Interfaces;
using StudyManager.WpfApp.Infrastructure;

namespace StudyManager.WpfApp.ViewModels;

public sealed class SubjectDetailsViewModel : ViewModelBase
{
    private readonly ISubjectService _subjectService;
    private readonly INavigationService _navigation;

    public SubjectDetailsDto Subject { get; }

    private LessonListItemDto? _selectedLesson;
    public LessonListItemDto? SelectedLesson
    {
        get => _selectedLesson;
        set
        {
            _selectedLesson = value;
            OnPropertyChanged();
            OpenLessonCommand.RaiseCanExecuteChanged();
        }
    }

    public RelayCommand OpenLessonCommand { get; }
    public RelayCommand BackCommand { get; }

    public SubjectDetailsViewModel(
        Guid subjectId,
        ISubjectService subjectService,
        INavigationService navigation)
    {
        _subjectService = subjectService;
        _navigation = navigation;

        Subject = _subjectService.GetSubjectDetails(subjectId);

        OpenLessonCommand = new RelayCommand(
            execute: () => _navigation.NavigateToLessonDetails(SelectedLesson!.Id),
            canExecute: () => SelectedLesson is not null
        );

        BackCommand = new RelayCommand(() => _navigation.GoBack());
    }
}
