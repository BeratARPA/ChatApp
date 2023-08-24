namespace Database.Models
{
    public class Message
    {
        public string Key { get; set; }
        public string MessageSenderUserId { get; set; }
        public string MessageSenderDisplayName { get; set; }
        public string MessageReceiverUserId { get; set; }
        public string MessageReceiverDisplayName { get; set; }
        public string Text { get; set; }
    }
}
