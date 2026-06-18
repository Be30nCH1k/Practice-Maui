using UP.Models;
using UP.Services;

namespace UP.ViewModels;

public class AchievementsViewModel : BaseViewModel
{
    private readonly AchievementService _service;

    private List<Achievement> _achievements = new();
    public List<Achievement> Achievements
    {
        get => _achievements;
        set => SetField(ref _achievements, value);
    }

    public int UnlockedCount => Achievements.Count(a => a.IsUnlocked);
    public int TotalCount => Achievements.Count;

    public AchievementsViewModel(AchievementService service)
    {
        _service = service;
        Refresh();
    }

    public void Refresh()
    {
        Achievements = _service.GetAll();
        OnPropertyChanged(nameof(UnlockedCount));
        OnPropertyChanged(nameof(TotalCount));
    }
}