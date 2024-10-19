del /f /q /s discordbot-linux-x64 > nul
rmdir /q /s discordbot-linux-x64
dotnet publish --configuration Release --output discordbot-linux --framework net8.0 --self-contained true --runtime linux-x64
::copy News_TestBot\DiscordToken discordbot-linux-arm\
pause