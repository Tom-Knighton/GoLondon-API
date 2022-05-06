using System;
namespace GoLondonAPI.Domain.Entities
{
    public class UserRole
    {
        public string UserUUID { get; set; }
        public int RoleId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}

