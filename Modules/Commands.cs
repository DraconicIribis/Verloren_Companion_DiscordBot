using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using static Verloren_Companion_Bot.LycanEvents;
using static Verloren_Companion_Bot.DreamEvents;


namespace Verloren_Companion_Bot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {

        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync(Context.Message.Author.Mention);
        }

        [Command("help")]
        public async Task Help()
        {
            var helpEmbed = new EmbedBuilder();

            helpEmbed.WithDescription("Useful commands to give you resources or help!");
            helpEmbed.WithTitle("Help Page");
            helpEmbed.AddField("**__Management__**", "**Ping**\nReturns a message (testing if bot is online). (V-ping)", true);
            helpEmbed.AddField("**__Resources__**", "**Rule Books**\nReturns a link to many 5e rule books. (V-books)\n**Maps**\nReturns a link to the world map. Will be updated in the future so you can choose an area map to link too (as you discover more areas). (V-map + map name)\n**Characters**\nGives you a summary of a player's character. (V-pc + irl player's name)", true);
            helpEmbed.WithFooter(footer => footer.Text = "Thanks for using the bot y'all!!!");
            helpEmbed.WithColor(new Color(0x792891));
            await Context.Channel.SendMessageAsync("", false, helpEmbed.Build());
        }

        [Command("soulcurse")]
        public async Task scSim(IUser user)
        {
            var scRnd = new Random();
            int severity = scRnd.Next(1,100);
            int time = scRnd.Next(12,120);
            string[][] masterArray = new string[5][];
            var author = Context.Message.Author;
            ulong roleId = 681934468352573538;
            var role = Context.Guild.GetRole(roleId);

            if (((SocketGuildUser) author).Roles.Contains(role))
            {
                masterArray[0] = new string[] {"You raise your voice.","You become slightly more hostile, aggressive, and reckless.","You treat your friends with anger and contempt."};
                masterArray[1] = new string[] {"You rush enemies more aggressively.","You will turn on your friends if they oppose you in any way (not with weaponry, but with anger and sometimes wrestling).","You smile upon killing anything and become without reason."};
                masterArray[2] = new string[] {"You will turn against your allies for even simple mistakes (still no weapons, just anger and wrestling).","Your bloodlust is almost uncontrollable and you actively seek out more enemies.","You yell almost constantly and treat everything with aggression."};
                masterArray[3] = new string[] {"You treat your allies no longer as friends. You will attack them for simple mistakes and try to get them unconscious.","Your bloodlust is uncontrollable, you will kill every enemy in sight and bathe in their blood.","You yell constantly and treat everything with aggression. You are suspicious of everything."};
                masterArray[4] = new string[] {"You have lost all control of your actions.","You will attack every possible living thing in sight, ally or not.","You no longer talk in your normal accent or language, but in hissings, growlings, and simple phrases."};

                int roundTime = time / 6;
                int severityIndex = severity / 20;
                string effects = "\n - " + masterArray[severityIndex][0] + "\n - " + masterArray[severityIndex][1] + "\n - " + masterArray[severityIndex][2];

                string msg = $"**__Soulcurse__**\n{severity}% severity\nLasts:\n   {time} seconds\n   {roundTime} rounds\nThe General Effects:{effects}";

                await Discord.UserExtensions.SendMessageAsync(author, severity + "% Severity\n" + time + " seconds\n" + roundTime + " rounds\n" + severityIndex + " severity array" + effects);
                await Discord.UserExtensions.SendMessageAsync(user, msg);
            }
        }

        [Command("lycanthrope")]
        public async Task lycanSim(IUser user)
        {
            var lycanRnd = new Random();
            double weight = lycanRnd.NextDouble();
            int severity;
            var author = Context.Message.Author;
            ulong roleId = 681934468352573538;
            var role = Context.Guild.GetRole(roleId);

            if (((SocketGuildUser) author).Roles.Contains(role))
            {
                if (weight > 0.9)
                {
                    severity = 4;
                }
                else if ((weight <= 0.9) && (weight > 0.75))
                {
                    severity = 3;
                }
                else if ((weight <= 0.75) && (weight > 0.5))
                {
                    severity = 2;
                }
                else if ((weight <= 0.5) && (weight > 0.25))
                {
                    severity = 1;
                }
                else
                {
                    severity = 0;
                }

                LycanEvents events = new LycanEvents();

                string[][] severityArray = await events.getLycanEvents();

                string[][] masterArray = new string[5][];

                masterArray[0] = severityArray[0];
                masterArray[1] = severityArray[1];
                masterArray[2] = severityArray[2];
                masterArray[3] = severityArray[3];
                masterArray[4] = severityArray[4];

                int eventText = lycanRnd.Next(0,masterArray[severity].Length);

                string msg = $"You wake up in the middle of the night as your tiger form. {masterArray[severity][0]}";

                string[] notes = msg.Split('<','>');

                await Discord.UserExtensions.SendMessageAsync(author, $"Severity {severity}\nNotes:");

                foreach (var item in notes)
                {
                    if (item.Contains('[') || item.Contains('{') || item.Contains('('))
                    {
                        await Discord.UserExtensions.SendMessageAsync(author, " - " + item.ToString());
                    }
                }

                msg = msg.Replace("<","");
                msg = msg.Replace(">","");

                await Discord.UserExtensions.SendMessageAsync(user, msg);
            }
        }

        [Command("dream")]
        public async Task dreamProphecy(IUser user)
        {
            var dreamRnd = new Random();
            double weight = dreamRnd.NextDouble();
            string[][] masterArray = new string[3][];
            int revealingFactor = 0;
            var author = Context.Message.Author;
            ulong roleId = 681934468352573538;
            var role = Context.Guild.GetRole(roleId);

            if (((SocketGuildUser) author).Roles.Contains(role))
            {
                if (weight >= 0.8)
                {
                    revealingFactor = 2;
                }
                else if ((weight < 0.8) && (weight >= 0.4))
                {
                    revealingFactor = 1;
                }
                else if (weight < 0.4)
                {
                    revealingFactor = 0;
                }

                DreamEvents events = new DreamEvents();

                string[][] deepArray = await events.getDreamEvents();

                masterArray[0] = deepArray[0];
                masterArray[1] = deepArray[1];
                masterArray[2] = deepArray[2];

                int eventText = dreamRnd.Next(0,masterArray[revealingFactor].Length);

                string msg = $"You are sleeping deeply when a dream befalls you. {masterArray[revealingFactor][eventText]}";

                await Discord.UserExtensions.SendMessageAsync(author, revealingFactor + " Dream Severity Factor\n" + masterArray[revealingFactor][eventText]);
                await Discord.UserExtensions.SendMessageAsync(user, msg);
            }
        }

        [Command("books")]
        public async Task ruleBooks()
        {
            await Context.Channel.SendMessageAsync("https://drive.google.com/open?id=16fCUXUV5sIPcacUtmZpzbjPHhxOJrMVh");
        }

        [Command("map")]
        public async Task giveMap(string input)
        {
            string mapPick = input;
            mapPick = mapPick.ToLower();

            if (mapPick.Equals("world") || mapPick.Equals("verloren") || mapPick.Equals("area"))
            {
                await Context.Channel.SendMessageAsync("https://imgur.com/a/1eT1efQ");
            }
            else
            {
                await Context.Channel.SendMessageAsync("That is not a map name. Try again (type the command again).");
            }
        }

        [Command("pc")]
        public async Task findPC(string input)
        {
            input = input.ToLower();

            if (input.Equals("jack") || input.Equals("jackson"))
            {
                await Context.Channel.SendMessageAsync("Jack's PC:\n  Name: Amerigo\n  Race: Human\n  Class/Subclass: Fighter; Eldritch Knight\n - Occupies himself with painting and art\n - Closely resembles (in our world) an Italian painter");
            }
            else if (input.Equals("kyler"))
            {
                await Context.Channel.SendMessageAsync("Kyler's PC:\n  Name: Shadow\n  Race: Dragonborn\n  Class/Subclass: Bard\n - Personality remains undetermined for now, will fix as time progresses.");
            }
            else if (input.Equals("jared"))
            {
                await Context.Channel.SendMessageAsync("Jared's PC:\n  Name: Adran Galanodel\n  Race: High Elf\n  Class/Subclass: Wizard; Psionics\n - Wise and intelligent. Focuses on sharpening his knowledge and mind.\n - Enjoys a lot of Astronomy.");
            }
            else if (input.Equals("shilo") || input.Equals("shiloth"))
            {
                await Context.Channel.SendMessageAsync("Shilo's PC:\n  Name: Tachinga\n  Race: Lizardfolk\n  Class/Subclass: Rogue; Swashbuckler\n - Really low intelligence, to the point of being comedic.\n - Always has wanted to own an airship.");
            }
            else if (input.Equals("brian"))
            {
                await Context.Channel.SendMessageAsync("Brian's PC:\n  Name: Perelor Soulcursed\n  Race: Human\n  Class/Subclass: Paladin; Never told me\n - Silent but strong in combat.");
            }
            else if (input.Equals("sofie") || input.Equals("sofia"))
            {
                await Context.Channel.SendMessageAsync("Sofie's PC:\n __TBD__");
            }
            else if (input.Equals("nathan"))
            {
                await Context.Channel.SendMessageAsync("Nathan's PC:\n  Name: Thrul'loc\n  Race: Dragonborn\n  Class/Subclass: Fighter; Eldritch Knight\n - Reserved and typically does better alone than with a group.\n - An excellent warrior and fighter.");
            }
            else
            {
                await Context.Channel.SendMessageAsync("That is not a person's name. Please try again.");
            }
        }
    }
}