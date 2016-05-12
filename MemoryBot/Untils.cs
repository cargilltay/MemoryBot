using MargieBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MemoryBot
{
    class Utils
    {
        //https://www.youtube.com/watch?v=p3Fvy-JOaVw
        private static char[] trim = { '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '=', '+', '[', ']', '{', '}', '\\', '|', ';', ':', '"', '\'', ',', '<', '.', '>', '/', '?' };
        private static string MemoryAccessType;
        private static object writeItem;


        public static string StringBuilder(string message)
        {
            var clean = message.ToLower();
            var words = clean.Split(' ');

            // Check the first word.
            if (words[0].StartsWith("<@"))
                clean = String.Join(" ", words.Skip(1));

            // Check the last word
            if (words[words.Length - 1].StartsWith("<@"))
                clean = String.Join(" ", words.Take(words.Length - 1));

            return clean;
        }

        public static void PrintChatHubInfo()
        {
            foreach (KeyValuePair<string, SlackChatHub> hub in Program.memoryBot.ConnectedHubs)
            {
                Console.WriteLine(hub.Key);
                Console.WriteLine(hub.Value.Name);
                Console.WriteLine("----------------------");
            }
        }

        public static void PrintUserNameCache(ResponseContext context)
        {
            foreach (KeyValuePair<string, string> CacheItem in context.UserNameCache)
            {
                Console.WriteLine(CacheItem.Key);
                Console.WriteLine(CacheItem.Value);
                Console.WriteLine("---------------");
            }
        }

        public static string GetUserDMChannelID(string userName)
        {
            string userDMChannelID = "";
            foreach (KeyValuePair<string, SlackChatHub> hub in Program.memoryBot.ConnectedHubs)
            {
                if (userName == hub.Value.Name)
                {
                    userDMChannelID = hub.Key;
                    return userDMChannelID;
                }
            }
            Console.WriteLine(userDMChannelID);
            return userDMChannelID;
        }

        public static string GetUserName(ResponseContext context)
        {
            string userName = "";
            foreach (KeyValuePair<string, string> CacheItem in context.UserNameCache)
            {
                if (CacheItem.Key == context.Message.User.ID)
                {
                    userName = "@" + CacheItem.Value;
                    return userName;
                }
            }
            return userName;
        }

        public static double GetDateTimeDifference(DateTime reminderTime)
        {
            double diffInSeconds = (reminderTime - DateTime.Now).TotalSeconds;
            return diffInSeconds;
        }
        
        public static void Say(ReminderNode nextReminder)
        {
            SlackChatHub H = new SlackChatHub();
            H.ID = GetUserDMChannelID(nextReminder.UserName);
            BotMessage ConnectionMessage = new BotMessage();
            ConnectionMessage.Text = nextReminder.ReminderContent;
            ConnectionMessage.ChatHub = H;
            Program.memoryBot.Say(ConnectionMessage).Wait();
        }

        public static void WriteMemory()
        {
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(@"../../Memory/ReminderMemory.json"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                if(MemoryAccessType == "Reminder") serializer.Serialize(writer, Reminder.reminderQueue);
                //if(MemoryAccessType == "Memory") serializer.Serialize(writer, SOMEITEM)
            }
        }

        public static object ReadMemory()
        {
            using (StreamReader file = File.OpenText(@"../../Memory/ReminderMemory.json"))
            {
                JsonSerializer serializer = new JsonSerializer();

                if(MemoryAccessType == "Reminder")
                {
                   return (List<ReminderNode>)serializer.Deserialize(file, typeof(List<ReminderNode>));

                   
                }
                else if(MemoryAccessType == "Memory")
                {
                    //return something for memory
                    return "";
                }

                return "";
            }
        }

        public static void SetMemoryAccessType(string type)
        {
            MemoryAccessType = type;
        }
    }
}
