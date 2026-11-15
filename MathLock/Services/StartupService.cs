using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace MathLock.Services;

public static class StartupService
{
    private const string RunRegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string AppName = "MathLock";

    public static bool IsStartupEnabled()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunRegistryKey, false);
            var value = key?.GetValue(AppName);
            return value != null;
        }
        catch
        {
            return false;
        }
    }

    public static bool SetStartup(bool enable, string? targetExePath = null)
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunRegistryKey, true);
            if (key == null) return false;

            if (enable)
            {
                string exePath = targetExePath ?? Environment.ProcessPath ?? Assembly.GetExecutingAssembly().Location;
                if (exePath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                {
                    exePath = Path.ChangeExtension(exePath, ".exe");
                }

                // Run in tray mode to handle SessionUnlock and 30-minute periodic triggers
                string cmd = $"\"{exePath}\" --tray";
                key.SetValue(AppName, cmd);
            }
            else
            {
                key.DeleteValue(AppName, false);
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
}
