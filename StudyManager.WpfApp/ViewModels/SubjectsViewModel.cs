using System.Collections.ObjectModel;
using System.Windows;
using StudyManager.Services.Dtos.Subjects;
using StudyManager.Services.Interfaces;
using StudyManager.Storage;
using StudyManager.WpfApp.Infrastructure;

namespace StudyManager.WpfApp.ViewModels;

public sealed class SubjectsViewModel : ViewModelBase
{
    private readonly ISubjectService _subjectService;
    private readonly INavigationService _navigation;
    private readonly List<SubjectListItemDto> _allSubjects = new();

    private SubjectListItemDto? _selectedSubject;
    private bool _isBusy;
    private string _newSubjectName = string.Empty;
    private string _newSubjectEctsCredits = string.Empty;
    private KnowledgeArea _selectedKnowledgeArea;
    private string _searchText = string.Empty;
    private string _selectedSortOption = "Name A-Z";

    public ObservableCollection<SubjectListItemDto> Subjects { get; } = new();
    public ObservableCollection<KnowledgeArea> KnowledgeAreas { get; } = new();
    public ObservableCollection<string> SortOptions { get; } = new()
    {
        "Name A-Z",
        "Name Z-A",
        "ECTS Asc",
        "ECTS Desc"
    };

    public SubjectListItemDto? SelectedSubject
    {
        get => _selectedSubject;
        set
        {
            _selectedSubject = value;
            OnPropertyChanged();
            OpenSubjectCommand.RaiseCanExecuteChanged();
            DeleteSubjectCommand.RaiseCanExecuteChanged();
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            _isBusy = value;
            OnPropertyChanged();
            OpenSubjectCommand.RaiseCanExecuteChanged();
            DeleteSubjectCommand.RaiseCanExecuteChanged();
            CreateSubjectCommand.RaiseCanExecuteChanged();
            LoadCommand.RaiseCanExecuteChanged();
        }
    }

    public string NewSubjectName
    {
        get => _newSubjectName;
        set
        {
            _newSubjectName = value;
            OnPropertyChanged();
            CreateSubjectCommand.RaiseCanExecuteChanged();
        }
    }

    public string NewSubjectEctsCredits
    {
        get => _newSubjectEctsCredits;
        set
        {
            _newSubjectEctsCredits = value;
            OnPropertyChanged();
            CreateSubjectCommand.RaiseCanExecuteChanged();
        }
    }

    public KnowledgeArea SelectedKnowledgeArea
    {
        get => _selectedKnowledgeArea;
        set
        {
            _selectedKnowledgeArea = value;
            OnPropertyChanged();
            CreateSubjectCommand.RaiseCanExecuteChanged();
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
            ApplySubjectView();
        }
    }

    public string SelectedSortOption
    {
        get => _selectedSortOption;
        set
        {
            _selectedSortOption = value;
            OnPropertyChanged();
            ApplySubjectView();
        }
    }

    public RelayCommand OpenSubjectCommand { get; }
    public AsyncRelayCommand DeleteSubjectCommand { get; }
    public AsyncRelayCommand CreateSubjectCommand { get; }
    public AsyncRelayCommand LoadCommand { get; }

    public SubjectsViewModel(ISubjectService subjectService, INavigationService navigation)
    {
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

        OpenSubjectCommand = new RelayCommand(
            execute: () => _navigation.NavigateToSubjectDetails(SelectedSubject!.Id),
            canExecute: () => SelectedSubject is not null && !IsBusy);

        DeleteSubjectCommand = new AsyncRelayCommand(
            execute: DeleteSelectedSubjectAsync,
            canExecute: () => SelectedSubject is not null && !IsBusy);

        CreateSubjectCommand = new AsyncRelayCommand(
            execute: CreateSubjectAsync,
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
            SelectedSubject = null;

            var subjects = await _subjectService.GetSubjectsAsync();

            _allSubjects.Clear();
            _allSubjects.AddRange(subjects);

            ApplySubjectView();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CreateSubjectAsync()
    {
        if (IsBusy)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(NewSubjectName))
        {
            MessageBox.Show(
                "Subject name is required.",
                "Validation error",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        if (!int.TryParse(NewSubjectEctsCredits, out var ectsCredits) || ectsCredits <= 0)
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

            var createdSubject = await _subjectService.CreateSubjectAsync(new UpsertSubjectDto
            {
                Name = NewSubjectName.Trim(),
                EctsCredits = ectsCredits,
                Area = SelectedKnowledgeArea
            });

            _allSubjects.Add(new SubjectListItemDto
            {
                Id = createdSubject.Id,
                Name = createdSubject.Name,
                EctsCredits = createdSubject.EctsCredits,
                Area = createdSubject.Area
            });

            NewSubjectName = string.Empty;
            NewSubjectEctsCredits = string.Empty;

            ApplySubjectView();
            SelectedSubject = Subjects.FirstOrDefault(s => s.Id == createdSubject.Id);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Create error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DeleteSelectedSubjectAsync()
    {
        if (SelectedSubject is null || IsBusy)
        {
            return;
        }

        var subject = SelectedSubject;

        var answer = MessageBox.Show(
            $"Delete subject '{subject.Name}' and all its lessons?",
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

            await _subjectService.DeleteSubjectAsync(subject.Id);

            _allSubjects.RemoveAll(s => s.Id == subject.Id);
            SelectedSubject = null;
            ApplySubjectView();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Delete error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ApplySubjectView()
    {
        IEnumerable<SubjectListItemDto> query = _allSubjects;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var term = SearchText.Trim();
            query = query.Where(s =>
                s.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                s.Area.ToString().Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        query = SelectedSortOption switch
        {
            "Name Z-A" => query.OrderByDescending(s => s.Name),
            "ECTS Asc" => query.OrderBy(s => s.EctsCredits).ThenBy(s => s.Name),
            "ECTS Desc" => query.OrderByDescending(s => s.EctsCredits).ThenBy(s => s.Name),
            _ => query.OrderBy(s => s.Name)
        };

        var selectedId = SelectedSubject?.Id;

        Subjects.Clear();
        foreach (var subject in query)
        {
            Subjects.Add(subject);
        }

        if (selectedId is not null)
        {
            SelectedSubject = Subjects.FirstOrDefault(s => s.Id == selectedId.Value);
        }
    }
}
