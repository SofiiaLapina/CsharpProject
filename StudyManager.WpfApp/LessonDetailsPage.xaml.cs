using System.Windows;
using System.Windows.Controls;
using StudyManager.WpfApp.ViewModels;

namespace StudyManager.WpfApp;

public partial class LessonDetailsPage : Page
{
    private readonly LessonDetailsViewModel _viewModel;

    public LessonDetailsPage(LessonDetailsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        Loaded += LessonDetailsPage_Loaded;
    }

    private async void LessonDetailsPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= LessonDetailsPage_Loaded;
        await _viewModel.LoadAsync();
    }
}
