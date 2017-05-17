using System;
using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using TestingBot.Services;

namespace TestingBot
{
    class DiscordBot
    {
        DiscordClient client;
        CommandService commands;

        public DiscordBot()
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

            commands.CreateCommand("test").Do(async (e) =>
            {
                await e.Channel.SendMessage("Congratulations, you did it!");
            });

            commands.CreateCommand("announce").Parameter("message", ParameterType.Multiple).Do(async (e) =>
            {
                await DoAnnouncement(e);
            });

            commands.CreateCommand("award").Parameter("name", ParameterType.Multiple).Do(async (e) =>
            {
                await AwardUserTokens(e);
            });

            commands.CreateCommand("hello").Parameter("@name", ParameterType.Multiple).Do(async (e) =>
            {
                await ComplimentUser(e);
            });

            commands.CreateCommand("delete").Parameter("messages", ParameterType.Multiple).Do(async (e) =>
            {
                await DeleteMessage(e);
            });

            commands.CreateCommand("purge").Do(async (e) =>
            {                
                Message[] messagesToDelete;
                messagesToDelete = await e.Channel.DownloadMessages(100);

                await e.Channel.DeleteMessages(messagesToDelete);
            });

            commands.CreateCommand("spam").Do(async (e) =>
            {
                //string channelName = "private";

                //var server = client.Servers.FirstOrDefault();
                //Channel channelToSpam = server.AllChannels.FirstOrDefault(channel => channel.Name.Equals(channelName));
                //foreach (var user in e.Channel.Users)
                //{
                //    await e.Channel.SendMessage(string.Format("{0}: {1}", user.Mention, user.Status.Value));
                //}

                foreach (var user in e.Channel.Users)
                {
                    await e.Channel.SendMessage(string.Format("Hey {0}, how you doin sexy ;)", user.Mention));

                    System.Threading.Thread.Sleep(5000);
                }

                //string channelName = "private";
                //Channel channelToSpam = e.Server.AllChannels.FirstOrDefault(channel => channel.Name.Equals(channelName));

                //foreach (var user in e.Channel.Users)
                //{
                //    await e.Channel.SendMessage(string.Format("{0}: {1}", user.Mention, user.Status.Value));
                //}
            });

            commands.CreateCommand("hello everyone").Do(async (e) =>
            {
                await e.Channel.SendMessage("@everyone" + " hey how you doin lil mama? lemme whisper in your ear tell you sumthing that you might like to hear" +
                        " you got a sexy ass body and your ass look soft mind if I touch it? and see if its soft naw im jus playin' unless you say I can and im known to be a real nasty man" +
                        " and they say a closed mouth dont get fed so I don't mind askin for head you heard what I said, we need to make our way to the bed and you can start usin' yo head" +
                        " you like to fuck, have yo legs open all in da butt do it up slappin ass cuz the sex gets rough switch the positions and ready to get down to business" +
                        " so you can see what you've been missin' you might have some but you never had none like this just wait til you see my dick");
            });

            commands.CreateCommand("details").Do(async (e) =>
            {
                await e.Channel.SendMessage("```To get specific information on a command, simply do !details commandName!```");
            });

            commands.CreateCommand("details hello").Do(async (e) =>
            {
                await e.Channel.SendMessage("```In order to do the !hello command, please use the user's username, not nickname. In order to find out the user's username, simply click there name" +
                    " on the left side of your screen and you should see a picture of the user, the nickname, then the actual username!```");
            });
            //User Joined send message to the server code
            //client.UserJoined += async (s, e) =>
            //{
            //    var channel = e.Server.FindChannels("welcome", ChannelType.Text).FirstOrDefault();

            //    var user = e.User;

            //    await channel.SendMessage(string.Format("{0} has joined the channel!", user.Name));
            //};

            //User left send message to the server code
            //client.UserLeft += async (s, e) =>
            //{
            //    var channel = e.Server.FindChannels("welcome", ChannelType.Text).FirstOrDefault();

            //    var user = e.User;

            //    await channel.SendMessage(string.Format("{0} has left the channel!", user.Name));
            //};

            client.ExecuteAndWait(async () =>
            {
                await client.Connect("MTc4NjEyNTk2MTUxMjg3ODA4.C_kXoQ.jfdG8_5vboW1_Nso5ru2v0Znqdk", TokenType.Bot);
            });
        }

        private async Task SpamMessage(CommandEventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                await e.Channel.SendMessage("hello");
            }
        }

        private async Task DeleteMessage(CommandEventArgs e)
        {
            var messages = e.Args[0];

            Message[] messagesToDelete;

            messagesToDelete = await e.Channel.DownloadMessages(Int32.Parse(messages));

            await e.Channel.DeleteMessages(messagesToDelete);
        }

        private async Task ComplimentUser(CommandEventArgs e)
        {
            var username = e.Args[0];            

            var user = e.Server.FindUsers(@username).FirstOrDefault();            

            if (e.Args.Contains(user.Mention) || e.Args.Contains(user.NicknameMention))
            {
                if (user != null)
                {                    
                    await e.Channel.SendMessage(string.Format("{0} hey how you doin lil mama? lemme whisper in your ear tell you sumthing that you might like to hear" +
                        " you got a sexy ass body and your ass look soft mind if I touch it? and see if its soft naw im jus playin' unless you say I can and im known to be a real nasty man" +
                        " and they say a closed mouth dont get fed so I don't mind askin for head you heard what I said, we need to make our way to the bed and you can start usin' yo head" +
                        " you like to fuck, have yo legs open all in da butt do it up slappin ass cuz the sex gets rough switch the positions and ready to get down to business" +
                        " so you can see what you've been missin' you might have some but you never had none like this just wait til you see my dick", user.Mention));
                }
                else
                {
                    await e.Channel.SendMessage("No user with that name is in the server!");
                }
            }    
            else
            {
                await e.Channel.SendMessage("Command not preformed correctly!");
            }        
        }

        private async Task AwardUserTokens(CommandEventArgs e)
        {
            var username = e.Args[0];

            var user = e.Server.FindUsers(username).FirstOrDefault();

            var userRoles = e.User.Roles;


            if (userRoles.Any(input => input.Name.ToUpper() == "MODERATOR"))
            {
                if (user != null)
                {
                    var tokenAmount = e.Args[1];

                    try
                    {
                        TokenService.AwardTokens(user, tokenAmount);

                        await e.Channel.SendMessage(string.Format("{0} has been awarded with {1} tokens", user.Name, tokenAmount));
                    }
                    catch (Exception ex)
                    {
                        await e.Channel.SendMessage("Error occured, please contact Dylan");
                    }
                }
                else
                {
                    await e.Channel.SendMessage(string.Format("Could not find user with username {0}!", username));
                }
            }   
            else
            {
                await e.Channel.SendMessage("You do not have sufficient permissions for this command!");
            }         
        }

        private async Task DoAnnouncement(CommandEventArgs e)
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

        private string ConstructMessage(CommandEventArgs e, bool firstArgIsChannel)
        {
            string message = "";

            var name = e.User.Nickname != null ? e.User.Nickname : e.User.Name;

            var startIndex = firstArgIsChannel ? 1 : 0;

            for (int i = startIndex; i < e.Args.Length; i++)
            {
                message += e.Args[i].ToString() + " ";
            }

            var result = name + " says: " + message;

            return result;
        }

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
