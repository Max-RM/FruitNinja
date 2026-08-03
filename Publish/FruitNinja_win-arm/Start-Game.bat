@echo off
cd /d "%~dp0"
if exist "%~dp0shared\Microsoft.WindowsDesktop.App" (
  set DOTNET_ROOT=%~dp0
  set DOTNET_MULTILEVEL_LOOKUP=0
)
if exist "%~dp0dotnet.exe" (
  "%~dp0dotnet.exe" FruitNinja.dll %*
) else (
  dotnet FruitNinja.dll %*
)
if errorlevel 1 pause
