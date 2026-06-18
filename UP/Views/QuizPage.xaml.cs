using UP.ViewModels;

namespace UP.Views;

public partial class QuizPage : ContentPage
{
    private readonly QuizViewModel _vm;

    public QuizPage(QuizViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;

        _vm.OnCrashTriggered += OnCrashTriggered;
        _vm.OnRewardTriggered += OnRewardTriggered;
        _vm.OnAchievementUnlocked += OnAchievementUnlocked;

        _vm.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(QuizViewModel.IsGameOver) && _vm.IsGameOver)
                ShowGameOver();
        };
    }

    // жизненный цикл

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.InitializeAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _vm.OnCrashTriggered -= OnCrashTriggered;
        _vm.OnRewardTriggered -= OnRewardTriggered;
        _vm.OnAchievementUnlocked -= OnAchievementUnlocked;
    }

    // обработчики событий

    private async void OnCrashTriggered(int crashType)
    {
        MainContent.IsEnabled = false;

        // ищем оверлей явно по типу — обходим проблему с x:Name
        var overlay = this.FindByName<CrashOverlay>("CrashOverlayView");
        if (overlay != null)
            await overlay.ShowAsync(crashType);

        MainContent.IsEnabled = true;
        _vm.ContinueAfterOverlay();
    }

    private void OnRewardTriggered(int rewardType, string text)
    {
        RewardLabel.Text = text;
        RewardOverlay.IsVisible = true;
        MainContent.IsEnabled = false;
    }

    private async void OnAchievementUnlocked(Models.Achievement ach)
    {
        // показываем уведомление
        await DisplayAlertAsync("Новая ачивка!", $"{ach.Emoji} {ach.Title}", "Ок");
    }

    // кнопки оверлеев

    private void OnContinueClicked(object sender, EventArgs e)
    {
        RewardOverlay.IsVisible = false;
        MainContent.IsEnabled = true;
        _vm.ContinueAfterOverlay();
    }

    private void OnRestartClicked(object sender, EventArgs e)
    {
        GameOverOverlay.IsVisible = false;
        _vm.RestartGame();
    }

    private async void OnMenuClicked(object sender, EventArgs e)
    {
        GameOverOverlay.IsVisible = false;
        await Shell.Current.GoToAsync("//MainMenu");
    }

    // экран конца игры

    private void ShowGameOver()
    {
        var result = _vm.GameResult;
        GameOverTitle.Text = result.CorrectAnswers >= result.TotalQuestions / 2
            ? "Молодец!" : "Провал!";
        GameOverScore.Text = $"Очков: {_vm.Score}";
        GameOverOverlay.IsVisible = true;
    }
}