using System.Collections.ObjectModel;
using System.Windows;
using StudyManager.Services.Dtos.Lessons;
using StudyManager.Services.Interfaces;
using StudyManager.Storage;
using StudyManager.WpfApp.Infrastructure;

namespace StudyManager.WpfApp.ViewModels;

public sealed class LessonDetailsViewModel : ViewModelBase
{
    private readonly Guid _lessonId;
    private readonly ILessonService _lessonService;
    private readonly INavigationService _navigation;

    private LessonDetailsDto? _lesson;
    private bool _isBusy;
    private bool _isEditMode;

    private string _editableTopic = string.Empty;
    private string _editableDate = string.Empty;
    private string _editableStartTime = string.Empty;
    private string _editableEndTime = string.Empty;
    private LessonType _selectedLessonType;

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

            EnableEditCommand.RaiseCanExecuteChanged();
            CancelEditCommand.RaiseCanExecuteChanged();
            SaveLessonCommand.RaiseCanExecuteChanged();
        }
    }

    public ObservableCollection<LessonType> LessonTypes { get; } = new();

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            _isBusy = value;
            OnPropertyChanged();
            GoBackCommand.RaiseCanExecuteChanged();
            LoadCommand.RaiseCanExecuteChanged();
            EnableEditCommand.RaiseCanExecuteChanged();
            CancelEditCommand.RaiseCanExecuteChanged();
            SaveLessonCommand.RaiseCanExecuteChanged();
        }
    }

    public bool IsEditMode
    {
        get => _isEditMode;
        private set
        {
            _isEditMode = value;
            OnPropertyChanged();
            EnableEditCommand.RaiseCanExecuteChanged();
            CancelEditCommand.RaiseCanExecuteChanged();
            SaveLessonCommand.RaiseCanExecuteChanged();
        }
    }

    public string EditableTopic
    {
        get => _editableTopic;
        set
        {
            _editableTopic = value;
            OnPropertyChanged();
            SaveLessonCommand.RaiseCanExecuteChanged();
        }
    }

    public string EditableDate
    {
        get => _editableDate;
        set
        {
            _editableDate = value;
            OnPropertyChanged();
            SaveLessonCommand.RaiseCanExecuteChanged();
        }
    }

    public string EditableStartTime
    {
        get => _editableStartTime;
        set
        {
            _editableStartTime = value;
            OnPropertyChanged();
            SaveLessonCommand.RaiseCanExecuteChanged();
        }
    }

    public string EditableEndTime
    {
        get => _editableEndTime;
        set
        {
            _editableEndTime = value;
            OnPropertyChanged();
            SaveLessonCommand.RaiseCanExecuteChanged();
        }
    }

    public LessonType SelectedLessonType
    {
        get => _selectedLessonType;
        set
        {
            _selectedLessonType = value;
            OnPropertyChanged();
            SaveLessonCommand.RaiseCanExecuteChanged();
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
    public RelayCommand EnableEditCommand { get; }
    public RelayCommand CancelEditCommand { get; }
    public AsyncRelayCommand SaveLessonCommand { get; }
    public AsyncRelayCommand LoadCommand { get; }

    public LessonDetailsViewModel(
        Guid lessonId,
        ILessonService lessonService,
        INavigationService navigation)
    {
        _lessonId = lessonId;
        _lessonService = lessonService;
        _navigation = navigation;

        foreach (var lessonType in _lessonService.GetLessonTypes())
        {
            LessonTypes.Add(lessonType);
        }

        if (LessonTypes.Count > 0)
        {
            _selectedLessonType = LessonTypes[0];
        }

        GoBackCommand = new RelayCommand(
            execute: () => _navigation.GoBack(),
            canExecute: () => !IsBusy);

        EnableEditCommand = new RelayCommand(
            execute: () => IsEditMode = true,
            canExecute: () => Lesson is not null && !IsBusy && !IsEditMode);

        CancelEditCommand = new RelayCommand(
            execute: CancelEdit,
            canExecute: () => Lesson is not null && !IsBusy && IsEditMode);

        SaveLessonCommand = new AsyncRelayCommand(
            execute: SaveLessonAsync,
            canExecute: () => Lesson is not null && !IsBusy && IsEditMode);

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
            await RefreshLessonAsync();
            IsEditMode = false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveLessonAsync()
    {
        if (Lesson is null || IsBusy)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(EditableTopic))
        {
            MessageBox.Show(
                "Lesson topic is required.",
                "Validation error",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        if (!DateOnly.TryParse(EditableDate, out var date))
        {
            MessageBox.Show(
                "Date must be valid.",
                "Validation error",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        if (!TimeOnly.TryParse(EditableStartTime, out var startTime))
        {
            MessageBox.Show(
                "Start time must be valid.",
                "Validation error",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        if (!TimeOnly.TryParse(EditableEndTime, out var endTime))
        {
            MessageBox.Show(
                "End time must be valid.",
                "Validation error",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        try
        {
            IsBusy = true;

            await _lessonService.UpdateLessonAsync(
                _lessonId,
                new UpsertLessonDto
                {
                    SubjectId = Lesson.SubjectId,
                    Date = date,
                    StartTime = startTime,
                    EndTime = endTime,
                    Topic = EditableTopic.Trim(),
                    Type = SelectedLessonType
                });

            await RefreshLessonAsync();
            IsEditMode = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Save error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void CancelEdit()
    {
        if (Lesson is null)
        {
            return;
        }

        EditableTopic = Lesson.Topic;
        EditableDate = Lesson.Date.ToString("yyyy-MM-dd");
        EditableStartTime = Lesson.StartTime.ToString("HH:mm");
        EditableEndTime = Lesson.EndTime.ToString("HH:mm");
        SelectedLessonType = Lesson.Type;
        IsEditMode = false;
    }

    private async Task RefreshLessonAsync()
    {
        var lesson = await _lessonService.GetLessonDetailsAsync(_lessonId);
        var lessonForEdit = await _lessonService.GetLessonForEditAsync(_lessonId);

        Lesson = lesson;

        EditableTopic = lessonForEdit.Topic;
        EditableDate = lessonForEdit.Date.ToString("yyyy-MM-dd");
        EditableStartTime = lessonForEdit.StartTime.ToString("HH:mm");
        EditableEndTime = lessonForEdit.EndTime.ToString("HH:mm");
        SelectedLessonType = lessonForEdit.Type;
    }
}

