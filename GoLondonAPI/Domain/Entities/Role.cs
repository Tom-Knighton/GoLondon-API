using System;
namespace GoLondonAPI.Domain.Entities
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int CallsPerMin { get; set; }

        public virtual ICollection<UserRole> Users { get; set; }
    }
}

