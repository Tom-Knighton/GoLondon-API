using System;
using GoLondonAPI.Data;
using GoLondonAPI.Domain.Entities;
using GoLondonAPI.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace GoLondonAPI.Services
{
    public class ProjectService : IProjectService
    {
        private readonly DataContext _context;
        private readonly IAuthenticationService _auth;

        public ProjectService(DataContext context, IAuthenticationService auth)
        {
            _context = context;
            _auth = auth;
        }

        public async Task<Project> CreateProject(CreateProjectDTO projectDTO)
        {
            Project project = new Project
            {
                ProjectId = Guid.NewGuid().ToString("N"),
                ProjectName = projectDTO.ProjectName,
                UserUUID = projectDTO.UserUUID,
                IsDeleted = false,
                APIKey = _auth.GetRandomHash()
            };
           
            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
            return await GetProjectAsync(project.ProjectId);
        }

        public async Task<bool> DeleteProject(string uuid)
        {
            Project p = await _context.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.ProjectId == uuid);
            if (p == null)
            {
                return false;
            }

            p.IsDeleted = true;
            _context.Update(p);
            return await _context.SaveChangesAsync() == 1;
        }

        public async Task<Project?> GetProjectAsync(string projectId)
        {
            return await _context.Projects
                .FirstOrDefaultAsync(p => !p.IsDeleted && p.ProjectId == projectId);
        }

        public async Task<Project?> GetProjectByAPIKey(string apiKey)
        {
            return await _context.Projects
                .FirstOrDefaultAsync(p => !p.IsDeleted && p.APIKey == apiKey);
        }

        public async Task<ICollection<Project>> GetProjectsAsync(bool includeUsers = false)
        {
            return await _context.Projects
                .Where(p => !p.IsDeleted)
                .IncludeIf(includeUsers, p => p.User, p => p.User.Role, p => p.User.Role.Role)
                .ToListAsync();
        }

        public async Task<ICollection<Project>> GetProjectsForUserAsync(string userUUID)
        {
            return await _context.Projects
                .Where(p => !p.IsDeleted && p.UserUUID == userUUID)
                .ToListAsync();
        }

        public async Task<Project?> RenameProject(string projectUUID, string newName)
        {
            Project p = await _context.Projects.AsNoTracking().FirstOrDefaultAsync(p => !p.IsDeleted && p.ProjectId == projectUUID);
            if (p == null)
            {
                throw new Exception("Invalid project UUID");
            }

            p.ProjectName = newName;
            _context.Update(p);
            await _context.SaveChangesAsync();
            return await GetProjectAsync(p.ProjectId);
        }
    }
}

