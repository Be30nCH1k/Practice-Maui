namespace QuizOrDie.Models;

/// Один вопрос викторины. берётся из questions.json.

public class Question
{
    public string Text { get; set; } = string.Empty;

    /// 4 варианта ответа
    public List<string> Options { get; set; } = new();

    /// индекс правильного ответа в списке
    public int CorrectIndex { get; set; }

    /// категория для отображения и фильтрации
    public string Category { get; set; } = "Общие знания";

    /// easy/medium/hard
    public string Difficulty { get; set; } = "medium";
}