using MargieBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MemoryBot
{
    class Utils
    {
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
        
        public static void Say(ReminderItem nextReminder)
        {
            SlackChatHub H = new SlackChatHub();
            H.ID = GetUserDMChannelID(nextReminder.UserName);
            BotMessage ConnectionMessage = new BotMessage();
            ConnectionMessage.Text = nextReminder.ReminderContent;
            ConnectionMessage.ChatHub = H;
            Program.memoryBot.Say(ConnectionMessage).Wait();
        }
    }
}
