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
    private readonly INavigationService _navigation;

    private SubjectDetailsDto? _subject;
    private LessonListItemDto? _selectedLesson;
    private bool _isBusy;
    private bool _isEditMode;
    private string _editableName = string.Empty;
    private string _editableEctsCredits = string.Empty;
    private KnowledgeArea _selectedKnowledgeArea;

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
    public ObservableCollection<KnowledgeArea> KnowledgeAreas { get; } = new();

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

    public string Title => Subject?.Name ?? string.Empty;
    public string SubjectIdText => Subject is null ? string.Empty : $"Id: {Subject.Id}";
    public string TotalDurationText => Subject is null ? string.Empty : $"Total: {Subject.TotalDuration:hh\\:mm\\:ss}";

    public RelayCommand OpenLessonCommand { get; }
    public RelayCommand GoBackCommand { get; }
    public RelayCommand EnableEditCommand { get; }
    public RelayCommand CancelEditCommand { get; }
    public AsyncRelayCommand SaveSubjectCommand { get; }
    public AsyncRelayCommand LoadCommand { get; }

    public SubjectDetailsViewModel(
        Guid subjectId,
        ISubjectService subjectService,
        INavigationService navigation)
    {
        _subjectId = subjectId;
        _subjectService = subjectService;
        _navigation = navigation;

        foreach (var area in _subjectService.GetKnowledgeAreas())
        {
            KnowledgeAreas.Add(area);
        }

        if (KnowledgeAreas.Count > 0)
        {
            _selectedKnowledgeArea = KnowledgeAreas[0];
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
            var subjectForEdit = await _subjectService.GetSubjectForEditAsync(_subjectId);

            ApplySubject(subject);
            ApplyEditableSubject(subjectForEdit);

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

    private void ApplySubject(SubjectDetailsDto subject)
    {
        Subject = subject;

        Lessons.Clear();
        foreach (var lesson in subject.Lessons)
        {
            Lessons.Add(lesson);
        }
    }

    private void ApplyEditableSubject(SubjectEditDto subject)
    {
        EditableName = subject.Name;
        EditableEctsCredits = subject.EctsCredits.ToString();
        SelectedKnowledgeArea = subject.Area;
    }
}