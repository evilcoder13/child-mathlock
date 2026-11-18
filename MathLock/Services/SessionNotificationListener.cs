using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace MathLock.Services;

public class SessionNotificationListener : IDisposable
{
    private const int NOTIFY_FOR_THIS_SESSION = 0;
    private const int WM_WTSSESSION_CHANGE = 0x02B1;

    public const int WTS_CONSOLE_CONNECT = 0x1;
    public const int WTS_CONSOLE_DISCONNECT = 0x2;
    public const int WTS_REMOTE_CONNECT = 0x3;
    public const int WTS_REMOTE_DISCONNECT = 0x4;
    public const int WTS_SESSION_LOGON = 0x5;
    public const int WTS_SESSION_LOGOFF = 0x6;
    public const int WTS_SESSION_LOCK = 0x7;
    public const int WTS_SESSION_UNLOCK = 0x8;
    public const int WTS_SESSION_REMOTE_CONTROL = 0x9;

    [DllImport("wtsapi32.dll", SetLastError = true)]
    private static extern bool WTSRegisterSessionNotification(IntPtr hWnd, int dwFlags);

    [DllImport("wtsapi32.dll", SetLastError = true)]
    private static extern bool WTSUnRegisterSessionNotification(IntPtr hWnd);

    private readonly HwndSource _hwndSource;
    public event Action<int>? SessionChanged;

    public SessionNotificationListener()
    {
        var parameters = new HwndSourceParameters("MathLock_SessionListener")
        {
            WindowStyle = 0,
            ExtendedWindowStyle = 0,
            Width = 0,
            Height = 0
        };

        _hwndSource = new HwndSource(parameters);
        _hwndSource.AddHook(WndProc);

        bool registered = WTSRegisterSessionNotification(_hwndSource.Handle, NOTIFY_FOR_THIS_SESSION);
        Logger.Log($"WTSRegisterSessionNotification result: {registered}, Handle: {_hwndSource.Handle}");
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WM_WTSSESSION_CHANGE)
        {
            int reason = wParam.ToInt32();
            Logger.Log($"WM_WTSSESSION_CHANGE received! Reason code: 0x{reason:X} ({GetReasonName(reason)})");
            SessionChanged?.Invoke(reason);
            handled = true;
        }

        return IntPtr.Zero;
    }

    private static string GetReasonName(int reason) => reason switch
    {
        WTS_SESSION_LOCK => "WTS_SESSION_LOCK",
        WTS_SESSION_UNLOCK => "WTS_SESSION_UNLOCK",
        WTS_SESSION_LOGON => "WTS_SESSION_LOGON",
        WTS_SESSION_LOGOFF => "WTS_SESSION_LOGOFF",
        _ => $"Reason_{reason}"
    };

    public void Dispose()
    {
        try
        {
            if (_hwndSource.Handle != IntPtr.Zero)
            {
                WTSUnRegisterSessionNotification(_hwndSource.Handle);
            }
            _hwndSource.Dispose();
        }
        catch { }
    }
}
