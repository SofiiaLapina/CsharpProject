using StudyManager.Presentation;
using StudyManager.Services;
using System.Windows;
using System.Windows.Controls;

namespace StudyManager.WpfApp;

public partial class SubjectDetailsPage : Page
{
    private readonly IStorageService _storageService;
    private readonly SubjectViewModel _subject;

    public SubjectDetailsPage(IStorageService storageService, SubjectViewModel subject)
    {
        InitializeComponent();
        _storageService = storageService;
        _subject = subject;
        LoadSubjectDetails();
    }

    private void LoadSubjectDetails()
    {
        SubjectNameText.Text = _subject.Name;
        SubjectInfoText.Text = $"ECTS Credits: {_subject.EctsCredits} | Area: {_subject.Area}";

        if (!_subject.LessonsLoaded)
        {
            var lessonsData = _storageService.GetLessonsBySubjectId(_subject.Id);
            _subject.LoadLessons(lessonsData);
        }

        LessonsList.ItemsSource = _subject.Lessons;
    }

    private void LessonsList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (LessonsList.SelectedItem is LessonViewModel selectedLesson)
        {
            NavigationService.Navigate(new LessonDetailsPage(selectedLesson));
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (NavigationService.CanGoBack)
        {
            NavigationService.GoBack();
        }
    }
}