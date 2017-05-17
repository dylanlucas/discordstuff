using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace ModerationBot
{    
    class ModerationBot
    {
        DiscordClient client;
        CommandService commands;

        public ModerationBot()
        {
            client = new DiscordClient(input =>
            {
                input.LogLevel = LogSeverity.Info;
                input.LogHandler = Log;
            });

            client.UsingCommands(input =>
            {
                input.PrefixChar = '!';
                input.AllowMentionPrefix = true;
            });

            commands = client.GetService<CommandService>();

            commands.CreateCommand("warn").Parameter("@name", ParameterType.Multiple).Do(async (e) =>
            {
                await WarnUser(e);
            });

            commands.CreateCommand("kick").Parameter("@name", ParameterType.Multiple).Do(async (e) =>
            {
                await KickUser(e);
            });

            client.ExecuteAndWait(async () =>
            {
                await client.Connect("MzEzODkwOTA0MzA4MDU2MDY2.C_wO1Q.nOZGWG-i1prqZrgyrkDrC342IWc", TokenType.Bot);
            });

        }

        private async Task KickUser(CommandEventArgs e)
        {
            var userRoles = e.User.Roles;

            var username = e.Args[0];

            var user = e.Server.FindUsers(username).FirstOrDefault();

            try
            {
                if (userRoles.Any(input => input.Name.ToUpper() == "MODERATOR"))
                {
                    await user.Kick();
                    await e.Channel.SendMessage(string.Format("{0} has been kicked from the server.", user.Mention));
                }
            }
            catch (Exception ex)
            {
                await e.Channel.SendMessage(ex.Message);
            }
        }

        private async Task WarnUser(CommandEventArgs e)
        {
            var userRoles = e.User.Roles;

            if (userRoles.Any(input => input.Name.ToUpper() == "MODERATOR"))
            {
                var channel = e.Server.FindChannels(e.Args[0], ChannelType.Text).FirstOrDefault();

                var message = ConstructMessage(e, channel != null);

                if (channel != null)
                {
                    await channel.SendMessage(message);
                }
                else
                {
                    await e.Channel.SendMessage(message);
                }
            }
        }

        private string ConstructMessage(CommandEventArgs e, bool firstArgIsChannel)
        {
            string message = "";

            var name = e.User.Nickname != null ? e.User.Nickname : e.User.Name;

            var startIndex = firstArgIsChannel ? 1 : 0;

            for (int i = startIndex; i < e.Args.Length; i++)
            {
                message += e.Args[i].ToString() + " ";
            }

            var result = message;

            return result;
        }

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
