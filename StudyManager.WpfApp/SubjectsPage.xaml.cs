using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StudyManager.WpfApp.ViewModels;

namespace StudyManager.WpfApp;

public partial class SubjectsPage : Page
{
    private readonly SubjectsViewModel _viewModel;

    public SubjectsPage(SubjectsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        Loaded += SubjectsPage_Loaded;
    }

    private async void SubjectsPage_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.LoadAsync();
    }

    private void SubjectsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_viewModel.OpenSubjectCommand.CanExecute(null))
        {
            _viewModel.OpenSubjectCommand.Execute(null);
        }
    }
}