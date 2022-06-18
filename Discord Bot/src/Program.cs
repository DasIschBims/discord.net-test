using System;
using System.Text;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord_Bot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Program
{
    public static Task Main(string[] args)
    {
        return new Program().MainAsync();
    }

    private DiscordSocketClient? _client;

    public async Task MainAsync()
    {
        var json = string.Empty;
        using (var fs = File.OpenRead("../../../src/config.json"))
        using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
            json = await sr.ReadToEndAsync().ConfigureAwait(false);

        var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

        _client = new DiscordSocketClient();

        // Login and connect.
        await _client.LoginAsync(TokenType.Bot, configJson.Token);
        await _client.SetGameAsync("C# Code", null, ActivityType.Watching);
        await _client.StartAsync();
        Console.WriteLine($"Bot successfully logged in with Id {configJson.BotId}");

        // Wait infinitely so your bot actually stays connected.
        await Task.Delay(Timeout.Infinite);

    }

    public class LoggingService 
    {
        public LoggingService(DiscordSocketClient client, CommandService command)
        {
            client.Log += LogAsync;
            command.Log += LogAsync;
        }
        private Task LogAsync(LogMessage message)
        {
            if (message.Exception is CommandException cmdException)
            {
                Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases.First()}"
                    + $" failed to execute in {cmdException.Context.Channel}.");
                Console.WriteLine(cmdException);
            }
            else
                Console.WriteLine($"[General/{message.Severity}] {message}");

            return Task.CompletedTask;
        }
    }
}