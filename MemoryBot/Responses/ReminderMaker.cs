using MargieBot.Models;
using MargieBot.Responders;
using Newtonsoft.Json;
using System;
using System.IO;

namespace MemoryBot.Responses
{
    class ReminderMaker : IResponder
    {
        private string[] commands;

        public bool CanRespond(ResponseContext context)
        {
            bool canTalk = (context.Message.MentionsBot || context.Message.ChatHub.Type == SlackChatHubType.DM);
            if (canTalk)
            {
                    return true;
            }
            return false;
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            LoadCommands();
            var msg = Utils.StringBuilder(context.Message.Text);
            var user = context.Message.User.FormattedUserID;

            if (context.Message.MentionsBot)
            {
                foreach (string command in commands)
                {
                    if (context.Message.Text.ToLower().Contains(command))
                    {
                        Console.WriteLine(context.Message.User.ID);
                        Reminder.AddReminderItem(msg, user, DateTime.Now.AddMinutes(2), Utils.GetUserName(context));
                        return new BotMessage { Text = "I'll remind you" };
                    }
                }
            }
            return new BotMessage { Text =  ""};
        }

        private void LoadCommands()
        {
            using (StreamReader file = File.OpenText(@"../../Commands/ReminderCommands.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                commands = (string[])serializer.Deserialize(file, typeof(string[]));
            }
        }
    }
}
