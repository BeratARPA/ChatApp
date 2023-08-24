namespace Database.Models
{
    public class UserInfoModel
    {
        public string Kind { get; set; }
        public string LocalId { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string IdToken { get; set; }
        public bool Registered { get; set; }
        public string ProfilePicture { get; set; }
    }
}
