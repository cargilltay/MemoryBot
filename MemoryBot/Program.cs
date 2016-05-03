using System;
using MargieBot.Models;
using MargieBot;

namespace MemoryBot
{
    class Program
    {
        public static Bot memoryBot;

        static void Main(string[] args)
        {
            InitBot();

            while (true) { };
        }

        public static async void InitBot()
        {
            memoryBot = new Bot();

            InitResponses(memoryBot);

            memoryBot.ConnectionStatusChanged += (bool isConnected) =>
            {
                if (isConnected)
                {
                    //Utils.PrintChatHubInfo();

                    SlackChatHub H = new SlackChatHub();
                    H.ID = "********";
                    Console.WriteLine("Connected");
                    BotMessage ConnectionMessage = new BotMessage();
                    ConnectionMessage.Text = "hello taylor";
                    ConnectionMessage.ChatHub = H;
                    memoryBot.Say(ConnectionMessage).Wait();
                }
            };

            await memoryBot.Connect("***************);
            Reminder.StartBrain();
        }

        public static void InitResponses(Bot bot)
        {
            bot.Responders.Add(new Responses.ReminderMaker());
            bot.Responders.Add(new Responses.ReminderResponder());
        }
    }
}
