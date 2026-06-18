namespace UP.Models;

// получение ачивки

public class Achievement
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Emoji { get; set; } = "🏆";
    public bool IsUnlocked { get; set; } = false;
    public DateTime? UnlockedAt { get; set; }
}