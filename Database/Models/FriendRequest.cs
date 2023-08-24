namespace Database.Models
{
    public class FriendRequest
    {
        public string Key { get; set; }
        public bool Accepted{ get; set; }
        public string ReceiverEmail { get; set; }
        public string SenderEmail { get; set; }
    }
}
