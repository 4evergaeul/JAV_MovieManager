@echo off
setlocal

:: Check if running as administrator
openfiles >nul 2>nul
if %errorlevel% neq 0 (
    echo Requesting administrative privileges...
    powershell -Command "Start-Process '%0' -Verb runAs"
    exit /b
)

echo Step 1: Installing .NET Core 3.1 SDK...
start /wait "" "%~dp0dotnet-sdk-3.1.426-win-x64.exe" /install /quiet /norestart
if %errorlevel% neq 0 (
    echo .NET Core 3.1 SDK installation failed.
    pause
    exit /b %errorlevel%
)
echo .NET Core 3.1 SDK installed successfully.

echo Step 2: Installing Node.js...
start /wait msiexec /i "%~dp0node-v20.15.0-x64.msi" /quiet /norestart
if %errorlevel% neq 0 (
    echo Node.js installation failed.
    pause
    exit /b %errorlevel%
)
echo Node.js installed successfully.

echo Step 3: Installing PotPlayer...
start /wait "%~dp0PotPlayerSetup64.exe" /s
if %errorlevel% neq 0 (
    echo PotPlayer installation failed.
    pause
    exit /b %errorlevel%
)
echo PotPlayer installed successfully.

echo Step 4: Installing serve and http-server globally...
npm install --global serve
if %errorlevel% neq 0 (
    echo npm install serve failed.
    pause
    exit /b %errorlevel%
)
npm install --global http-server
if %errorlevel% neq 0 (
    echo npm install http-server failed.
    pause
    exit /b %errorlevel%
)
echo Node packages installed successfully.

echo Installation Completed.
pause
endlocal
