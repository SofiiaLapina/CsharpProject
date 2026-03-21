using System.Windows.Controls;
using StudyManager.WpfApp.ViewModels;

namespace StudyManager.WpfApp;

public partial class LessonDetailsPage : Page
{
    public LessonDetailsPage(LessonDetailsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
