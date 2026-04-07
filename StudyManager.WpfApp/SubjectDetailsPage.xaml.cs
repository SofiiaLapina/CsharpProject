using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StudyManager.WpfApp.ViewModels;

namespace StudyManager.WpfApp;

public partial class SubjectDetailsPage : Page
{
    private readonly SubjectDetailsViewModel _viewModel;

    public SubjectDetailsPage(SubjectDetailsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        Loaded += SubjectDetailsPage_Loaded;
    }

    private async void SubjectDetailsPage_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.LoadAsync();
    }

    private void LessonsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_viewModel.OpenLessonCommand.CanExecute(null))
        {
            _viewModel.OpenLessonCommand.Execute(null);
        }
    }
}