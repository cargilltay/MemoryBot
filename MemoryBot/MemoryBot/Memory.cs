using Newtonsoft.Json;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.IO;

namespace MemoryBot
{
    static class Memory
    {
        public static FastPriorityQueue<MemoryItem> memoryQueue;
        public static MemoryItem nextReminder;
        private static MemoryItem memoryHolder;
        private const int MAX_MEMORY_ITEMS = 100;

        public static void WriteMemory(string message, string user, DateTime reminderTime)
        {
            // save to memory cache
            Console.WriteLine(reminderTime);

            MemoryItem memoryInput = new MemoryItem();
            memoryInput.UserID = user;
            memoryInput.ReminderContent = message;
            memoryInput.ReminderTime = reminderTime;
            memoryInput.Priority = Utils.GetDateTimeDifference(memoryInput.ReminderTime);

            JsonSerializer serializer = new JsonSerializer();

            List<MemoryItem> memory = ReadMemory();
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
        public static void GetNextReminder()
        {
            if(memoryQueue != null)
            {
                Console.WriteLine("MEMORY QUE IS NOT NULL");
                if (memoryQueue.Count != 0)
                {
                    memoryHolder = memoryQueue.Dequeue();
                    Console.WriteLine(memoryHolder.ReminderTime);
                }
            }
        }

        public static void MemoryTimeComparison(Object o)
        {
            Console.WriteLine("Tick");
            if(memoryHolder != null)
            {
                //Console.WriteLine(memoryHolder.ReminderTime);
            }
            if (memoryQueue != null && memoryHolder != null)
            {
                if (memoryQueue.Count != 0)
                {
                    Console.WriteLine("MADE IT");
                    if ((memoryHolder.ReminderTime - DateTime.Now).TotalMinutes <= 1)
                    {
                        Console.WriteLine("Date Compare Worked");
                        nextReminder = memoryHolder;
                        GetNextReminder();
                    }
                }
            }
        }
    }
}
