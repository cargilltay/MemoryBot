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
        public bool CanRespond(ResponseContext context)
        {
            bool canTalk = (context.Message.MentionsBot || context.Message.ChatHub.Type == SlackChatHubType.DM);
            if (canTalk || Memory.nextReminder != null)
            {
                return true;
            }
            return false;
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            if(Memory.nextReminder != null)
            {
                var reminder = Memory.nextReminder;
                Memory.nextReminder = null;

                return new BotMessage { Text = reminder.UserID + ", here is your reminder:" + reminder.ReminderContent};
            }
            return new BotMessage {Text =  ""};
        }
    }
}
