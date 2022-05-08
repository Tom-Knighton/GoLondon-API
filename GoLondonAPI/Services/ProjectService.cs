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

        public ProjectService(DataContext context)
        {
            _context = context;
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
    }
}

