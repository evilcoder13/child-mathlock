using System.Media;

namespace MathLock.Services;

public static class SoundService
{
    public static void PlayCorrect()
    {
        try
        {
            // Windows Asterisk sounds bright and rewarding
            SystemSounds.Asterisk.Play();
        }
        catch { }
    }

    public static void PlayIncorrect()
    {
        try
        {
            // Windows Hand or Beep
            SystemSounds.Hand.Play();
        }
        catch { }
    }

    public static void PlayVictory()
    {
        try
        {
            SystemSounds.Exclamation.Play();
        }
        catch { }
    }
}
