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
        public static FastPriorityQueue<ReminderNode> reminderQueue;
        public static ReminderNode nextReminder;
        private static Timer brainTimer;
        private static ReminderNode reminderHolder;
        private const int MAX_MEMORY_ITEMS = 100;

        public static void AddReminderItem(string message, string user, DateTime reminderTime, string userName)
        {
            ReminderNode reminderInput = new ReminderNode();
            reminderInput.UserID = user;
            reminderInput.UserName = userName;
            reminderInput.ReminderContent = message;
            reminderInput.ReminderTime = reminderTime;
            reminderInput.Priority = Utils.GetDateTimeDifference(reminderInput.ReminderTime);

            ReadAndEnqueueMemory();
            reminderQueue.Enqueue(reminderInput, reminderInput.Priority);
            Utils.SetMemoryAccessType("Reminder");
            Utils.WriteMemory();
        }

        

        private static void RePrioritizeMemory()
        {
            //ReSorts memory to prioritize the most urgent memory
            foreach(ReminderNode MI in reminderQueue)
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
            ReadAndEnqueueMemory();
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
                    Utils.SetMemoryAccessType("Reminder");
                    Utils.Say(nextReminder);
                    Utils.WriteMemory();
                    GetNextReminder();
                }
            }
        }
        private static void ReadAndEnqueueMemory()
        {
            reminderQueue = new FastPriorityQueue<ReminderNode>(MAX_MEMORY_ITEMS);
            Utils.SetMemoryAccessType("Reminder");
            List<ReminderNode> temp = (List<ReminderNode>)Utils.ReadMemory();
            foreach (ReminderNode MI in temp)
            {
                reminderQueue.Enqueue(MI, MI.Priority);
            }
            RePrioritizeMemory();
        }
    }
}
