del /f /q /s discordbot-linux-arm > nul
rmdir /q /s discordbot-linux-arm
dotnet publish --configuration Release --output discordbot-linux-arm --framework net8.0 --self-contained true --runtime linux-arm64
::copy News_TestBot\DiscordToken discordbot-linux-arm\
pause