using System.Diagnostics;
using System.IO;

namespace MathLock.Services;

public static class TaskSchedulerService
{
    private const string TaskName = "MathLock_AutoLock";

    public static bool RegisterTriggers(string exePath)
    {
        try
        {
            // Register Windows task on Logon and on Workstation Unlock using schtasks
            // 1. Task on Logon
            string logonCmd = $"schtasks /create /tn \"{TaskName}_Logon\" /tr \"\\\"{exePath}\\\" --lock\" /sc onlogon /rl highest /f";
            RunProcess("cmd.exe", $"/c {logonCmd}");

            // 2. Task on Event (Workstation Unlock is EventID 4801 in Security log or session unlock trigger)
            // Windows Task Scheduler natively supports SessionStateChange / Unlock via XML or PowerShell
            string psScript = $@"
$action = New-ScheduledTaskAction -Execute '{exePath}' -Argument '--lock'
$trigger1 = Get-CimClass -ClassName MSFT_TaskSessionStateChangeTrigger -Namespace Root/Microsoft/Windows/TaskScheduler
$triggerUnlock = $trigger1 | New-CimInstance -ClientOnly
$triggerUnlock.StateChange = 8 # 8 = TASK_SESSION_UNLOCK
$principal = New-ScheduledTaskPrincipal -UserId $env:USERNAME -RunLevel Highest
Register-ScheduledTask -TaskName '{TaskName}_Unlock' -Action $action -Trigger $triggerUnlock -Principal $principal -Force
";
            RunProcess("powershell.exe", $"-NoProfile -ExecutionPolicy Bypass -Command \"{psScript}\"");
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool UnregisterTriggers()
    {
        try
        {
            RunProcess("schtasks", $"/delete /tn \"{TaskName}_Logon\" /f");
            RunProcess("schtasks", $"/delete /tn \"{TaskName}_Unlock\" /f");
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static void RunProcess(string filename, string args)
    {
        var psi = new ProcessStartInfo
        {
            FileName = filename,
            Arguments = args,
            CreateNoWindow = true,
            UseShellExecute = false
        };
        using var proc = Process.Start(psi);
        proc?.WaitForExit(5000);
    }
}
