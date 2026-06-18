namespace QuizOrDie.Services;

// на платформах без звука молча пропускает
public class AudioService
{

    public Task PlayCorrectAsync() => PlayEmbeddedAsync("correct.mp3");
    public Task PlayWrongAsync() => PlayEmbeddedAsync("wrong.mp3");

    public Task PlayApplauseAsync() => PlayEmbeddedAsync("applause.mp3");

    private async Task PlayEmbeddedAsync(string fileName)
    {
        try
        {
            await Task.CompletedTask; // заглушка нужно заменить на реальный вызов
        }
        catch
        {
        }
    }
}