using StudyManager.Presentation;
using StudyManager.Services;

namespace StudyManager.ConsoleApp;

internal static class Program
{
    private static readonly StorageService Storage = new();

    public static void Main()
    {
        while (true)
        {
            var subjects = Storage.GetSubjects()
                .Select(s => new SubjectViewModel(s))
                .ToList();
            Console.Clear();
            Console.WriteLine("Subjects:\n");

            for (int i = 0; i < subjects.Count; i++)
            {
                var s = subjects[i];
                Console.WriteLine($"{i + 1}. {s.Name} | {s.EctsCredits} ECTS | {s.Area}");
            }

            Console.WriteLine("\n0. Exit");
            Console.Write("\nChoose subject number: ");

            if (!int.TryParse(Console.ReadLine(), out int choice))
                continue;

            if (choice == 0)
                return;

            if (choice < 1 || choice > subjects.Count)
                continue;

            ShowSubjectDetails(subjects[choice - 1]);
        }
    }

    private static void ShowSubjectDetails(SubjectViewModel subject)
    {
        if (!subject.LessonsLoaded)
        {
            var lessonsData = Storage.GetLessonsBySubjectId(subject.Id);
            subject.LoadLessons(lessonsData);
        }

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Subject details:\n");
            Console.WriteLine($"Name: {subject.Name}");
            Console.WriteLine($"ECTS: {subject.EctsCredits}");
            Console.WriteLine($"Area: {subject.Area}");
            Console.WriteLine($"Total duration: {FormatDuration(subject.TotalDuration)}");
            Console.WriteLine();

            if (subject.Lessons.Count == 0)
            {
                Console.WriteLine("No lessons for this subject.\n");
            }
            else
            {
                Console.WriteLine("Lessons:\n");
                for (int i = 0; i < subject.Lessons.Count; i++)
                {
                    var l = subject.Lessons[i];
                    Console.WriteLine(
                        $"{i + 1}. {l.Date:yyyy-MM-dd} {l.StartTime:HH:mm}-{l.EndTime:HH:mm} [{l.Type}] {l.Topic} ({FormatDuration(l.Duration)})");
                }
                Console.WriteLine();
            }

            Console.WriteLine("0. Back");
            Console.Write("Choose lesson number for details (or 0): ");

            if (!int.TryParse(Console.ReadLine(), out int lessonChoice))
                continue;

            if (lessonChoice == 0)
                return;

            if (lessonChoice < 1 || lessonChoice > subject.Lessons.Count)
                continue;

            ShowLessonDetails(subject.Lessons[lessonChoice - 1]);
        }
    }

    private static void ShowLessonDetails(LessonViewModel lesson)
    {
        Console.Clear();
        Console.WriteLine("Lesson details:\n");
        Console.WriteLine($"Date: {lesson.Date:yyyy-MM-dd}");
        Console.WriteLine($"Time: {lesson.StartTime:HH:mm} - {lesson.EndTime:HH:mm}");
        Console.WriteLine($"Type: {lesson.Type}");
        Console.WriteLine($"Topic: {lesson.Topic}");
        Console.WriteLine($"Duration: {FormatDuration(lesson.Duration)}");
        Console.WriteLine("\nPress Enter to go back...");
        Console.ReadLine();
    }

    private static string FormatDuration(TimeSpan duration)
    {
        if (duration == TimeSpan.Zero)
            return "0m";

        var hours = (int)duration.TotalHours;
        var minutes = duration.Minutes;

        if (hours <= 0)
            return $"{minutes}m";

        return minutes == 0 ? $"{hours}h" : $"{hours}h {minutes}m";
    }
}