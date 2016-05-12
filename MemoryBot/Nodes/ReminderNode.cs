using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Priority_Queue;

namespace MemoryBot
{
    class ReminderNode : FastPriorityQueueNode
    {
        public string UserID;
        public string UserName;
        public string ReminderContent;
        public DateTime ReminderTime;
    }
}
