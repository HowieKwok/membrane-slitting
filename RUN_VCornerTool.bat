@echo off
setlocal
chcp 65001 >nul
title VCorner WPF Tool Launcher

set "ROOT=%~dp0"
set "APPDIR=%ROOT%VCornerWpfTool"
set "APP=%APPDIR%\ConvexPeakWpfDemo.exe"
set "LOG=%ROOT%VCornerWpfTool_run.log"

set "HALCONROOT=E:\halconPath"
set "HALCONARCH=x64-win64"
set "PATH=E:\halconPath\bin\x64-win64;%PATH%"

for /f "usebackq tokens=*" %%P in (`powershell -NoProfile -ExecutionPolicy Bypass -Command "$p=Get-Process ConvexPeakWpfDemo -ErrorAction SilentlyContinue | Select-Object -First 1; if($p){Add-Type 'using System; using System.Runtime.InteropServices; public static class W{[DllImport(\"user32.dll\")] public static extern bool ShowWindowAsync(IntPtr hWnd,int nCmdShow); [DllImport(\"user32.dll\")] public static extern bool SetForegroundWindow(IntPtr hWnd);}'; [W]::ShowWindowAsync($p.MainWindowHandle,9) | Out-Null; [W]::SetForegroundWindow($p.MainWindowHandle) | Out-Null; Write-Output $p.Id}"`) do set "RUNNING_PID=%%P"
if defined RUNNING_PID (
    echo Already running, activated PID=%RUNNING_PID%
    exit /b 0
)

echo [%date% %time%] Start > "%LOG%"
echo APP=%APP% >> "%LOG%"
echo HALCONROOT=%HALCONROOT% >> "%LOG%"
echo HALCONARCH=%HALCONARCH% >> "%LOG%"

if not exist "%APP%" (
    echo ERROR: app not found:
    echo %APP%
    echo ERROR: app not found: %APP% >> "%LOG%"
    pause
    exit /b 1
)

if not exist "E:\halconPath\bin\x64-win64\halcon.dll" (
    echo ERROR: HALCON dll not found:
    echo E:\halconPath\bin\x64-win64\halcon.dll
    echo ERROR: HALCON dll not found >> "%LOG%"
    pause
    exit /b 1
)

cd /d "%APPDIR%"
echo Starting VCorner WPF Tool...
echo Starting VCorner WPF Tool... >> "%LOG%"
start "" "%APP%"

timeout /t 2 /nobreak >nul
tasklist /FI "IMAGENAME eq ConvexPeakWpfDemo.exe" | find /I "ConvexPeakWpfDemo.exe" >nul
if errorlevel 1 (
    echo.
    echo ERROR: program did not stay running.
    echo Log file:
    echo %LOG%
    pause
    exit /b 1
)

echo Started OK >> "%LOG%"
exit /b 0
