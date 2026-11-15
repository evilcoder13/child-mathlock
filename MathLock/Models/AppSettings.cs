using System.Security.Cryptography;
using System.Text;

namespace MathLock.Models;

public enum DifficultyLevel
{
    Grade1_2, // + and - within 20, visual bonds
    Grade2_3, // +, - within 100, x, ÷ within 10x10, missing numbers, comparison
    Grade4_5  // Multi-digit +, -, x, ÷, two-step operations
}

public class AppSettings
{
    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Grade2_3;
    public int RequiredQuestions { get; set; } = 10;
    public int SecondsPerQuestion { get; set; } = 12; // Anti-calculator countdown
    public string ParentPinSha256 { get; set; } = ComputeSha256("1234"); // Default PIN: 1234
    public bool PlaySounds { get; set; } = true;
    public bool AutoStartWithWindows { get; set; } = true;

    public int PeriodicIntervalMinutes { get; set; } = 30; // Auto-lock every 30 minutes
    public bool EnablePeriodicLock { get; set; } = true;

    public static string ComputeSha256(string raw)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public bool VerifyPin(string enteredPin)
    {
        return string.Equals(ParentPinSha256, ComputeSha256(enteredPin), StringComparison.OrdinalIgnoreCase);
    }
}
