using System.Collections.ObjectModel;
using System.Linq;
using StudyManager.Services.Dtos.Subjects;
using StudyManager.Services.Interfaces;
using StudyManager.WpfApp.Infrastructure;

namespace StudyManager.WpfApp.ViewModels;

public sealed class SubjectsViewModel : ViewModelBase
{
    private readonly ISubjectService _subjectService;
    private readonly INavigationService _navigation;

    private SubjectListItemDto? _selectedSubject;
    private bool _isBusy;

    public ObservableCollection<SubjectListItemDto> Subjects { get; } = new();

    public SubjectListItemDto? SelectedSubject
    {
        get => _selectedSubject;
        set
        {
            _selectedSubject = value;
            OnPropertyChanged();
            OpenSubjectCommand.RaiseCanExecuteChanged();
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
            LoadCommand.RaiseCanExecuteChanged();
        }
    }

    public RelayCommand OpenSubjectCommand { get; }
    public AsyncRelayCommand LoadCommand { get; }

    public SubjectsViewModel(ISubjectService subjectService, INavigationService navigation)
    {
        _subjectService = subjectService;
        _navigation = navigation;

        OpenSubjectCommand = new RelayCommand(
            execute: () => _navigation.NavigateToSubjectDetails(SelectedSubject!.Id),
            canExecute: () => SelectedSubject is not null && !IsBusy);

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
}
