del /f /q /s discordbot-linux-arm > nul
rmdir /q /s discordbot-linux-arm
dotnet publish --configuration Release --output discordbot-linux-arm --framework netcoreapp3.1 --self-contained true --runtime linux-arm
::copy News_TestBot\DiscordToken discordbot-linux-arm\
pause