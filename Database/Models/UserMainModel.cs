using System.Collections.Generic;

namespace Database.Models
{
    public class UserMainModel
    {
        public string Kind { get; set; }
        public List<UserMainInfoModel> Users { get; set; }
    }
}
