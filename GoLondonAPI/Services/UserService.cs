using System;
using GoLondonAPI.Data;
using GoLondonAPI.Domain.Entities;
using GoLondonAPI.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace GoLondonAPI.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }

        public async Task<User?> EditUserDetails(string userUUID, RegistratingUser details)
        {
            User user = await GetUserAsync(userUUID);
            if (user == null)
            {
                return null;
            }

            user.UserName = details.UserName;
            user.UserEmail = details.UserEmail;

            _context.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetUserAsync(string userUUID)
        {
            return await _context.Users
                    .AsNoTracking()
                    .Where(u => u.UserUUID == userUUID)
                    .Include(u => u.Projects)
                    .Include(u => u.Role)
                        .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserFromAPIKey(string apiKey)
        {
            return await _context.Users
                    .AsNoTracking()
                    .Include(u => u.Projects)
                    .Include(u => u.Role)
                        .ThenInclude(ur => ur.Role)
                    .Where(u => !u.IsDeleted && u.Projects.Any(p => p.APIKey == apiKey))
                    .FirstOrDefaultAsync();
        }

        public async Task<ICollection<User>> GetUsersAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Include(u => u.Projects)
                .Include(u => u.Role)
                    .ThenInclude(ur => ur.Role)
                .Where(u => !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> IsEmailFree(string email)
        {
            return !(await _context.Users.AnyAsync(u => !u.IsDeleted && u.UserEmail.ToLower().Trim() == email.ToLower().Trim()));
        }
    }
}

