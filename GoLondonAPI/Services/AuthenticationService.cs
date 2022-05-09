using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GoLondonAPI.Data;
using GoLondonAPI.Domain.Entities;
using GoLondonAPI.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GoLondonAPI.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly DataContext _context;
        private readonly IUserService _userService;
        private readonly AppSettings _appSettings;

        public AuthenticationService(DataContext context, IUserService userService, IOptions<AppSettings> options)
        {
            _context = context;
            _userService = userService;
            _appSettings = options.Value;
        }

        public async Task<User?> Authenticate(AuthenticatingUser authUser, bool needsTokens = false)
        {
            User user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserEmail.ToLower().Trim() == authUser.UserEmailAddress.ToLower().Trim());
            if (user == null)
                return null;

            if (!VerifyHash(user.UserPassHash, user.UserPassSalt, authUser.UserPassword))
            {
                return null;
            }

            user = await _userService.GetUserAsync(user.UserUUID);
            user.AuthTokens = needsTokens ? await GenerateInitialUserTokens(user.UserUUID) : null;
            return user;
        }

        public async Task<User> RegisterNewUser(RegistratingUser registrationDetails)
        {
            if (!await _userService.IsEmailFree(registrationDetails.UserEmail))
            {
                throw new AuthenticationException("A user with this email address already exists");
            }

            string newGUID = Guid.NewGuid().ToString("N");
            Tuple<string, string> hashedPasses = HashAndSaltNewPassword(registrationDetails.UserPassword);
            User user = new()
            {
                UserUUID = newGUID,
                UserName = registrationDetails.UserName,
                UserEmail = registrationDetails.UserEmail,
                UserPassHash = hashedPasses.Item1,
                UserPassSalt = hashedPasses.Item2,
                Role = new UserRole
                {
                    UserUUID = newGUID,
                    RoleId = 1
                }
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return await _userService.GetUserAsync(newGUID);
        }

        public async Task<string?> CreateAuthTokensForUser(string userUUID)
        {
            User user = await _userService.GetUserAsync(userUUID);
            if (user == null)
            {
                return null;
            }

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] secretKey = Encoding.ASCII.GetBytes(_appSettings.Secret);
            List<Claim> claims = new List<Claim> { new Claim(ClaimTypes.Name, userUUID) };
            claims.Add(new Claim(ClaimTypes.Role, user.Role.Role.RoleName));

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims.ToArray()),
                Expires = DateTime.Now.AddMinutes(1), //TODO: Set
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _appSettings.Issuer,
                Audience = _appSettings.Audience,
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.UtcNow
            };

            SecurityToken token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<UserAuthenticationTokens> GenerateInitialUserTokens(string userUUID)
        {
            string newRefreshToken = GetRandomHash();
            UserRefreshToken token = new UserRefreshToken
            {
                RefreshToken = newRefreshToken,
                UserUUID = userUUID,
                TokenIssueDate = DateTime.UtcNow,
                TokenExpiryDate = DateTime.UtcNow.AddMonths(2),
                TokenClient = "N/A",
                IsDeleted = false
            };

            await _context.UserRefreshTokens.AddAsync(token);
            await _context.SaveChangesAsync();

            return new UserAuthenticationTokens
            {
                AuthenticationToken = await CreateAuthTokensForUser(userUUID) ?? "",
                RefreshToken = newRefreshToken
            };
        }

        public string GetRandomHash()
        {
            byte[] bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes);
        }

        public Tuple<string, string> HashAndSaltNewPassword(string password)
        {
            string salt = GetRandomHash();
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), iterations: 4096, HashAlgorithmName.SHA256);
            return new Tuple<string, string>(Convert.ToBase64String(pbkdf2.GetBytes(64)), salt);
        }

        public async Task InvalidateAllRefreshTokens(string userUUID)
        {
            ICollection<UserRefreshToken> tokens = await _context.UserRefreshTokens.Where(t => t.UserUUID == userUUID).ToListAsync();

            foreach (UserRefreshToken token in tokens)
            {
                token.IsDeleted = true;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsRefreshTokenValid(string userUUID, string refreshToken)
        {
            UserRefreshToken token = await _context.UserRefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.UserUUID == userUUID && t.RefreshToken == refreshToken && !t.IsDeleted);

            if (token != null)
            {
                return token.TokenExpiryDate > DateTime.UtcNow;
            }

            return false;
        }

        public async Task<UserAuthenticationTokens> RefreshTokensForUser(string userUUID, string refreshToken)
        {
            if (!await IsRefreshTokenValid(userUUID, refreshToken))
                return null;

            UserRefreshToken token = await _context.UserRefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.UserUUID == userUUID && t.RefreshToken == refreshToken);
            if (token == null)
                return null;

            token.IsDeleted = true;
            string newRefreshToken = GetRandomHash();
            UserRefreshToken userRefreshToken = new UserRefreshToken
            {
                UserUUID = userUUID,
                RefreshToken = newRefreshToken,
                TokenClient = "N/A",
                TokenIssueDate = DateTime.UtcNow,
                TokenExpiryDate = DateTime.UtcNow.AddMonths(2),
                IsDeleted = false
            };

            await _context.UserRefreshTokens.AddAsync(userRefreshToken);
            _context.UserRefreshTokens.Update(token);
            await _context.SaveChangesAsync();

            return new UserAuthenticationTokens
            {
                AuthenticationToken = await CreateAuthTokensForUser(userUUID) ?? "",
                RefreshToken = newRefreshToken
            };

        }

        public Task<bool> ResetPassword()
        {
            throw new NotImplementedException();
        }

        public bool VerifyHash(string UserPassHash, string UserSalt, string passwordToVerify)
        {
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(passwordToVerify, Convert.FromBase64String(UserSalt), iterations: 4096, HashAlgorithmName.SHA256);
            return Convert.ToBase64String(pbkdf2.GetBytes(64)).Equals(UserPassHash);
        }
    }
}

