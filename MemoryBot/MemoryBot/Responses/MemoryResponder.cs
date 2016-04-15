using MargieBot.Models;
using MargieBot.Responders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.IO;
using MemoryBot;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MemoryBot.Responses
{
    class MemoryResponder : IResponder
    {
        string[] commands;

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
            LoadJson();
            var msg = Utils.StringBuilder(context.Message.Text);
            var user = context.Message.User.FormattedUserID;

            if (context.Message.MentionsBot)
            {
                foreach (string command in commands)
                {
                    if (context.Message.Text.Contains(command))
                    {
                        writeMemory(msg, user, DateTime.Now);
                        return new BotMessage { Text = "I'll remind you" };
                    }
                }
            }
            return new BotMessage { Text =  string.Format("Hello {0}",user)};
        }

        public void LoadJson()
        {
            using (StreamReader file = File.OpenText(@"../../commands.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                commands = (string[])serializer.Deserialize(file, typeof(string[]));
            }
        }

        public void writeMemory(string message, string user, DateTime reminderTime)
        {
            // save to memory cache
            string[] memoryItem = new string[3];
            memoryItem[0] = user;
            memoryItem[1] = message;
            memoryItem[2] = reminderTime.ToString();

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            List<string[]> memory = readMemory();
            memory.Add(memoryItem);

            var data = serializer.Serialize(memory);
            File.WriteAllText(@"../../memory.json", data);
        }
        public List<string[]> readMemory()
        {
            using (StreamReader file = File.OpenText(@"../../memory.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                var memory = serializer.Deserialize(file, typeof(List<string[]>));
                return (List<string[]>)memory;
            }
        }
    }
}
