using MargieBot.Responders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MargieBot.Models;
using System.IO;
using Newtonsoft.Json;

namespace MemoryBot.Responses
{
    class MemoryMaker : IResponder
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
                        Memory.AddMemoryItem(msg, user, Utils.GetUserName(context));
                        return new BotMessage { Text = "I'll Shall Not Forget!" };
                    }
                }
            }
            return new BotMessage { Text = ""};
            //return new BotMessage { Text = string.Format("Hello {0}, I do not recognize your command", user) };
        }

        private void LoadCommands()
        {
            using (StreamReader file = File.OpenText(@"../../Commands/MemoryCommands.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                commands = (string[])serializer.Deserialize(file, typeof(string[]));
            }
        }
    }
}
