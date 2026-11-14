# MathLock Auto-Lock Task Installer
# Run in Administrator PowerShell to register Logon and Session Unlock triggers

param (
    [switch]$Uninstall
)

$TaskNameLogon = "MathLock_AutoLock_Logon"
$TaskNameUnlock = "MathLock_AutoLock_Unlock"

if ($Uninstall) {
    Write-Host "Unregistering MathLock scheduled tasks..." -ForegroundColor Yellow
    schtasks /delete /tn $TaskNameLogon /f 2>$null
    schtasks /delete /tn $TaskNameUnlock /f 2>$null
    Write-Host "MathLock tasks removed successfully!" -ForegroundColor Green
    exit
}

$ExePath = Join-Path $PSScriptRoot "bin\Release\net8.0-windows\win-x64\publish\MathLock.exe"
if (-not (Test-Path $ExePath)) {
    $ExePath = Join-Path $PSScriptRoot "bin\Debug\net8.0-windows\MathLock.exe"
}

if (-not (Test-Path $ExePath)) {
    Write-Error "Could not find MathLock.exe at $ExePath. Please build the project first: dotnet publish -c Release -r win-x64 --self-contained false"
    exit 1
}

Write-Host "Registering MathLock for executable: $ExePath" -ForegroundColor Cyan

# 1. Register Logon Task
$cmdLogon = "schtasks /create /tn `"$TaskNameLogon`" /tr `"\`"$ExePath\`" --lock`" /sc onlogon /rl highest /f"
Invoke-Expression $cmdLogon

# 2. Register Workstation Unlock Task (CIM session trigger)
try {
    $action = New-ScheduledTaskAction -Execute $ExePath -Argument "--lock"
    $triggerClass = Get-CimClass -ClassName MSFT_TaskSessionStateChangeTrigger -Namespace Root/Microsoft/Windows/TaskScheduler
    $triggerUnlock = $triggerClass | New-CimInstance -ClientOnly
    $triggerUnlock.StateChange = 8 # TASK_SESSION_UNLOCK
    $principal = New-ScheduledTaskPrincipal -UserId $env:USERNAME -RunLevel Highest

    Register-ScheduledTask -TaskName $TaskNameUnlock -Action $action -Trigger $triggerUnlock -Principal $principal -Force | Out-Null
    Write-Host "Successfully registered Workstation Unlock trigger ($TaskNameUnlock)!" -ForegroundColor Green
} catch {
    Write-Warning "Could not register CIM Unlock trigger automatically. You can also run MathLock with '--tray' in the background to handle SessionUnlock."
}

Write-Host "MathLock setup complete! Every time the user logs on or unlocks the PC, the 10 quick maths challenge will activate." -ForegroundColor Green
