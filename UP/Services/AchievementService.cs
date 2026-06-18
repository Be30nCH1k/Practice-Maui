using UP.Models;

namespace UP.Services;

// хранит и выдаёт ачивки
public class AchievementService
{
    // список всех возможных ачивок
    private static readonly List<Achievement> Catalog = new()
    {
        new() { Id = "first_blood",    Title = "Первая кровь",      Emoji = "🩸", Description = "Ответь правильно впервые" },
        new() { Id = "brain_on_fire",  Title = "Мозг в ударе",      Emoji = "🧠", Description = "3 правильных подряд" },
        new() { Id = "survived",       Title = "Выживший",           Emoji = "💀", Description = "Получи фейк-краш и продолжи" },
        new() { Id = "erudite",        Title = "Эрудит-террорист",   Emoji = "💣", Description = "Пройди раунд без ошибок" },
        new() { Id = "diplomat",       Title = "Дипломат",           Emoji = "🎖️", Description = "Ответь на 50 вопросов суммарно" },
        new() { Id = "genius",         Title = "Да ты боженька",     Emoji = "⚡", Description = "10 правильных в одной сессии" },
    };

    // публичные методы

    public List<Achievement> GetAll()
    {
        return Catalog.Select(a =>
        {
            a.IsUnlocked = Preferences.Get($"ach_{a.Id}", false);
            return a;
        }).ToList();
    }

    // разблокировать ачивку по Id
    public bool Unlock(string achievementId)
    {
        var key = $"ach_{achievementId}";
        if (Preferences.Get(key, false)) return false; // уже была

        Preferences.Set(key, true);
        return true;
    }

    public void ResetAll()
    {
        foreach (var a in Catalog)
            Preferences.Remove($"ach_{a.Id}");
    }

    //статистика для проверки условий 

    public int GetTotalCorrect() => Preferences.Get("stat_total_correct", 0);
    public void AddCorrect(int n = 1) => Preferences.Set("stat_total_correct", GetTotalCorrect() + n);
}