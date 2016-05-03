using Newtonsoft.Json;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace MemoryBot
{
    static class Reminder
    {
        public static FastPriorityQueue<ReminderItem> reminderQueue;
        public static ReminderItem nextReminder;
        private static Timer brainTimer;
        private static ReminderItem reminderHolder;
        private const int MAX_MEMORY_ITEMS = 100;

        public static void AddReminderItem(string message, string user, DateTime reminderTime, string userName)
        {
            ReminderItem reminderInput = new ReminderItem();
            reminderInput.UserID = user;
            reminderInput.UserName = userName;
            reminderInput.ReminderContent = message;
            reminderInput.ReminderTime = reminderTime;
            reminderInput.Priority = Utils.GetDateTimeDifference(reminderInput.ReminderTime);

            ReadReminderMemory();
            reminderQueue.Enqueue(reminderInput, reminderInput.Priority);
            WriteReminderMemory();
        }

        public static void WriteReminderMemory()
        {
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(@"../../memory.json"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, reminderQueue);
            }
        }

        public static void ReadReminderMemory()
        {
            using (StreamReader file = File.OpenText(@"../../memory.json"))
            {
                reminderQueue = new FastPriorityQueue<ReminderItem>(MAX_MEMORY_ITEMS);
                JsonSerializer serializer = new JsonSerializer();
                List<ReminderItem> temp = (List<ReminderItem>)serializer.Deserialize(file, typeof(List<ReminderItem>));
                foreach(ReminderItem MI in temp)
                {
                    reminderQueue.Enqueue(MI, MI.Priority);
                }
                RePrioritizeMemory();
            }
        }

        private static void RePrioritizeMemory()
        {
            //ReSorts memory to prioritize the most urgent memory
            foreach(ReminderItem MI in reminderQueue)
            {
                MI.Priority = Utils.GetDateTimeDifference(MI.ReminderTime);
                reminderQueue.UpdatePriority(MI, MI.Priority);
            }
        }

        public static void GetNextReminder()
        {
            if(reminderQueue != null && reminderQueue.Count != 0)
            {
                Console.WriteLine("MEMORY QUEUE IS NOT EMPTY");
                reminderHolder = reminderQueue.Dequeue();
                Console.WriteLine(reminderQueue.Count);
            }
        }

        public static void StartBrain()
        {
            ReadReminderMemory();
            GetNextReminder();

            if (brainTimer != null)
            {
                brainTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
            brainTimer = new Timer(ReminderTimeComparison, null, 0, 2000);
        }

        public static void ReminderTimeComparison(Object o)
        {
            Console.WriteLine("Tick");
            if(reminderHolder == null)
            {
                GetNextReminder();
            }
            else 
            {
                Console.WriteLine((reminderHolder.ReminderTime - DateTime.Now).TotalMinutes);
                if ((reminderHolder.ReminderTime - DateTime.Now).TotalMinutes <= 1)
                {
                    nextReminder = reminderHolder;
                    reminderHolder = null;
                    Utils.Say(nextReminder);
                    WriteReminderMemory();
                    GetNextReminder();
                }
            }
        }
    }
}
