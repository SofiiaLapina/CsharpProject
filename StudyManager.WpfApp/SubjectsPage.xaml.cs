using StudyManager.Presentation;
using StudyManager.Services;
using System.Linq;
using System.Windows.Controls;

namespace StudyManager.WpfApp;

public partial class SubjectsPage : Page
{
    private readonly IStorageService _storageService;

    public SubjectsPage(IStorageService storageService)
    {
        InitializeComponent();
        _storageService = storageService;
        LoadData();
    }

    private void LoadData()
    {
        var subjects = _storageService.GetSubjects()
            .Select(s => new SubjectViewModel(s))
            .ToList();
        SubjectsList.ItemsSource = subjects;
    }

    private void SubjectsList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (SubjectsList.SelectedItem is SubjectViewModel selectedSubject)
        {
            NavigationService.Navigate(new SubjectDetailsPage(_storageService, selectedSubject));
        }
    }
}