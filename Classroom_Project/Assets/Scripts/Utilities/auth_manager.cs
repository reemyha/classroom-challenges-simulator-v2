using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


/// <summary>
/// Manages user authentication with MongoDB database.
/// Handles login, password verification, and session management.
/// </summary>
public class AuthenticationManager : MonoBehaviour
{
    [Header("MongoDB Connection")]
    [Tooltip("MongoDB connection string - example: mongodb://localhost:27017")]
    public string connectionString = "mongodb://localhost:27017";

    [Header("Dev / Offline Mode")]
    public bool allowOfflineLoginIfMongoFails = true;
    private bool mongoReady = false;

    [Tooltip("Name of the database")]
    public string databaseName = "ClassroomSimulator";
    
    [Tooltip("Name of the users collection")]
    public string usersCollectionName = "users";

    [Header("Current Session")]
    public UserModel currentUser;
    public bool isLoggedIn = false;

    // MongoDB objects
    private IMongoClient mongoClient;
    private IMongoDatabase database;
    private IMongoCollection<UserModel> usersCollection;

    // Singleton pattern - only one AuthenticationManager exists
    public static AuthenticationManager Instance { get; private set; }

    void Awake()
    {
        // Implement singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist between scenes
    }

    void Start()
    {
        InitializeMongoDB();
    }

    /// <summary>
    /// Connect to MongoDB database
    /// </summary>
    void InitializeMongoDB()
    {
        try
        {
            Debug.Log("Connecting to MongoDB...");
            
            // Create MongoDB client
            mongoClient = new MongoClient(connectionString);
            
            // Get database
            database = mongoClient.GetDatabase(databaseName);
            
            // Get users collection
            usersCollection = database.GetCollection<UserModel>(usersCollectionName);
            
            Debug.Log("MongoDB connection successful!");
            
            // Create default admin user if database is empty
            //CreateDefaultAdminIfNeeded();
            //mongoReady = true;

            mongoReady = true;

            // רק אם יש חיבור אמיתי - ננסה ליצור אדמין
            CreateDefaultAdminIfNeeded();


        }
        catch (Exception e)
        {
            Debug.LogError($"MongoDB connection failed: {e.Message}");
            Debug.LogError("Make sure MongoDB is running on your machine!");
            mongoReady = false;

        }

    }

    /// <summary>
    /// Create a default admin user if no users exist
    /// This is helpful for first-time setup
    /// </summary>
    async void CreateDefaultAdminIfNeeded()
    {
        try
        {
            long userCount = await usersCollection.CountDocumentsAsync(new BsonDocument());
            
            if (userCount == 0)
            {
                Debug.Log("No users found. Creating default admin user...");
                
                UserModel admin = new UserModel
                {
                    Username = "admin",
                    PasswordHash = HashPassword("admin123"),
                    Email = "admin@classroom.sim",
                    FullName = "System Administrator",
                    Role = UserRole.Administrator,
                    IsActive = true
                };
                
                await usersCollection.InsertOneAsync(admin);
                Debug.Log("Default admin created! Username: admin, Password: admin123");
                Debug.Log("⚠️ IMPORTANT: Change this password in production!");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error creating default admin: {e.Message}");
        }
    }

    /// <summary>
    /// Attempt to log in a user
    /// </summary>
    public async Task<LoginResponse> LoginAsync(string username, string password)
    {
        if (!mongoReady || usersCollection == null)
        {
            if (allowOfflineLoginIfMongoFails)
            {
                // Dev login – allow admin/admin123 locally
                bool ok = (username == "admin" && password == "admin123");
                return new LoginResponse
                {
                    Success = ok,
                    Message = ok ? "Offline login successful" : "Offline login failed",
                    User = ok ? new UserModel { Username = "admin", FullName = "Offline Admin", Role = UserRole.Administrator, IsActive = true } : null,
                    SessionToken = ok ? GenerateSessionToken(new UserModel { Username = "admin" }) : null
                };
            }

            return new LoginResponse { Success = false, Message = "Database unavailable" };
        }

        LoginResponse response = new LoginResponse();

        try
        {
            // Validate input
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                response.Success = false;
                response.Message = "Username and password are required";
                return response;
            }

            Debug.Log($"Attempting login for user: {username}");

            // Find user in database
            var filter = Builders<UserModel>.Filter.Eq(u => u.Username, username);
            UserModel user = await usersCollection.Find(filter).FirstOrDefaultAsync();

            // Check if user exists
            if (user == null)
            {
                response.Success = false;
                response.Message = "Invalid username or password";
                Debug.Log("User not found");
                return response;
            }

            // Check if account is active
            if (!user.IsActive)
            {
                response.Success = false;
                response.Message = "Account is disabled. Contact administrator.";
                Debug.Log("Account is disabled");
                return response;
            }

            // Verify password
            string passwordHash = HashPassword(password);
            if (user.PasswordHash != passwordHash)
            {
                response.Success = false;
                response.Message = "Invalid username or password";
                Debug.Log("Password incorrect");
                return response;
            }

            // Login successful!
            Debug.Log($"Login successful for {username}");

            // Update last login time and session count
            var update = Builders<UserModel>.Update
                .Set(u => u.LastLogin, DateTime.Now)
                .Inc(u => u.SessionCount, 1);
            
            await usersCollection.UpdateOneAsync(filter, update);

            // Store current user
            currentUser = user;
            isLoggedIn = true;

            // Prepare response
            response.Success = true;
            response.Message = "Login successful!";
            response.User = user;
            response.SessionToken = GenerateSessionToken(user);

            return response;
        }
        catch (Exception e)
        {
            Debug.LogError($"Login error: {e.Message}");
            response.Success = false;
            response.Message = "Login failed. Please try again.";
            return response;
        }
    }

    /// <summary>
    /// Create a new user account
    /// </summary>
    public async Task<bool> RegisterUserAsync(string username, string password, string email, string fullName, UserRole role = UserRole.Student)
    {
        try
        {
            // Check if username already exists
            var filter = Builders<UserModel>.Filter.Eq(u => u.Username, username);
            long existingUsers = await usersCollection.CountDocumentsAsync(filter);

            if (existingUsers > 0)
            {
                Debug.LogWarning($"Username '{username}' already exists");
                return false;
            }

            // Create new user
            UserModel newUser = new UserModel
            {
                Username = username,
                PasswordHash = HashPassword(password),
                Email = email,
                FullName = fullName,
                Role = role,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            // Insert into database
            await usersCollection.InsertOneAsync(newUser);
            Debug.Log($"User '{username}' registered successfully");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Registration error: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// Log out current user
    /// </summary>
    public void Logout()
    {
        currentUser = null;
        isLoggedIn = false;
        Debug.Log("User logged out");
    }

    /// <summary>
    /// Hash password using SHA256
    /// IMPORTANT: In production, use more secure hashing like bcrypt!
    /// </summary>
    private string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }

    /// <summary>
    /// Generate a session token for the user
    /// </summary>
    private string GenerateSessionToken(UserModel user)
    {
        // Simple token: username + timestamp + random number, then hashed
        string tokenData = $"{user.Username}_{DateTime.Now.Ticks}_{UnityEngine.Random.Range(1000, 9999)}";
        return HashPassword(tokenData);
    }

    /// <summary>
    /// Check if user has permission to access a scenario
    /// </summary>
    public bool CanAccessScenario(string scenarioName)
    {
        if (!isLoggedIn || currentUser == null)
            return false;

        return currentUser.CanAccessScenario(scenarioName);
    }

    /// <summary>
    /// Get all users (admin only)
    /// </summary>
    public async Task<UserModel[]> GetAllUsersAsync()
    {
        if (currentUser == null || currentUser.Role != UserRole.Administrator)
        {
            Debug.LogWarning("Only administrators can view all users");
            return new UserModel[0];
        }

        try
        {
            var users = await usersCollection.Find(new BsonDocument()).ToListAsync();
            return users.ToArray();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error fetching users: {e.Message}");
            return new UserModel[0];
        }
    }
}