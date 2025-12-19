using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using Server.Models;
using SharedLibrary;

namespace Server.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly Settings _settings;
    private readonly GameDbContext _context;

    public AuthenticationService(Settings settings, GameDbContext context)
    {
        _settings = settings;
        _context = context;
    }

    public (bool success, string content) Register(string userName, string password)
    {
        if (_context.Users.Any(u => u.UserName == userName))
        {
            return (false, "Username not available");
        }
        var user = new User
        {
            UserName = userName, PasswordHash = password
        };
        user.ProvideSaltAndHash();

        _context.Add(user);
        _context.SaveChanges();

        return (true, "");
    }

    public (bool success, string token) Login(string userName, string password)
    {
        var user = _context.Users.SingleOrDefault(u => u.UserName == userName);

        if (user == null)
        {
            return (false, "Invalid userName");
        }

        if (user.PasswordHash != AuthenticationHelpers.ComputeHash(password, user.Salt))
        {
            return (false, "Invalid password");
        }

        return (true, GenerateJwtToken(AssembleClaimsIdentity(user)));
    }

    private ClaimsIdentity AssembleClaimsIdentity(User user)
    {
        var subject = new ClaimsIdentity(new[]
        {
            new Claim("id", user.Id.ToString()),
        });

        return subject;
    }

    private string GenerateJwtToken(ClaimsIdentity subject)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_settings.BearerKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            Expires = DateTime.Now.AddYears(10),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

public interface IAuthenticationService
{
    (bool success, string content) Register(string userName, string password);
    (bool success, string token) Login(string userName, string password);
}

public static class AuthenticationHelpers
{
    public static void ProvideSaltAndHash(this User user)
    {
        var salt = GenerateSalt();
        user.Salt = Convert.ToBase64String(salt);
        user.PasswordHash = ComputeHash(user.PasswordHash, user.Salt);
    }

    private static byte[] GenerateSalt()
    {
        var rng = RandomNumberGenerator.Create();
        var salt = new byte[24];
        rng.GetBytes(salt);
        return salt;
    }

    public static string ComputeHash(string password, string saltString)
    {
        var salt = Convert.FromBase64String(saltString);

        using var hashGenerator = new Rfc2898DeriveBytes(password, salt);
        hashGenerator.IterationCount = 10101;
        var bytes = hashGenerator.GetBytes(256 / 8);
        return Convert.ToBase64String(bytes);
    }
}