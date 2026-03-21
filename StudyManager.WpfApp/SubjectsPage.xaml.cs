using System.Windows.Controls;
using StudyManager.WpfApp.ViewModels;

namespace StudyManager.WpfApp;

public partial class SubjectsPage : Page
{
    public SubjectsPage(SubjectsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}