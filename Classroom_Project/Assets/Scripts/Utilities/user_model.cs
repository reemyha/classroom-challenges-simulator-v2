using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

/// <summary>
/// Represents a user in the system.
/// This matches the structure in MongoDB database.
/// Each user has credentials, role, and session history.
/// </summary>
[Serializable]
public class UserModel
{
    // MongoDB automatically creates this ID
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    // Login credentials
    [BsonElement("username")]
    public string Username { get; set; }

    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } // We store hashed passwords, never plain text!

    // User information
    [BsonElement("email")]
    public string Email { get; set; }

    [BsonElement("fullName")]
    public string FullName { get; set; }

    // Role determines what user can do
    [BsonElement("role")]
    public UserRole Role { get; set; }

    // Tracking
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("lastLogin")]
    public DateTime LastLogin { get; set; }

    [BsonElement("sessionCount")]
    public int SessionCount { get; set; }

    // Permissions
    [BsonElement("allowedScenarios")]
    public string[] AllowedScenarios { get; set; } // Which scenarios this user can access

    [BsonElement("isActive")]
    public bool IsActive { get; set; } // Can this user log in?

    /// <summary>
    /// Create a new user with default values
    /// </summary>
    public UserModel()
    {
        CreatedAt = DateTime.Now;
        IsActive = true;
        SessionCount = 0;
        Role = UserRole.Student;
        AllowedScenarios = new string[] { }; // Empty means all scenarios allowed
    }

    /// <summary>
    /// Check if this user can access a specific scenario
    /// </summary>
    public bool CanAccessScenario(string scenarioName)
    {
        // If AllowedScenarios is empty or null, user can access all scenarios
        if (AllowedScenarios == null || AllowedScenarios.Length == 0)
            return true;

        // Check if scenario is in allowed list
        foreach (string allowed in AllowedScenarios)
        {
            if (allowed.Equals(scenarioName, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}

/// <summary>
/// Different user roles with different permissions
/// </summary>
public enum UserRole
{
    Student,        // Regular trainee teacher
    Instructor,     // Can view all student sessions
    Administrator   // Can create users and manage system
}

/// <summary>
/// Data structure for login attempts
/// </summary>
[Serializable]
public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

/// <summary>
/// Data returned after successful login
/// </summary>
[Serializable]
public class LoginResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public UserModel User { get; set; }
    public string SessionToken { get; set; } // For maintaining login state
}