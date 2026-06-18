namespace UP.Services;

// обертка над вибрацией
public static class VibrationService
{
    public static void ShortBuzz() => TryVibrate(200);
    public static void LongCrash() => TryVibrate(700);

    private static void TryVibrate(int ms)
    {
        try { Vibration.Default.Vibrate(ms); }
        catch {  }
    }
}