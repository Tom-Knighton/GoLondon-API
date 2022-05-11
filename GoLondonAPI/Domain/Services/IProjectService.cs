using System;
using GoLondonAPI.Domain.Entities;

namespace GoLondonAPI.Domain.Services
{
    public interface IProjectService
    {
        Task<ICollection<Project>> GetProjectsAsync(bool includeUsers = false);
        Task<Project?> GetProjectAsync(string projectId);
        Task<Project?> GetProjectByAPIKey(string apiKey);
        Task<ICollection<Project>> GetProjectsForUserAsync(string userUUID);

        Task<Project> CreateProject(CreateProjectDTO project);
        Task<Project?> RenameProject(string projectUUID, string newName);
        Task<bool> DeleteProject(string uuid);
     }
}

