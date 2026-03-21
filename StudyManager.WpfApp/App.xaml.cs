using Microsoft.Extensions.DependencyInjection;
using StudyManager.Repositories;
using StudyManager.Services;
using StudyManager.Services.Interfaces;
using StudyManager.WpfApp.Infrastructure;
using StudyManager.WpfApp.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace StudyManager.WpfApp;

public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigureServices(services);

        ServiceProvider = services.BuildServiceProvider();

        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();

        mainWindow.MainFrame.Navigate(ServiceProvider.GetRequiredService<SubjectsPage>());
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ISubjectRepository, SubjectRepository>();
        services.AddSingleton<ILessonRepository, LessonRepository>();

        services.AddSingleton<ISubjectService, SubjectService>();
        services.AddSingleton<ILessonService, LessonService>();

        services.AddSingleton<MainWindow>();

        services.AddTransient<SubjectsViewModel>();
        services.AddTransient<Func<Guid, SubjectDetailsViewModel>>(sp =>
            subjectId => ActivatorUtilities.CreateInstance<SubjectDetailsViewModel>(sp, subjectId));
        services.AddTransient<Func<Guid, LessonDetailsViewModel>>(sp =>
            lessonId => ActivatorUtilities.CreateInstance<LessonDetailsViewModel>(sp, lessonId));

        services.AddTransient<SubjectsPage>();
        services.AddTransient<Func<Guid, SubjectDetailsPage>>(sp =>
            subjectId => ActivatorUtilities.CreateInstance<SubjectDetailsPage>(
                sp,
                sp.GetRequiredService<Func<Guid, SubjectDetailsViewModel>>()(subjectId)));

        services.AddTransient<Func<Guid, LessonDetailsPage>>(sp =>
            lessonId => ActivatorUtilities.CreateInstance<LessonDetailsPage>(
                sp,
                sp.GetRequiredService<Func<Guid, LessonDetailsViewModel>>()(lessonId)));

        services.AddSingleton<INavigationService>(sp =>
        {
            var mainWindow = sp.GetRequiredService<MainWindow>();
            Frame frame = mainWindow.MainFrame;

            return new NavigationService(
                frame,
                subjectId => sp.GetRequiredService<Func<Guid, SubjectDetailsPage>>()(subjectId),
                lessonId => sp.GetRequiredService<Func<Guid, LessonDetailsPage>>()(lessonId)
            );
        });
    }
}