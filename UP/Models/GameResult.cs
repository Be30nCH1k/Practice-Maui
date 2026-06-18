namespace QuizOrDie.Models;

/// результат одной игровой сессии (передаётся между ViewModel и View).

public class GameResult
{
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public int Score => CorrectAnswers * 100;
    public bool IsVictory => CorrectAnswers == TotalQuestions;
}