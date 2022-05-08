using System;
using GoLondonAPI.Domain.Entities;

namespace GoLondonAPI.Domain.Services
{
    public interface IProjectService
    {
        Task<ICollection<Project>> GetProjectsAsync(bool includeUsers = false);
        Task<Project?> GetProjectAsync(string projectId);
        Task<Project?> GetProjectByAPIKey(string apiKey);
    }
}

