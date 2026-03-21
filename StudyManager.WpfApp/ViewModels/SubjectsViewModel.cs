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

    public ObservableCollection<SubjectListItemDto> Subjects { get; } = new();

    private SubjectListItemDto? _selectedSubject;
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

    public RelayCommand OpenSubjectCommand { get; }

    public SubjectsViewModel(ISubjectService subjectService, INavigationService navigation)
    {
        _subjectService = subjectService;
        _navigation = navigation;

        OpenSubjectCommand = new RelayCommand(
            execute: () => _navigation.NavigateToSubjectDetails(SelectedSubject!.Id),
            canExecute: () => SelectedSubject is not null
        );

        Load();
    }

    private void Load()
    {
        Subjects.Clear();
        foreach (var s in _subjectService.GetSubjects())
            Subjects.Add(s);
    }
}
