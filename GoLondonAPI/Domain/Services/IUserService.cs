using System;
using GoLondonAPI.Domain.Entities;

namespace GoLondonAPI.Domain.Services
{
    public interface IUserService
    {
        Task<ICollection<User>> GetUsersAsync();
        Task<User?> GetUserAsync(string userUUID);
        Task<User?> GetUserFromAPIKey(string apiKey);
        Task<bool> IsEmailFree(string email);

        Task<User?> EditUserDetails(string userUUID, RegistratingUser details);
    }
}

