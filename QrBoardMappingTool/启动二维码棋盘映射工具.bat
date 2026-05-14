@echo off
setlocal
chcp 65001 >nul
title QR Board Mapping Tool Launcher

set "ROOT=%~dp0"
set "APPDIR=%ROOT%bin\Release"
set "APP=%APPDIR%\QrBoardMappingTool.exe"
set "LOG=%ROOT%QrBoardMappingTool_run.log"

set "HALCONROOT=E:\halconPath"
set "HALCONARCH=x64-win64"
set "PATH=%HALCONROOT%\bin\x64-win64;%PATH%"

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

if not exist "%HALCONROOT%\bin\x64-win64\halcon.dll" (
    echo ERROR: HALCON dll not found:
    echo %HALCONROOT%\bin\x64-win64\halcon.dll
    echo ERROR: HALCON dll not found >> "%LOG%"
    pause
    exit /b 1
)

cd /d "%APPDIR%"
start "" "%APP%"

timeout /t 5 /nobreak >nul
tasklist /FI "IMAGENAME eq QrBoardMappingTool.exe" | find /I "QrBoardMappingTool.exe" >nul
if errorlevel 1 (
    echo.
    echo ERROR: program did not stay running.
    echo Log file:
    echo %LOG%
    echo ERROR: program did not stay running >> "%LOG%"
    pause
    exit /b 1
)

echo Started OK >> "%LOG%"
exit /b 0
