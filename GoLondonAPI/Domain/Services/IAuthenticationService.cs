using System;
using GoLondonAPI.Data;
using GoLondonAPI.Domain.Entities;

namespace GoLondonAPI.Domain.Services
{
    public interface IAuthenticationService
    {
        Task<User?> Authenticate(AuthenticatingUser authUser, bool needsTokens = false);
        Task<User> RegisterNewUser(RegistratingUser registrationDetails);
        Task<bool> ResetPassword();
        string GetRandomHash();
        Tuple<string, string> HashAndSaltNewPassword(string password);
        bool VerifyHash(string UserPassHash, string UserSalt, string passwordToVerify);
        Task<UserAuthenticationTokens> GenerateInitialUserTokens(string userUUID);
        Task<string?> CreateAuthTokensForUser(string userUUID);
        Task<UserAuthenticationTokens> RefreshTokensForUser(string userUUID, string refreshToken);
        Task<bool> IsRefreshTokenValid(string userUUID, string refreshToken);
        Task InvalidateAllRefreshTokens(string userUUID);
    }
}

