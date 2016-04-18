using MargieBot.Models;
using MargieBot.Responders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.IO;
using Priority_Queue;
using System.Threading;

namespace MemoryBot.Responses
{
    class MemoryResponder : IResponder
    {
        private string[] commands;
        private FastPriorityQueue<MemoryItem> memoryQueue;
        private const int MAX_MEMORY_ITEMS = 100;
        MemoryItem nextReminder;

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
                        //Somehow communicate that it is reminder timer to user throught GetResponse.
                            //maybe look at marks code for this
                        WriteMemory(msg, user, DateTime.Now.AddSeconds(15));
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

        private void WriteMemory(string message, string user, DateTime reminderTime)
        {
            // save to memory cache
            MemoryItem memoryInput = new MemoryItem();
            memoryInput.UserID = user;
            memoryInput.ReminderContent = message;
            memoryInput.ReminderTime = reminderTime;
            memoryInput.Priority = GetDateTimeDifference(memoryInput.ReminderTime);

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            List<MemoryItem> memory = ReadMemory();
            memory.Add(memoryInput);

            var data = serializer.Serialize(memory);
            File.WriteAllText(@"../../memory.json", data);
        }
        private List<MemoryItem> ReadMemory()
        {
            using (StreamReader file = File.OpenText(@"../../memory.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                List<MemoryItem> memory = (List<MemoryItem>)serializer.Deserialize(file, typeof(List<MemoryItem>));
                EnqueueMemory(memory);
                return memory;
            }
        }

        private double GetDateTimeDifference(DateTime reminderTime)
        {
            double diffInSeconds = (reminderTime - DateTime.Now).TotalSeconds;
            return diffInSeconds;
        }

        private void EnqueueMemory(List<MemoryItem> memory)
        {
            memoryQueue = new FastPriorityQueue<MemoryItem>(MAX_MEMORY_ITEMS);
            foreach (MemoryItem MI in memory)
            {
                memoryQueue.Enqueue(MI, MI.Priority);
            }
            GetNextReminder();
        }

        private void GetNextReminder()
        {
            nextReminder = memoryQueue.Dequeue();
            Timer timer = new Timer(TimerCallback, null, 0, 2000);
        }

        private void TimerCallback(Object o)
        {
            if (memoryQueue.Count != 0)
            {
                if(nextReminder.ReminderTime == DateTime.Now)
                {
                    GetNextReminder();
                }
            }
        }
    }
}
