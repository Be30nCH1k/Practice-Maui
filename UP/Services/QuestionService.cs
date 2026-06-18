using UP.Models;
using System.Text.Json;

namespace UP.Services;

/* будет загружать вопросы из questions.json
перемешивает и раздаёт по одному */
public class QuestionService
{
    private List<Question> _allQuestions = new();
    private readonly Random _rng = new();

    // загрузить вопросы из бандла
    public async Task LoadAsync()
    {
        if (_allQuestions.Count > 0) return; // уже загружены

        await using var stream = await FileSystem.OpenAppPackageFileAsync("questions.json");
        using var reader = new StreamReader(stream);
        var json = await reader.ReadToEndAsync();

        _allQuestions = JsonSerializer.Deserialize<List<Question>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? new List<Question>();
    }

    // вернуть N случайных вопросов без повторов
    public List<Question> GetRandomQuestions(int count)
    {
        if (_allQuestions.Count == 0)
            throw new InvalidOperationException("Вопросы не загружены. Сначала вызовите LoadAsync().");

        return _allQuestions
            .OrderBy(_ => _rng.Next())
            .Take(Math.Min(count, _allQuestions.Count))
            .ToList();
    }
}