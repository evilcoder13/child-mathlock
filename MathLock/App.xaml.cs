using System.Threading;
using System.Windows;
using Microsoft.Win32;
using MathLock.Models;
using MathLock.Services;
using MathLock.Views;

namespace MathLock;

public partial class App : System.Windows.Application
{
    private static Mutex? _singleInstanceMutex;
    private AppSettings? _settings;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Load configuration
        _settings = SettingsService.Load();

        // Parse CLI arguments
        // --settings : opens parent settings
        // --tray     : runs in background and listens to SessionSwitch (SessionLock/SessionUnlock)
        // default / --lock : displays full lock screen

        bool showSettings = e.Args.Contains("--settings", StringComparer.OrdinalIgnoreCase);
        bool runTray = e.Args.Contains("--tray", StringComparer.OrdinalIgnoreCase);

        if (showSettings)
        {
            var settingsWin = new SettingsWindow(_settings);
            settingsWin.Show();
            return;
        }

        if (runTray)
        {
            StartBackgroundListener();
            return;
        }

        // Lock mode: Ensure single active lock instance
        const string mutexName = "Global\\MathLock_Running_Instance";
        _singleInstanceMutex = new Mutex(true, mutexName, out bool createdNew);

        if (!createdNew)
        {
            // Another lock instance is already active
            Shutdown();
            return;
        }

        var lockWin = new LockWindow(_settings);
        lockWin.Show();
    }

    private void StartBackgroundListener()
    {
        // Listen to Windows Session Switch (Logon / Unlock / Lock)
        SystemEvents.SessionSwitch += (s, ev) =>
        {
            if (ev.Reason == SessionSwitchReason.SessionUnlock ||
                ev.Reason == SessionSwitchReason.SessionLogon)
            {
                Dispatcher.Invoke(() =>
                {
                    _settings = SettingsService.Load();
                    var lockWin = new LockWindow(_settings);
                    lockWin.Show();
                });
            }
        };

        // Keep app alive without opening visible window
        ShutdownMode = ShutdownMode.OnExplicitShutdown;
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _singleInstanceMutex?.Dispose();
        base.OnExit(e);
    }
}
