using System.Windows.Input;
using QuizOrDie.Models;
using QuizOrDie.Services;

namespace QuizOrDie.ViewModels;

/// главная логика викторины
public class QuizViewModel : BaseViewModel
{
    // зависимости
    private readonly QuestionService _questionService;
    private readonly AchievementService _achievementService;
    private readonly AudioService _audioService;

    // состояние сессии
    private List<Question> _questions = new();
    private int _currentIndex = 0;
    private int _consecutiveCorrect = 0;
    private bool _answerLocked = false;

    // таймер 
    private IDispatcherTimer? _timer;
    private int _secondsLeft;

    // константы 
    private const int QuestionsPerRound = 10;
    private const int SecondsPerQuestion = 10;

    // привязываемые свойства

    private string _questionText = string.Empty;
    public string QuestionText
    {
        get => _questionText;
        set => SetField(ref _questionText, value);
    }

    private List<string> _options = new();
    public List<string> Options
    {
        get => _options;
        set => SetField(ref _options, value);
    }

    private int _score;
    public int Score
    {
        get => _score;
        set => SetField(ref _score, value);
    }

    private int _questionNumber;
    public int QuestionNumber
    {
        get => _questionNumber;
        set => SetField(ref _questionNumber, value);
    }

    private int _timerSeconds;
    public int TimerSeconds
    {
        get => _timerSeconds;
        set => SetField(ref _timerSeconds, value);
    }

    private string _category = string.Empty;
    public string Category
    {
        get => _category;
        set => SetField(ref _category, value);
    }

    private string _difficulty = string.Empty;
    public string Difficulty
    {
        get => _difficulty;
        set => SetField(ref _difficulty, value);
    }

    // флаги для View

    private int _crashType;
    public int CrashType
    {
        get => _crashType;
        set => SetField(ref _crashType, value);
    }

    private int _rewardType;
    public int RewardType
    {
        get => _rewardType;
        set => SetField(ref _rewardType, value);
    }

    private string _rewardText = string.Empty;
    public string RewardText
    {
        get => _rewardText;
        set => SetField(ref _rewardText, value);
    }

    private bool _isGameOver;
    public bool IsGameOver
    {
        get => _isGameOver;
        set => SetField(ref _isGameOver, value);
    }

    private GameResult _gameResult = new();
    public GameResult GameResult
    {
        get => _gameResult;
        set => SetField(ref _gameResult, value);
    }

    // события для View 
    public event Action<int>? OnCrashTriggered;
    public event Action<int, string>? OnRewardTriggered;
    public event Action<Achievement>? OnAchievementUnlocked;

    // команды
    public ICommand AnswerCommand { get; }

    // конструктор

    public QuizViewModel(
        QuestionService questionService,
        AchievementService achievementService,
        AudioService audioService)
    {
        _questionService = questionService;
        _achievementService = achievementService;
        _audioService = audioService;

        AnswerCommand = new Command<string>(OnAnswer);
    }

    //старт раунда

    public async Task InitializeAsync()
    {
        await _questionService.LoadAsync();
        StartRound();
    }

    private void StartRound()
    {
        _questions = _questionService.GetRandomQuestions(QuestionsPerRound);
        _currentIndex = 0;
        _consecutiveCorrect = 0;
        Score = 0;
        IsGameOver = false;

        ShowCurrentQuestion();
    }

    // отображение запроса

    private void ShowCurrentQuestion()
    {
        if (_currentIndex >= _questions.Count)
        {
            FinishGame();
            return;
        }

        _answerLocked = false;
        var q = _questions[_currentIndex];
        QuestionText = q.Text;
        Options = q.Options;
        Category = q.Category;
        Difficulty = q.Difficulty;
        QuestionNumber = _currentIndex + 1;

        StartTimer();
    }

    // таймер

    private void StartTimer()
    {
        StopTimer();
        _secondsLeft = SecondsPerQuestion;
        TimerSeconds = _secondsLeft;

        _timer = Application.Current!.Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += OnTimerTick;
        _timer.Start();
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        _secondsLeft--;
        TimerSeconds = _secondsLeft;

        if (_secondsLeft <= 0)
            OnAnswer(null);
    }

    private void StopTimer()
    {
        if (_timer is null) return;
        _timer.Stop();
        _timer.Tick -= OnTimerTick;
        _timer = null;
    }

    // обработка ответа

    private void OnAnswer(string? selectedOption)
    {
        if (_answerLocked) return;
        _answerLocked = true;
        StopTimer();

        var question = _questions[_currentIndex];
        bool isCorrect = selectedOption != null &&
                         question.Options.IndexOf(selectedOption) == question.CorrectIndex;

        if (isCorrect)
            HandleCorrectAnswer(question);
        else
            HandleWrongAnswer();
    }

    private void HandleCorrectAnswer(Question question)
    {
        Score += CalculateBonus();
        _consecutiveCorrect++;
        _achievementService.AddCorrect();

        _ = _audioService.PlayCorrectAsync();

        // проверка ачивки
        CheckAndUnlockAchievements();

        // выбор случайной награды
        var reward = PickReward(question);
        RewardType = reward.type;
        RewardText = reward.text;
        OnRewardTriggered?.Invoke(reward.type, reward.text);
    }

    private void HandleWrongAnswer()
    {
        _consecutiveCorrect = 0;
        VibrationService.LongCrash();
        _ = _audioService.PlayWrongAsync();

        var unlocked = _achievementService.Unlock("survived");
        if (unlocked)
        {
            var ach = new Achievement { Id = "survived", Title = "Выживший", Emoji = "💀" };
            OnAchievementUnlocked?.Invoke(ach);
        }

        // случайный краш от 1 до 5
        CrashType = Random.Shared.Next(1, 6);
        OnCrashTriggered?.Invoke(CrashType);
    }

    public void ContinueAfterOverlay()
    {
        _currentIndex++;
        CrashType = 0;
        RewardType = 0;
        ShowCurrentQuestion();
    }

    // конец игры

    private void FinishGame()
    {
        StopTimer();
        GameResult = new GameResult
        {
            TotalQuestions = _questions.Count,
            CorrectAnswers = _questions.Count(q =>
                _questions.IndexOf(q) < _currentIndex)
        };

        // счёт
        GameResult = new GameResult
        {
            TotalQuestions = QuestionsPerRound,
            CorrectAnswers = Score / 100
        };

        IsGameOver = true;
    }

    public void RestartGame() => StartRound();

    private int CalculateBonus()
    {
        return 100 + (_secondsLeft * 5);
    }

    private (int type, string text) PickReward(Question question)
    {
        int type = Random.Shared.Next(1, 6);

        var texts = new Dictionary<int, string[]>
        {
            [1] = new[] { "ДА ТЫ ПРОСТО БОСС",
                          "Последние нейроны пришли в действие",
                          "Шерлок Холмс нервно курит в углу" },
            [2] = new[] { "Настолько хорош!", "Аплодисменты!", "Красавчик!" },
            [3] = new[] { GetRandomFact() },
            [4] = new[] { GetRandomBadge() },
            [5] = new[] { $"ДИПЛОМ: «{GetDiplomaTitle(question.Difficulty)}»\nВручается за минимально проявленную мозговую активность!" }
        };

        var text = texts[type][Random.Shared.Next(texts[type].Length)];
        return (type, text);
    }

    private static string GetRandomFact()
    {
        string[] facts =
        {
            "ФАКТ: Осьминоги имеют три сердца и голубую кровь.",
            "ФАКТ: Банан — ягода, а клубника — нет.",
            "ФАКТ: Молния бьёт в одно место дважды. Часто.",
            "ФАКТ: Мёд не портится — нашли мёд в египетских гробницах и он был съедобен.",
            "ФАКТ: Кошки не чувствуют сладкого вкуса.",
        };
        return facts[Random.Shared.Next(facts.Length)];
    }

    private static string GetRandomBadge()
    {
        string[] badges = { "🏅 Мозг в ударе", "🏅 Эроист-террорист", "🏅 Да ты боженька!", "🏅 Выжившие будут завидовать мёртвым" };
        return badges[Random.Shared.Next(badges.Length)];
    }

    private static string GetDiplomaTitle(string difficulty)
    {
        return difficulty switch
        {
            "hard" => "Покоритель Сложного",
            "medium" => "Мастер Среднего",
            _ => "Искатель ответов"
        };
    }

    private void CheckAndUnlockAchievements()
    {
        // первый правильный
        if (_achievementService.GetTotalCorrect() == 1)
            TryUnlock("first_blood");

        // 3 подряд
        if (_consecutiveCorrect >= 3)
            TryUnlock("brain_on_fire");

        // 10 в сессии
        if (Score / 100 >= 10)
            TryUnlock("genius");

        // 50 суммарно
        if (_achievementService.GetTotalCorrect() >= 50)
            TryUnlock("diplomat");
    }

    private void TryUnlock(string id)
    {
        if (_achievementService.Unlock(id))
        {
            // нахождение ачивки для отображения
            var ach = new Achievement { Id = id };
            OnAchievementUnlocked?.Invoke(ach);
        }
    }
}