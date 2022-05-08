using System;
namespace GoLondonAPI.Domain.Entities
{
    public class Project
    {
        public string ProjectId { get; set; }
        public string UserUUID { get; set; }
        public string ProjectName { get; set; }
        public string APIKey { get; set; }
        public bool IsDeleted { get; set; }

        public virtual User User { get; set; }
    }
}

