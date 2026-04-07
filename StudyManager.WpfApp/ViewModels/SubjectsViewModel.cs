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

    private SubjectListItemDto? _selectedSubject;
    private bool _isBusy;
    private string _newSubjectName = string.Empty;
    private string _newSubjectEctsCredits = string.Empty;
    private KnowledgeArea _selectedKnowledgeArea;

    public ObservableCollection<SubjectListItemDto> Subjects { get; } = new();
    public ObservableCollection<KnowledgeArea> KnowledgeAreas { get; } = new();

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
            Subjects.Clear();
            SelectedSubject = null;

            var subjects = await _subjectService.GetSubjectsAsync();

            foreach (var subject in subjects)
            {
                Subjects.Add(subject);
            }
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

            var listItem = new SubjectListItemDto
            {
                Id = createdSubject.Id,
                Name = createdSubject.Name,
                EctsCredits = createdSubject.EctsCredits,
                Area = createdSubject.Area
            };

            Subjects.Add(listItem);
            SelectedSubject = listItem;

            NewSubjectName = string.Empty;
            NewSubjectEctsCredits = string.Empty;
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

            Subjects.Remove(subject);
            SelectedSubject = null;
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
}
