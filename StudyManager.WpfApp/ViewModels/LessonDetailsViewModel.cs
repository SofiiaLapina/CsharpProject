using StudyManager.Services.Dtos.Lessons;
using StudyManager.Services.Interfaces;
using StudyManager.WpfApp.Infrastructure;

namespace StudyManager.WpfApp.ViewModels;

public sealed class LessonDetailsViewModel : ViewModelBase
{
    private readonly Guid _lessonId;
    private readonly ILessonService _lessonService;
    private readonly INavigationService _navigation;

    private LessonDetailsDto? _lesson;
    private bool _isBusy;

    public LessonDetailsDto? Lesson
    {
        get => _lesson;
        private set
        {
            _lesson = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(IdText));
            OnPropertyChanged(nameof(SubjectIdText));
            OnPropertyChanged(nameof(DateText));
            OnPropertyChanged(nameof(TimeText));
            OnPropertyChanged(nameof(TypeText));
            OnPropertyChanged(nameof(DurationText));
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            _isBusy = value;
            OnPropertyChanged();
            GoBackCommand.RaiseCanExecuteChanged();
            LoadCommand.RaiseCanExecuteChanged();
        }
    }

    public string Title => Lesson?.Topic ?? string.Empty;
    public string IdText => Lesson is null ? string.Empty : $"Id: {Lesson.Id}";
    public string SubjectIdText => Lesson is null ? string.Empty : $"SubjectId: {Lesson.SubjectId}";
    public string DateText => Lesson is null ? string.Empty : $"Date: {Lesson.Date}";
    public string TimeText => Lesson is null ? string.Empty : $"Time: {Lesson.StartTime} - {Lesson.EndTime}";
    public string TypeText => Lesson is null ? string.Empty : $"Type: {Lesson.Type}";
    public string DurationText => Lesson is null ? string.Empty : $"Duration: {Lesson.Duration:hh\\:mm\\:ss}";

    public RelayCommand GoBackCommand { get; }
    public AsyncRelayCommand LoadCommand { get; }

    public LessonDetailsViewModel(
        Guid lessonId,
        ILessonService lessonService,
        INavigationService navigation)
    {
        _lessonId = lessonId;
        _lessonService = lessonService;
        _navigation = navigation;

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
            Lesson = await _lessonService.GetLessonDetailsAsync(_lessonId);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
