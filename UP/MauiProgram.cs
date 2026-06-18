using Microsoft.Extensions.Logging;
using UP;
using UP.Services;
using UP.ViewModels;
using UP.Views;

namespace UP
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Oswald-VariableFont_wght.ttf", "Oswald");
                });

            builder.Services.AddSingleton<QuestionService>();
            builder.Services.AddSingleton<AchievementService>();
            builder.Services.AddSingleton<AudioService>();

            builder.Services.AddSingleton<MainMenuViewModel>();
            builder.Services.AddSingleton<AchievementsViewModel>();
            builder.Services.AddTransient<QuizViewModel>();

            builder.Services.AddTransient<MainMenuPage>();
            builder.Services.AddTransient<QuizPage>();
            builder.Services.AddTransient<AchievementsPage>();

            return builder.Build();
        }
    }
}