using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Verloren_Companion_Bot
{
    class Program
    {
        public static void Main(string[] args)
        => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public async Task RunBotAsync() //Runs the bot in Async
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands) //These make sure no duplicates of _client and _commands run at the same time
                .AddSingleton<LycanEvents>()
                .AddSingleton<DreamEvents>()
                .BuildServiceProvider();

            String token = "Njg2NjM4ODAzOTU1NjEzNzQw.Xmaafw.yZ5LCeZQG4HTcTuxeWDtRxExyCg";

            await _client.SetGameAsync("\"V-help\" for help!");

            _client.Log += botLog; //Starts up client's log

            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, token); //Logs into the bot

            await _client.StartAsync(); //Starts up the bot

            await Task.Delay(-1); //Keeps the bot running even when nothing is occurring
        }

        private Task botLog(LogMessage arg) //Client's log and history of events
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync() //Registers the commands as an async (see comment below for the assembly)
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services); //Adds an assembly of the commands as an async module
        }

        private async Task HandleCommandAsync(SocketMessage arg) //Command handler, duh
        {
            var message = arg as SocketUserMessage; //Tells bot what to do on message found
            var context = new SocketCommandContext(_client, message);
            if (message.Author.IsBot) return; //Makes sure bot doesn't answer to other bots

            int argPos = 0;
            if (message.HasStringPrefix("V-", ref argPos)) //Set prefix
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
            }
        }
    }
}
