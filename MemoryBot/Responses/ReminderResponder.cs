using MargieBot.Models;
using MargieBot.Responders;

namespace MemoryBot.Responses
{
    class ReminderResponder : IResponder
    {
        public bool CanRespond(ResponseContext context)
        {
            bool canTalk = (context.Message.MentionsBot || context.Message.ChatHub.Type == SlackChatHubType.DM);
            if (canTalk || Reminder.nextReminder != null)
            {
                return true;
            }
            return false;
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            if(Reminder.nextReminder != null)
            {
                var reminder = Reminder.nextReminder;
                Reminder.nextReminder = null;

                return new BotMessage { Text = reminder.UserID + ", here is your reminder:" + reminder.ReminderContent};
            }
            return new BotMessage {Text =  ""};
        }
    }
}
