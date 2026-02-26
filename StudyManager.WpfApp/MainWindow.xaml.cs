using StudyManager.Services;
using System.Windows;

namespace StudyManager.WpfApp;

public partial class MainWindow : Window
{
    public MainWindow(IStorageService storageService)
    {
        InitializeComponent();

        MainFrame.Navigate(new SubjectsPage(storageService));
    }
}