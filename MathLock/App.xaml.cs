using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using MathLock.Models;
using MathLock.Services;
using MathLock.Views;

namespace MathLock;

public partial class App : System.Windows.Application
{
    private static Mutex? _singleInstanceMutex;
    private AppSettings? _settings;
    private DispatcherTimer? _periodicLockTimer;
    private LockWindow? _activeLockWindow;
    private SessionNotificationListener? _wtsListener;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        AppDomain.CurrentDomain.UnhandledException += (s, ev) =>
        {
            Logger.Log($"UnhandledException: {ev.ExceptionObject}");
        };
        DispatcherUnhandledException += (s, ev) =>
        {
            Logger.Log($"DispatcherUnhandledException: {ev.Exception}");
        };

        // Load configuration
        Logger.Log($"OnStartup args: {string.Join(" ", e.Args)}");
        _settings = SettingsService.Load();

        // Parse CLI arguments
        // --settings : opens parent settings
        // --tray     : runs in background and listens to SessionSwitch & periodic timer
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
            // Ensure single tray listener instance
            const string trayMutexName = "Global\\MathLock_Tray_Instance";
            _singleInstanceMutex = new Mutex(true, trayMutexName, out bool createdTray);
            if (!createdTray)
            {
                Shutdown();
                return;
            }

            StartBackgroundListener();
            return;
        }

        // Direct lock mode
        const string lockMutexName = "Global\\MathLock_Running_Instance";
        _singleInstanceMutex = new Mutex(true, lockMutexName, out bool createdLock);

        if (!createdLock)
        {
            // Another lock instance is already active
            Shutdown();
            return;
        }

        var lockWin = new LockWindow(_settings, closeOnlyOnUnlock: false);
        lockWin.Show();
    }

    private void StartBackgroundListener()
    {
        Logger.Log("StartBackgroundListener entered");
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        // Automatically trigger lock on startup/login
        TriggerLock();

        // Register direct Win32 WTS session notification for robust Unlock/Logon detection
        try
        {
            _wtsListener = new SessionNotificationListener();
            _wtsListener.SessionChanged += (reason) =>
            {
                if (reason == SessionNotificationListener.WTS_SESSION_UNLOCK ||
                    reason == SessionNotificationListener.WTS_SESSION_LOGON)
                {
                    Dispatcher.Invoke(TriggerLock);
                }
            };
        }
        catch (Exception ex)
        {
            Logger.Log($"Error creating SessionNotificationListener: {ex}");
        }

        // Also listen via Microsoft.Win32.SystemEvents as fallback
        SystemEvents.SessionSwitch += (s, ev) =>
        {
            Logger.Log($"SessionSwitch event received: {ev.Reason}");
            if (ev.Reason == SessionSwitchReason.SessionUnlock ||
                ev.Reason == SessionSwitchReason.SessionLogon)
            {
                Dispatcher.Invoke(TriggerLock);
            }
        };

        // Setup periodic timer (default: every 30 minutes)
        SetupPeriodicTimer();
        Logger.Log("StartBackgroundListener finished setup");
    }

    public void SetupPeriodicTimer()
    {
        _periodicLockTimer?.Stop();

        if (_settings != null && _settings.EnablePeriodicLock && _settings.PeriodicIntervalMinutes > 0)
        {
            _periodicLockTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(_settings.PeriodicIntervalMinutes)
            };
            _periodicLockTimer.Tick += (s, e) =>
            {
                TriggerLock();
            };
            _periodicLockTimer.Start();
        }
    }

    private void TriggerLock()
    {
        if (_activeLockWindow != null && _activeLockWindow.IsVisible)
        {
            _activeLockWindow.Activate();
            return;
        }

        _settings = SettingsService.Load();
        _activeLockWindow = new LockWindow(_settings, closeOnlyOnUnlock: true);
        _activeLockWindow.Closed += (s, e) =>
        {
            _activeLockWindow = null;
            // Reset periodic timer so child gets full interval after completing math
            SetupPeriodicTimer();
        };
        _activeLockWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Logger.Log($"OnExit code: {e.ApplicationExitCode}");
        _periodicLockTimer?.Stop();
        _wtsListener?.Dispose();
        _singleInstanceMutex?.Dispose();
        base.OnExit(e);
    }
}
