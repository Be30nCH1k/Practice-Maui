using QuizOrDie.Models;
using QuizOrDie.Services;
using UP.Models;
using UP.ViewModels;

namespace QuizOrDie.ViewModels;

/* модель представления для экрана достижений
управляет данными о полученных и заблокированных наградах пользователя */

public class AchievementsViewModel : BaseViewModel
{
    private readonly AchievementService _service;

    // закрытое поле для хранения списка достижений
    private List<Achievement> _achievements = new();


    /* список всех достижений для отображения в интерфейсе.
    при обновлении списка автоматически уведомляет View */

    public List<Achievement> Achievements
    {
        get => _achievements;
        set => SetField(ref _achievements, value);
    }

    // количество разблокированных достижений
    public int UnlockedCount => Achievements.Count(a => a.IsUnlocked);

    // общее количество существующих достижений в игре
    public int TotalCount => Achievements.Count;

    // конструктор класса инициализирует сервис и загружает первичные данные
    public AchievementsViewModel(AchievementService service)
    {
        _service = service;
        Refresh();
    }

    /* обновляет список достижений из сервиса
    и принудительно уведомляет интерфейс об изменении счетчиков */
    public void Refresh()
    {
        Achievements = _service.GetAll();

        OnPropertyChanged(nameof(UnlockedCount));
        OnPropertyChanged(nameof(TotalCount));
    }
}
