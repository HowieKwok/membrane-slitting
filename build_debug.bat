@echo off
rem ---------------------------------------------------------------------------
rem  Debug build entry for Mef_VisionFrame.sln
rem  - locates the installed Visual Studio via vswhere (VS2022 / VS2026 both OK)
rem  - builds Debug|Any CPU with the x64 toolchain
rem  - writes a UTF-8 build log next to this script
rem  Usage: build_debug.bat            (build)
rem         build_debug.bat rebuild    (clean + build)
rem  Exit code: 0 = success
rem ---------------------------------------------------------------------------
setlocal
cd /d "%~dp0"
chcp 65001 >nul

set "TARGET=Build"
if /i "%~1"=="rebuild" set "TARGET=Rebuild"

set "VSWHERE=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
set "VSCMD="
if exist "%VSWHERE%" (
  for /f "usebackq delims=" %%i in (`"%VSWHERE%" -latest -products * -requires Microsoft.Component.MSBuild -find Common7\Tools\VsDevCmd.bat`) do set "VSCMD=%%i"
)

if defined VSCMD (
  call "%VSCMD%" -arch=x64 -host_arch=x64 -no_logo
) else (
  echo [warn] VsDevCmd.bat not found, falling back to msbuild from PATH
)

echo === msbuild /t:%TARGET% Configuration=Debug ===
msbuild "Mef_VisionFrame.sln" /t:%TARGET% /p:Configuration=Debug /p:Platform="Any CPU" /v:m /nologo "/flp:logfile=build_debug.log;encoding=utf-8"
set "RC=%ERRORLEVEL%"
echo === MSBuild exit code = %RC% ===
endlocal & exit /b %RC%
