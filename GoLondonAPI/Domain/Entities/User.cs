namespace GoLondonAPI.Domain.Entities
{
    public class User
    {
        public string UserUUID { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPassHash { get; set; }
        public string UserPassSalt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<Project> Projects { get; set; }
        public virtual UserRole Role { get; set; }
    }
}

