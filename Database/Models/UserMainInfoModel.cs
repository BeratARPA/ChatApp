using System;
using System.Collections.Generic;

namespace Database.Models
{
    public class UserMainInfoModel
    {
        public string LocalId { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string IdToken { get; set; }
        public List<ProviderUserMainInfoModel> ProviderUserInfo { get; set; }
        public string PhotoUrl { get; set; }
        public string RefreshToken { get; set; }
        public string ExpiresIn { get; set; }
        public string Password { get; set; }
        public string PasswordHash { get; set; }
        public bool EmailVerified { get; set; }
        public long PasswordUpdateAt { get; set; }
        public string ValidSince { get; set; }
        public string LastLoginAt { get; set; }
        public string CreatedAt { get; set; }
        public DateTime LastRefreshAt { get; set; }
    }
}
