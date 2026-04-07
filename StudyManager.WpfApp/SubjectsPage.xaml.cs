using System.Windows.Controls;
using StudyManager.WpfApp.ViewModels;
using System.Windows.Input;
using System.Windows;

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
        Loaded -= SubjectsPage_Loaded;
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
