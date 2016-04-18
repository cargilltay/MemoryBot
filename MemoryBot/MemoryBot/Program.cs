using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MargieBot.Models;
using MargieBot.Responders;
using MargieBot;

namespace MemoryBot
{
    class Program
    {
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

            await memory.Connect("************************************");
        }

        public static void InitResponses(Bot bot)
        {
            bot.Responders.Add(new Responses.MemoryResponder());
        }
    }
}
