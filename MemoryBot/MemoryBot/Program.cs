using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MargieBot.Models;
using MargieBot.Responders;
using MargieBot;
using System.Threading;

namespace MemoryBot
{
    class Program
    {
        //private static Timer brainTimer;

        static void Main(string[] args)
        {
            InitBot();
            while (true) { };
        }
        
        public static async void InitBot()
        {
            Bot memory = new Bot();
            InitResponses(memory);

            memory.ConnectionStatusChanged += (bool isConnected) =>
            {
                if (isConnected)
                    memory.Say(new BotMessage() { Text = "Hi Taylor!" });
            };

            await memory.Connect("********************************");
        }

        public static void InitResponses(Bot bot)
        {
            bot.Responders.Add(new Responses.MemoryMaker());
            bot.Responders.Add(new Responses.MemoryResponder());
        }
    }
}
