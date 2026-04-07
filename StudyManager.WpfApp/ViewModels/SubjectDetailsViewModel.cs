using StudyManager.Services.Dtos.Lessons;
using StudyManager.Services.Dtos.Subjects;
using StudyManager.Services.Interfaces;
using StudyManager.WpfApp.Infrastructure;
using System.Collections.ObjectModel;

namespace StudyManager.WpfApp.ViewModels;

public sealed class SubjectDetailsViewModel : ViewModelBase
{
    private readonly Guid _subjectId;
    private readonly ISubjectService _subjectService;
    private readonly INavigationService _navigation;

    private SubjectDetailsDto? _subject;
    private LessonListItemDto? _selectedLesson;
    private bool _isBusy;

    public SubjectDetailsDto? Subject
    {
        get => _subject;
        private set
        {
            _subject = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(EctsCreditsText));
            OnPropertyChanged(nameof(AreaText));
            OnPropertyChanged(nameof(TotalDurationText));
        }
    }

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

    public ObservableCollection<LessonListItemDto> Lessons { get; } = new();

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            _isBusy = value;
            OnPropertyChanged();
            OpenLessonCommand.RaiseCanExecuteChanged();
            LoadCommand.RaiseCanExecuteChanged();
        }
    }

    public string Title => Subject?.Name ?? string.Empty;
    public string EctsCreditsText => Subject is null ? string.Empty : $"ECTS: {Subject.EctsCredits}";
    public string AreaText => Subject is null ? string.Empty : $"Area: {Subject.Area}";
    public string TotalDurationText => Subject is null ? string.Empty : $"Total: {Subject.TotalDuration:hh\\:mm\\:ss}";

    public RelayCommand OpenLessonCommand { get; }
    public RelayCommand GoBackCommand { get; }
    public AsyncRelayCommand LoadCommand { get; }

    public SubjectDetailsViewModel(
        Guid subjectId,
        ISubjectService subjectService,
        INavigationService navigation)
    {
        _subjectId = subjectId;
        _subjectService = subjectService;
        _navigation = navigation;

        OpenLessonCommand = new RelayCommand(
            execute: () => _navigation.NavigateToLessonDetails(SelectedLesson!.Id),
            canExecute: () => SelectedLesson is not null && !IsBusy);

        GoBackCommand = new RelayCommand(
            execute: () => _navigation.GoBack(),
            canExecute: () => !IsBusy);

        LoadCommand = new AsyncRelayCommand(
            execute: LoadAsync,
            canExecute: () => !IsBusy);
    }

    public async Task LoadAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;

            var subject = await _subjectService.GetSubjectDetailsAsync(_subjectId);
            Subject = subject;

            Lessons.Clear();
            foreach (var lesson in subject.Lessons)
            {
                Lessons.Add(lesson);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }
}
