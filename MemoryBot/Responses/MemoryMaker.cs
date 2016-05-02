using MargieBot.Models;
using MargieBot.Responders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Priority_Queue;
using System.Threading;

namespace MemoryBot.Responses
{
    class MemoryMaker : IResponder
    {
        private string[] commands;

        public bool CanRespond(ResponseContext context)
        {
            //logs messages from slack
            //Console.WriteLine(context.Message.Text);
            
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
                    if (context.Message.Text.Contains(command))
                    {
                        Memory.WriteMemory(msg, user, DateTime.Now);
                        return new BotMessage { Text = "I'll remind you" };
                    }
                }
            }
            return new BotMessage { Text =  string.Format("Hello {0}",user)};
        }

        private void LoadCommands()
        {
            using (StreamReader file = File.OpenText(@"../../commands.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                commands = (string[])serializer.Deserialize(file, typeof(string[]));
            }
        }
    }
}
