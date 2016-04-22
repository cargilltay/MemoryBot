using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MargieBot;
using MargieBot.Models;
using MargieBot.Responders;
using Priority_Queue;
using System.Threading;

namespace MemoryBot.Responses
{
    class MemoryResponder : IResponder
    {
        private static Timer brainTimer;

        public bool CanRespond(ResponseContext context)
        {
            Memory.GetNextReminder();
            brainTimer = new Timer(Memory.MemoryTimeComparison, null, 0, 2000);
            bool canTalk = (context.Message.MentionsBot || context.Message.ChatHub.Type == SlackChatHubType.DM);
            if (canTalk)
            {
                return true;
            }
            return false;
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            if(Memory.nextReminder != null)
            {

                Console.WriteLine("HELLO");
                var reminder = Memory.nextReminder;
                return new BotMessage { Text = reminder.UserID + ", here is your reminder:" + reminder.ReminderContent};
            }
            return new BotMessage {Text =  ""};
        }
    }
}
