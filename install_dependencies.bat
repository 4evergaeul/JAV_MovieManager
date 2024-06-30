@echo off
echo Installing .NET Core SDK...
start /wait "" "https://download.visualstudio.microsoft.com/download/pr/b70ad520-0e60-43f5-aee2-d3965094a40d/667c122b3736dcbfa1beff08092dbfc3/dotnet-sdk-3.1.426-win-x64.exe" /quiet /norestart

echo Installing Node.js...
start /wait "" "https://nodejs.org/dist/v18.16.0/node-v18.16.0-x64.msi" /quiet /norestart

echo Installing Potplayer...
start /wait "" "https://t1.daumcdn.net/potplayer/PotPlayer/Version/Latest/PotPlayerSetup64.exe" /quiet /norestart

echo Installing global npm packages...
npm install --global serve
npm install --global http-server

echo All dependencies are installed.
pause