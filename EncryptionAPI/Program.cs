using System.Security.Cryptography;
using EncryptionAPI;
using EncryptionAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register services
builder.Services.AddSingleton<IEncryptionService, AesEncryptionService>();
builder.Services.AddSingleton<IKeyManagementService, KeyManagementService>();

// Configure SQLite
builder.Services.AddDbContext<ApplicationDbContext>(
    (serviceProvider, options) =>
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add endpoints
app.MapPost(
        "/users",
        async (
            ApplicationDbContext db,
            IEncryptionService encryptionService,
            IKeyManagementService keyManagement,
            string data
        ) =>
        {
            // Get master key from configuration
            var masterKey = builder.Configuration["Encryption:MasterKey"];
            if (string.IsNullOrEmpty(masterKey))
                return Results.Problem("Master key not configured");

            // Generate new key pair
            var (key, iv) = await keyManagement.GenerateKeyAsync();

            // Encrypt user data
            var encryptedData = encryptionService.Encrypt(data, key, iv);

            // Encrypt the key with master key
            var encryptedKey = keyManagement.EncryptKey(key, masterKey);

            var user = new User
            {
                EncryptedData = encryptedData,
                Key = new UserKey { EncryptedKey = encryptedKey, IV = iv }
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return Results.Created($"/users/{user.Id}", new { user.Id });
        }
    )
    .WithName("CreateUser")
    .WithOpenApi();

app.MapGet(
        "/users/{id}",
        async (
            ApplicationDbContext db,
            IEncryptionService encryptionService,
            IKeyManagementService keyManagement,
            int id
        ) =>
        {
            var masterKey = builder.Configuration["Encryption:MasterKey"];
            if (string.IsNullOrEmpty(masterKey))
                return Results.Problem("Master key not configured");

            var user = await db.Users.Include(u => u.Key).FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return Results.NotFound();

            // Decrypt the user's key using master key
            var decryptedKey = keyManagement.DecryptKey(user.Key.EncryptedKey, masterKey);

            // Decrypt the actual data
            var decryptedData = encryptionService.Decrypt(
                user.EncryptedData,
                decryptedKey,
                user.Key.IV
            );

            return Results.Ok(new { Data = decryptedData });
        }
    )
    .WithName("GetUser")
    .WithOpenApi();

app.MapGet(
        "/users/test-encryption",
        async (ApplicationDbContext db, IEncryptionService encryptionService) =>
        {
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.GenerateKey();
            aes.GenerateIV();

            string key = Convert.ToBase64String(aes.Key);
            string iv = Convert.ToBase64String(aes.IV);

            // Test encryption
            string testData = "Test sensitive data";
            string encrypted = encryptionService.Encrypt(testData, key, iv);
            string decrypted = encryptionService.Decrypt(encrypted, key, iv);

            return Results.Ok(
                new
                {
                    original = testData,
                    encrypted = encrypted,
                    decrypted = decrypted
                }
            );
        }
    )
    .WithName("TestEncryption")
    .WithOpenApi();

app.Run();
