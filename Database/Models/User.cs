using System.Collections.Generic;

namespace Database.Models
{
    public class User
    {
        public string Key { get; set; }
        public string Email { get; set; }
        public bool Status { get; set; }
        public Dictionary<string, FriendRequest> FriendRequests { get; set; }
        public Dictionary<string, Friend> Friends { get; set; }
        public Dictionary<string, Chat> Chats { get; set; }
    }
}
