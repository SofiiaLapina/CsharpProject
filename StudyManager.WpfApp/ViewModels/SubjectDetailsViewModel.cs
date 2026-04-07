using System.Collections.ObjectModel;
using System.Windows;
using StudyManager.Services.Dtos.Lessons;
using StudyManager.Services.Dtos.Subjects;
using StudyManager.Services.Interfaces;
using StudyManager.Storage;
using StudyManager.WpfApp.Infrastructure;

namespace StudyManager.WpfApp.ViewModels;

public sealed class SubjectDetailsViewModel : ViewModelBase
{
    private readonly Guid _subjectId;
    private readonly ISubjectService _subjectService;
    private readonly ILessonService _lessonService;
    private readonly INavigationService _navigation;
    private readonly List<LessonListItemDto> _allLessons = new();

    private SubjectDetailsDto? _subject;
    private LessonListItemDto? _selectedLesson;
    private bool _isBusy;
    private bool _isEditMode;
    private string _editableName = string.Empty;
    private string _editableEctsCredits = string.Empty;
    private KnowledgeArea _selectedKnowledgeArea;

    private string _newLessonTopic = string.Empty;
    private string _newLessonDate = string.Empty;
    private string _newLessonStartTime = string.Empty;
    private string _newLessonEndTime = string.Empty;
    private LessonType _selectedLessonType;

    private string _lessonSearchText = string.Empty;
    private string _selectedLessonSortOption = "Date Desc";

    public SubjectDetailsDto? Subject
    {
        get => _subject;
        private set
        {
            _subject = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(SubjectIdText));
            OnPropertyChanged(nameof(TotalDurationText));

            OpenLessonCommand.RaiseCanExecuteChanged();
            EnableEditCommand.RaiseCanExecuteChanged();
            CancelEditCommand.RaiseCanExecuteChanged();
            SaveSubjectCommand.RaiseCanExecuteChanged();
            AddLessonCommand.RaiseCanExecuteChanged();
            DeleteLessonCommand.RaiseCanExecuteChanged();
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
            DeleteLessonCommand.RaiseCanExecuteChanged();
        }
    }

    public ObservableCollection<LessonListItemDto> Lessons { get; } = new();
    public ObservableCollection<KnowledgeArea> KnowledgeAreas { get; } = new();
    public ObservableCollection<LessonType> LessonTypes { get; } = new();
    public ObservableCollection<string> LessonSortOptions { get; } = new()
    {
        "Date Desc",
        "Date Asc",
        "Topic A-Z",
        "Topic Z-A",
        "Duration Asc",
        "Duration Desc",
        "Type A-Z",
        "Type Z-A"
    };

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            _isBusy = value;
            OnPropertyChanged();
            OpenLessonCommand.RaiseCanExecuteChanged();
            GoBackCommand.RaiseCanExecuteChanged();
            LoadCommand.RaiseCanExecuteChanged();
            EnableEditCommand.RaiseCanExecuteChanged();
            CancelEditCommand.RaiseCanExecuteChanged();
            SaveSubjectCommand.RaiseCanExecuteChanged();
            AddLessonCommand.RaiseCanExecuteChanged();
            DeleteLessonCommand.RaiseCanExecuteChanged();
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
            SaveSubjectCommand.RaiseCanExecuteChanged();
        }
    }

    public string EditableName
    {
        get => _editableName;
        set
        {
            _editableName = value;
            OnPropertyChanged();
            SaveSubjectCommand.RaiseCanExecuteChanged();
        }
    }

    public string EditableEctsCredits
    {
        get => _editableEctsCredits;
        set
        {
            _editableEctsCredits = value;
            OnPropertyChanged();
            SaveSubjectCommand.RaiseCanExecuteChanged();
        }
    }

    public KnowledgeArea SelectedKnowledgeArea
    {
        get => _selectedKnowledgeArea;
        set
        {
            _selectedKnowledgeArea = value;
            OnPropertyChanged();
            SaveSubjectCommand.RaiseCanExecuteChanged();
        }
    }

    public string NewLessonTopic
    {
        get => _newLessonTopic;
        set
        {
            _newLessonTopic = value;
            OnPropertyChanged();
            AddLessonCommand.RaiseCanExecuteChanged();
        }
    }

    public string NewLessonDate
    {
        get => _newLessonDate;
        set
        {
            _newLessonDate = value;
            OnPropertyChanged();
            AddLessonCommand.RaiseCanExecuteChanged();
        }
    }

    public string NewLessonStartTime
    {
        get => _newLessonStartTime;
        set
        {
            _newLessonStartTime = value;
            OnPropertyChanged();
            AddLessonCommand.RaiseCanExecuteChanged();
        }
    }

    public string NewLessonEndTime
    {
        get => _newLessonEndTime;
        set
        {
            _newLessonEndTime = value;
            OnPropertyChanged();
            AddLessonCommand.RaiseCanExecuteChanged();
        }
    }

    public LessonType SelectedLessonType
    {
        get => _selectedLessonType;
        set
        {
            _selectedLessonType = value;
            OnPropertyChanged();
            AddLessonCommand.RaiseCanExecuteChanged();
        }
    }

    public string LessonSearchText
    {
        get => _lessonSearchText;
        set
        {
            _lessonSearchText = value;
            OnPropertyChanged();
            ApplyLessonsView();
        }
    }

    public string SelectedLessonSortOption
    {
        get => _selectedLessonSortOption;
        set
        {
            _selectedLessonSortOption = value;
            OnPropertyChanged();
            ApplyLessonsView();
        }
    }

    public string Title => Subject?.Name ?? string.Empty;
    public string SubjectIdText => Subject is null ? string.Empty : $"Id: {Subject.Id}";
    public string TotalDurationText => Subject is null ? string.Empty : $"Total: {Subject.TotalDuration:hh\\:mm\\:ss}";

    public RelayCommand OpenLessonCommand { get; }
    public RelayCommand GoBackCommand { get; }
    public RelayCommand EnableEditCommand { get; }
    public RelayCommand CancelEditCommand { get; }
    public AsyncRelayCommand SaveSubjectCommand { get; }
    public AsyncRelayCommand AddLessonCommand { get; }
    public AsyncRelayCommand DeleteLessonCommand { get; }
    public AsyncRelayCommand LoadCommand { get; }

    public SubjectDetailsViewModel(
        Guid subjectId,
        ISubjectService subjectService,
        ILessonService lessonService,
        INavigationService navigation)
    {
        _subjectId = subjectId;
        _subjectService = subjectService;
        _lessonService = lessonService;
        _navigation = navigation;

        foreach (var area in _subjectService.GetKnowledgeAreas())
        {
            KnowledgeAreas.Add(area);
        }

        if (KnowledgeAreas.Count > 0)
        {
            _selectedKnowledgeArea = KnowledgeAreas[0];
        }

        foreach (var lessonType in _lessonService.GetLessonTypes())
        {
            LessonTypes.Add(lessonType);
        }

        if (LessonTypes.Count > 0)
        {
            _selectedLessonType = LessonTypes[0];
        }

        OpenLessonCommand = new RelayCommand(
            execute: () => _navigation.NavigateToLessonDetails(SelectedLesson!.Id),
            canExecute: () => SelectedLesson is not null && !IsBusy);

        GoBackCommand = new RelayCommand(
            execute: () => _navigation.GoBack(),
            canExecute: () => !IsBusy);

        EnableEditCommand = new RelayCommand(
            execute: () => IsEditMode = true,
            canExecute: () => Subject is not null && !IsBusy && !IsEditMode);

        CancelEditCommand = new RelayCommand(
            execute: CancelEdit,
            canExecute: () => Subject is not null && !IsBusy && IsEditMode);

        SaveSubjectCommand = new AsyncRelayCommand(
            execute: SaveSubjectAsync,
            canExecute: () => Subject is not null && !IsBusy && IsEditMode);

        AddLessonCommand = new AsyncRelayCommand(
            execute: AddLessonAsync,
            canExecute: () => Subject is not null && !IsBusy);

        DeleteLessonCommand = new AsyncRelayCommand(
            execute: DeleteSelectedLessonAsync,
            canExecute: () => SelectedLesson is not null && !IsBusy);

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
            await RefreshSubjectAsync();
            IsEditMode = false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveSubjectAsync()
    {
        if (Subject is null || IsBusy)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(EditableName))
        {
            MessageBox.Show(
                "Subject name is required.",
                "Validation error",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        if (!int.TryParse(EditableEctsCredits, out var ectsCredits) || ectsCredits <= 0)
        {
            MessageBox.Show(
                "ECTS credits must be a positive integer.",
                "Validation error",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        try
        {
            IsBusy = true;

            var updated = await _subjectService.UpdateSubjectAsync(
                _subjectId,
                new UpsertSubjectDto
                {
                    Name = EditableName.Trim(),
                    EctsCredits = ectsCredits,
                    Area = SelectedKnowledgeArea
                });

            ApplySubject(updated);
            EditableName = updated.Name;
            EditableEctsCredits = updated.EctsCredits.ToString();
            SelectedKnowledgeArea = updated.Area;

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

    private async Task AddLessonAsync()
    {
        if (Subject is null || IsBusy)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(NewLessonTopic))
        {
            MessageBox.Show(
                "Lesson topic is required.",
                "Validation error",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        if (!DateOnly.TryParse(NewLessonDate, out var date))
        {
            MessageBox.Show(
                "Date must be valid.",
                "Validation error",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        if (!TimeOnly.TryParse(NewLessonStartTime, out var startTime))
        {
            MessageBox.Show(
                "Start time must be valid.",
                "Validation error",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        if (!TimeOnly.TryParse(NewLessonEndTime, out var endTime))
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

            await _lessonService.CreateLessonAsync(new UpsertLessonDto
            {
                SubjectId = Subject.Id,
                Date = date,
                StartTime = startTime,
                EndTime = endTime,
                Topic = NewLessonTopic.Trim(),
                Type = SelectedLessonType
            });

            NewLessonTopic = string.Empty;
            NewLessonDate = string.Empty;
            NewLessonStartTime = string.Empty;
            NewLessonEndTime = string.Empty;

            await RefreshSubjectAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Add lesson error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DeleteSelectedLessonAsync()
    {
        if (SelectedLesson is null || IsBusy)
        {
            return;
        }

        var lesson = SelectedLesson;

        var answer = MessageBox.Show(
            $"Delete lesson '{lesson.Topic}'?",
            "Confirm delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (answer != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            IsBusy = true;

            await _lessonService.DeleteLessonAsync(lesson.Id);
            SelectedLesson = null;

            await RefreshSubjectAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Delete lesson error",
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
        if (Subject is null)
        {
            return;
        }

        EditableName = Subject.Name;
        EditableEctsCredits = Subject.EctsCredits.ToString();
        SelectedKnowledgeArea = Subject.Area;
        IsEditMode = false;
    }

    private async Task RefreshSubjectAsync()
    {
        var subject = await _subjectService.GetSubjectDetailsAsync(_subjectId);
        var subjectForEdit = await _subjectService.GetSubjectForEditAsync(_subjectId);

        ApplySubject(subject);
        ApplyEditableSubject(subjectForEdit);
    }

    private void ApplySubject(SubjectDetailsDto subject)
    {
        Subject = subject;

        _allLessons.Clear();
        _allLessons.AddRange(subject.Lessons);

        ApplyLessonsView();
    }

    private void ApplyEditableSubject(SubjectEditDto subject)
    {
        EditableName = subject.Name;
        EditableEctsCredits = subject.EctsCredits.ToString();
        SelectedKnowledgeArea = subject.Area;
    }

    private void ApplyLessonsView()
    {
        IEnumerable<LessonListItemDto> query = _allLessons;

        if (!string.IsNullOrWhiteSpace(LessonSearchText))
        {
            var term = LessonSearchText.Trim();
            query = query.Where(l =>
                l.Topic.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                l.Type.ToString().Contains(term, StringComparison.OrdinalIgnoreCase) ||
                l.Date.ToString().Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        query = SelectedLessonSortOption switch
        {
            "Date Asc" => query.OrderBy(l => l.Date).ThenBy(l => l.StartTime),
            "Topic A-Z" => query.OrderBy(l => l.Topic),
            "Topic Z-A" => query.OrderByDescending(l => l.Topic),
            "Duration Asc" => query.OrderBy(l => l.Duration).ThenBy(l => l.Topic),
            "Duration Desc" => query.OrderByDescending(l => l.Duration).ThenBy(l => l.Topic),
            "Type A-Z" => query.OrderBy(l => l.Type.ToString()).ThenBy(l => l.Topic),
            "Type Z-A" => query.OrderByDescending(l => l.Type.ToString()).ThenBy(l => l.Topic),
            _ => query.OrderByDescending(l => l.Date).ThenByDescending(l => l.StartTime)
        };

        var selectedId = SelectedLesson?.Id;

        Lessons.Clear();
        foreach (var lesson in query)
        {
            Lessons.Add(lesson);
        }

        if (selectedId is not null)
        {
            SelectedLesson = Lessons.FirstOrDefault(l => l.Id == selectedId.Value);
        }
    }
}
