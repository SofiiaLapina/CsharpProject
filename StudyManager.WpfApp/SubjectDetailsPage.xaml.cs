using System.Windows.Controls;
using StudyManager.WpfApp.ViewModels;

namespace StudyManager.WpfApp;

public partial class SubjectDetailsPage : Page
{
    public SubjectDetailsPage(SubjectDetailsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
