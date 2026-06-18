using System.Windows.Input;

namespace UP.ViewModels;

public class MainMenuViewModel : BaseViewModel
{
    public ICommand StartQuizCommand { get; }
    public ICommand OpenAchievementsCommand { get; }

    public MainMenuViewModel()
    {
        StartQuizCommand = new Command(async () => await Shell.Current.GoToAsync("//QuizPage"));
        OpenAchievementsCommand = new Command(async () => await Shell.Current.GoToAsync("//AchievementsPage"));
    }
}