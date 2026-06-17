namespace QuizOrDie.Views;

public partial class CrashOverlay : ContentView
{
    private TaskCompletionSource<bool>? _tcs;

    // хакерские строки
    private static readonly string[] HackLines =
    {
        "Connecting to fsb.ru...",
        "Access granted.",
        "Scanning brain.exe...",
        "downloading your_data.zip... 14%",
        "downloading your_data.zip... 67%",
        "downloading your_data.zip... DONE",
        "",
        "ERROR: файл 'мозги.db' не найден",
        "Reason: файл никогда не существовал",
    };

    public CrashOverlay()
    {
        InitializeComponent();
    }

    public async Task ShowAsync(int crashType)
    {
        _tcs = new TaskCompletionSource<bool>();

        // Скрываем все панели
        HideAll();
        IsVisible = true;

        switch (crashType)
        {
            case 1: await RunBsodAsync(); break;
            case 2: await RunUpdateAsync(); break;
            case 3: await RunCrackAsync(); break;
            case 4: await RunHackAsync(); break;
            case 5: await RunBatteryAsync(); break;
        }

        IsVisible = false;
        _tcs.SetResult(true);
    }

    public Task WaitForDismissAsync() => _tcs?.Task ?? Task.CompletedTask;

    // сценарии краша

    private async Task RunBsodAsync()
    {
        CrashBSOD.IsVisible = true;
        await Task.Delay(3500);
        BsodHintLabel.Text = "Шучу. Ты просто лох. Смахни вверх.";
        await Task.Delay(2000);
    }

    private async Task RunUpdateAsync()
    {
        CrashUpdate.IsVisible = true;

        // анимация прогресса до 99 с зависанием
        for (int i = 0; i <= 99; i++)
        {
            await UpdateProgress.ProgressTo(i / 100.0, 40, Easing.Linear);
            UpdatePercentLabel.Text = $"{i}%";
        }

        await Task.Delay(2500);
        UpdateHintLabel.Text = "Твой мозг завис на этом вопросе.";
        await Task.Delay(2000);
    }

    private async Task RunCrackAsync()
    {
        CrashCrack.IsVisible = true;
        await Task.Delay(3000);
        await CrashCrack.FadeToAsync(0, 500);
        CrashCrack.Opacity = 1;
    }

    private async Task RunHackAsync()
    {
        CrashHack.IsVisible = true;
        HackLabel.Text = "";

        foreach (var line in HackLines)
        {
            HackLabel.Text += line + "\n";
            await Task.Delay(400);
        }

        await Task.Delay(2000);
    }

    private async Task RunBatteryAsync()
    {
        CrashBattery.IsVisible = true;
        await Task.Delay(2000);

        string[] icons = { "🪫", "🔋", "🔋", "🔋" };
        for (int i = 0; i < icons.Length; i++)
        {
            BatteryIcon.Text = icons[i];
            await Task.Delay(500);
        }

        await Task.Delay(500);
    }

    private void HideAll()
    {
        CrashBSOD.IsVisible = false;
        CrashUpdate.IsVisible = false;
        CrashCrack.IsVisible = false;
        CrashHack.IsVisible = false;
        CrashBattery.IsVisible = false;
    }
}