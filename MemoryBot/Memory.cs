using MargieBot.Models;
using Newtonsoft.Json;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace MemoryBot
{
    static class Memory
    {
        private static Timer brainTimer;
        public static FastPriorityQueue<MemoryItem> memoryQueue;
        public static MemoryItem nextReminder;
        private static MemoryItem memoryHolder;
        private const int MAX_MEMORY_ITEMS = 100;

        public static void WriteMemory(string message, string user, DateTime reminderTime, string userName)
        {
            // save to memory cache
            //Console.WriteLine(reminderTime);

            MemoryItem memoryInput = new MemoryItem();
            memoryInput.UserID = user;
            memoryInput.UserName = userName;
            memoryInput.ReminderContent = message;
            memoryInput.ReminderTime = reminderTime;
            memoryInput.Priority = Utils.GetDateTimeDifference(memoryInput.ReminderTime);

            JsonSerializer serializer = new JsonSerializer();

            List<MemoryItem> memory = ReadMemory();
            RePrioritizeMemory(memory);
            memory.Add(memoryInput);
            
            using (StreamWriter sw = new StreamWriter(@"../../memory.json"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, memory);
            }
        }
        public static List<MemoryItem> ReadMemory()
        {
            using (StreamReader file = File.OpenText(@"../../memory.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                List<MemoryItem> memory = (List<MemoryItem>)serializer.Deserialize(file, typeof(List<MemoryItem>));
                EnqueueMemory(memory);
                return memory;
            }
        }

        private static void EnqueueMemory(List<MemoryItem> memory)
        {
            memoryQueue = new FastPriorityQueue<MemoryItem>(MAX_MEMORY_ITEMS);
            foreach (MemoryItem MI in memory)
            {
                memoryQueue.Enqueue(MI, MI.Priority);
            }
        }

        private static void RePrioritizeMemory(List<MemoryItem> memory)
        {
            //ReSorts memory to prioritize the most urgent memory
            foreach(MemoryItem MI in memory)
            {
                MI.Priority = Utils.GetDateTimeDifference(MI.ReminderTime);
            }
        }

        public static void GetNextReminder()
        {
            if(memoryQueue != null && memoryQueue.Count != 0)
            {
                Console.WriteLine("MEMORY QUEUE IS NOT EMPTY");
                memoryHolder = memoryQueue.Dequeue();
            }
        }

        public static void StartBrain()
        {
            ReadMemory();
            GetNextReminder();

            if (brainTimer != null)
                brainTimer.Change(Timeout.Infinite, Timeout.Infinite);

            brainTimer = new Timer(MemoryTimeComparison, null, 0, 2000);
        }

        public static void MemoryTimeComparison(Object o)
        {
            Console.WriteLine("Tick");

            if (memoryQueue != null && memoryHolder != null && memoryQueue.Count != 0)
            {
                Console.WriteLine((memoryHolder.ReminderTime - DateTime.Now).TotalMinutes);
                if ((memoryHolder.ReminderTime - DateTime.Now).TotalMinutes <= 1)
                {
                    nextReminder = memoryHolder;
                    Utils.Say(nextReminder);
                    GetNextReminder();
                }
            }
        }
    }
}
