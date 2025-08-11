using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChronoVoid.API.Data;
using ChronoVoid.API.Models;
using ChronoVoid.API.DTOs;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ChronoVoid.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ChronoVoidContext _context;

    public AuthController(ChronoVoidContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        // Validate username
        var usernameValidation = ValidateUsername(request.Username);
        if (!usernameValidation.IsValid)
        {
            return BadRequest(usernameValidation.ErrorMessage);
        }

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains("@"))
        {
            return BadRequest("Please provide a valid email address");
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
        {
            return BadRequest("Password must be at least 6 characters long");
        }

        // Check if username already exists (case-insensitive)
        var existingUsername = await _context.Users
            .FirstOrDefaultAsync(u => u.Username.ToLower() == request.Username.ToLower());
        if (existingUsername != null)
        {
            return BadRequest($"Username '{request.Username}' is already taken");
        }

        // Check if email already exists (case-insensitive)
        var existingEmail = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());
        if (existingEmail != null)
        {
            return BadRequest($"Email '{request.Email}' is already registered");
        }

        // Hash password
        var passwordHash = HashPassword(request.Password);

        // Create new user
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            CargoHolds = 300 // Default cargo holds
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var response = new AuthResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            CurrentNodeId = user.CurrentNodeId,
            RealmId = user.RealmId,
            CargoHolds = user.CargoHolds,
            LastLogin = user.LastLogin,
            Token = GenerateToken(user.Id) // Simple token for now
        };

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        // Allow login with either username or email
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Username);

        if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
        {
            return BadRequest("Invalid username or password");
        }

        // Update last login
        user.LastLogin = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var response = new AuthResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            CurrentNodeId = user.CurrentNodeId,
            RealmId = user.RealmId,
            CargoHolds = user.CargoHolds,
            LastLogin = user.LastLogin,
            Token = GenerateToken(user.Id)
        };

        return Ok(response);
    }

    [HttpPost("join-realm")]
    public async Task<ActionResult<JoinRealmResponse>> JoinRealm(JoinRealmRequest request)
    {
        var user = await _context.Users.FindAsync(request.UserId);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var realm = await _context.NexusRealms.FindAsync(request.RealmId);
        if (realm == null)
        {
            return NotFound("Realm not found");
        }

        // Check if user is already in this realm
        if (user.RealmId == request.RealmId && user.CurrentNodeId.HasValue)
        {
            // User is already in this realm, get their current node
            var currentNode = await _context.NeuralNodes
                .Include(n => n.OutgoingTunnels)
                .ThenInclude(t => t.ToNode)
                .Include(n => n.Realm)
                .FirstOrDefaultAsync(n => n.Id == user.CurrentNodeId);

            if (currentNode != null)
            {
                var nodeDetail = new NodeDetailDto
                {
                    Id = currentNode.Id,
                    NodeNumber = currentNode.NodeNumber,
                    RealmName = currentNode.Realm.Name,

                    HasQuantumStation = currentNode.HasQuantumStation,
                    StarName = currentNode.StarName,
                    PlanetCount = currentNode.PlanetCount,
                    ConnectedNodes = currentNode.OutgoingTunnels.Select(t => new ConnectedNodeDto
                    {
                        NodeId = t.ToNodeId,
                        NodeNumber = t.ToNode.NodeNumber,
                        HasQuantumStation = t.ToNode.HasQuantumStation,

                        StarName = t.ToNode.StarName,
                        PlanetCount = t.ToNode.PlanetCount
                    }).ToList()
                };

                return Ok(new JoinRealmResponse
                {
                    Success = true,
                    Message = $"Welcome back to {realm.Name}! You are at Node {currentNode.NodeNumber}.",
                    StartingNodeId = currentNode.Id,
                    StartingNode = nodeDetail
                });
            }
        }

        // User is new to this realm, assign them to Node 1
        var startingNode = await _context.NeuralNodes
            .Include(n => n.OutgoingTunnels)
            .ThenInclude(t => t.ToNode)
            .Include(n => n.Realm)
            .FirstOrDefaultAsync(n => n.RealmId == request.RealmId && n.NodeNumber == 1);

        if (startingNode == null)
        {
            return BadRequest("Realm has no starting node (Node 1)");
        }

        // Update user's realm and location
        user.RealmId = request.RealmId;
        user.CurrentNodeId = startingNode.Id;
        user.CargoHolds = 300; // Reset cargo holds for new realm
        await _context.SaveChangesAsync();

        var startingNodeDetail = new NodeDetailDto
        {
            Id = startingNode.Id,
            NodeNumber = startingNode.NodeNumber,
            RealmName = startingNode.Realm.Name,

            HasQuantumStation = startingNode.HasQuantumStation,
            StarName = startingNode.StarName,
            PlanetCount = startingNode.PlanetCount,
            ConnectedNodes = startingNode.OutgoingTunnels.Select(t => new ConnectedNodeDto
            {
                NodeId = t.ToNodeId,
                NodeNumber = t.ToNode.NodeNumber,
                HasQuantumStation = t.ToNode.HasQuantumStation,

                StarName = t.ToNode.StarName,
                PlanetCount = t.ToNode.PlanetCount
            }).ToList()
        };

        return Ok(new JoinRealmResponse
        {
            Success = true,
            Message = $"Welcome to {realm.Name}! Your ship has been created with 300 cargo holds. You start at Node 1.",
            StartingNodeId = startingNode.Id,
            StartingNode = startingNodeDetail
        });
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "ChronoVoidSalt"));
        return Convert.ToBase64String(hashedBytes);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        var passwordHash = HashPassword(password);
        return passwordHash == hash;
    }

    private static string GenerateToken(int userId)
    {
        // Simple token for now - in production, use JWT
        return Convert.ToBase64String(Encoding.UTF8.GetBytes($"ChronoVoid_{userId}_{DateTime.UtcNow.Ticks}"));
    }

    private static (bool IsValid, string ErrorMessage) ValidateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return (false, "Username is required");
        }

        if (username.Length < 3)
        {
            return (false, "Username must be at least 3 characters long");
        }

        if (username.Length > 20)
        {
            return (false, "Username must be 20 characters or less");
        }

        // Check for valid characters (alphanumeric, underscore, hyphen)
        if (!System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9_-]+$"))
        {
            return (false, "Username can only contain letters, numbers, underscores, and hyphens");
        }

        // TODO: Add inappropriate content filtering here
        // For now, just basic validation
        var lowercaseUsername = username.ToLower();
        
        // Basic inappropriate content check (placeholder)
        string[] bannedWords = { "admin", "system", "null", "undefined", "test" };
        if (bannedWords.Any(word => lowercaseUsername.Contains(word)))
        {
            return (false, "Username contains restricted words");
        }

        return (true, string.Empty);
    }

    [HttpGet("debug/users")]
    public async Task<ActionResult> GetAllUsersDebug()
    {
        var users = await _context.Users
            .Select(u => new { u.Id, u.Username, u.Email, u.CreatedAt })
            .ToListAsync();
        
        return Ok(new { 
            Count = users.Count, 
            Users = users 
        });
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        // Find user by email or username
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.EmailOrUsername || u.Username == request.EmailOrUsername);

        if (user == null)
        {
            return BadRequest("User not found");
        }

        // For now, just reset to a temporary password (no security verification)
        // TODO: Add proper email verification in the future
        string tempPassword = "temp123";
        user.PasswordHash = HashPassword(tempPassword);
        await _context.SaveChangesAsync();

        return Ok(new { 
            Message = "Password has been reset to 'temp123'. Please login and change your password.",
            TempPassword = tempPassword 
        });
    }

    [HttpGet("validate-users")]
    public async Task<ActionResult> ValidateUsers()
    {
        var invalidUsers = await _context.Users
            .Where(u => string.IsNullOrEmpty(u.Username) || u.Username.Length < 3)
            .Select(u => new { u.Id, u.Username, u.Email, u.CreatedAt })
            .ToListAsync();

        return Ok(new {
            InvalidUserCount = invalidUsers.Count,
            InvalidUsers = invalidUsers,
            ValidationRules = new {
                MinUsernameLength = 3,
                RequiredFields = new[] { "Username" }
            }
        });
    }

    [HttpPost("debug/test-password")]
    public async Task<ActionResult> TestPassword([FromBody] LoginRequest request)
    {
        string username = request.Username;
        string password = request.Password;
        
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username || u.Email == username);
            
        if (user == null)
        {
            return NotFound("User not found");
        }
        
        var inputHash = HashPassword(password);
        var storedHash = user.PasswordHash;
        var matches = VerifyPassword(password, storedHash);
        
        return Ok(new {
            Username = user.Username,
            Email = user.Email,
            InputPassword = password,
            InputHash = inputHash,
            StoredHash = storedHash,
            PasswordMatches = matches
        });
    }
}