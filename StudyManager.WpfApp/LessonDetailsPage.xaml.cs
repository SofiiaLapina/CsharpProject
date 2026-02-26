using StudyManager.Presentation;
using System.Windows;
using System.Windows.Controls;

namespace StudyManager.WpfApp;

public partial class LessonDetailsPage : Page
{
    public LessonDetailsPage(LessonViewModel lesson)
    {
        InitializeComponent();
        LoadLessonDetails(lesson);
    }

    private void LoadLessonDetails(LessonViewModel lesson)
    {
        TopicText.Text = $"Topic: {lesson.Topic}";
        DateText.Text = $"Date: {lesson.Date:yyyy-MM-dd}";

        string durationStr = lesson.Duration.Hours > 0
            ? $"{lesson.Duration.Hours}h {lesson.Duration.Minutes}m"
            : $"{lesson.Duration.Minutes}m";

        TimeText.Text = $"Time: {lesson.StartTime:HH:mm} - {lesson.EndTime:HH:mm} (Duration: {durationStr})";
        TypeText.Text = $"Type: {lesson.Type}";
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (NavigationService.CanGoBack)
        {
            NavigationService.GoBack();
        }
    }
}