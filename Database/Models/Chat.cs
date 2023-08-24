using System.Collections.Generic;

namespace Database.Models
{
    public class Chat
    {
        public string Key { get; set; }
        public string ChatId { get; set; }
        public string ChatReceiverUserId { get; set; }
        public Dictionary<string, Message> Messages { get; set; }
    }
}
