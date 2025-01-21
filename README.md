# Secure Data Encryption API

A .NET Core API that demonstrates secure data encryption using AES-256 encryption with master key management. This project showcases how to implement encryption for sensitive data storage with a focus on security best practices.

## 🔐 Features

- AES-256 encryption for sensitive data
- Master key-based encryption key management
- RESTful API endpoints for data operations
- SQLite database for data persistence
- Swagger UI for API testing and documentation

## 📋 Prerequisites

- .NET 8.0 SDK or later
- SQLite
- An IDE (Visual Studio, VS Code, or Rider)

## 🚀 Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/mglongbao/AESEncryptionAPI
cd AESEncryptionAPI
```

### 2. Generate and Configure Master Key

Create a new console application to generate your master key:

```csharp
using System.Security.Cryptography;

Aes aes = Aes.Create();
Console.WriteLine(Convert.ToBase64String(aes.Key));
```

Copy the generated base64 string and update it in `appsettings.json`:

```json
{
  "Encryption": {
    "MasterKey": "your-generated-base64-key"
  }
}
```

### 3. Setup and Run

```bash
dotnet restore
dotnet run
```

The API will be available at:

- Swagger UI: http://localhost:5062/swagger/index.html
- API Base URL: http://localhost:5062

## 📝 API Usage

### 1. Create Encrypted Data

```http
POST /users
Content-Type: text/plain

Request Body: "Your sensitive data here"
```

Response: Returns user ID for future retrieval

### 2. Retrieve Decrypted Data

```http
GET /users/{id}
```

Response: Returns original decrypted data

### 3. Test Encryption

```http
GET /users/test-encryption
```

Response: Shows encryption/decryption process with test data

## 🔒 How It Works

1. **Data Encryption Flow**:

   ```
   User Data → Generate Unique Key → Encrypt Data → Encrypt Key with Master Key → Store
   ```

2. **Data Retrieval Flow**:
   ```
   Retrieve Encrypted Data → Decrypt Key using Master Key → Decrypt Data → Return
   ```

## 🏗️ Project Structure

- `AesEncryptionService.cs`: Core encryption/decryption logic
- `KeyManagementService.cs`: Handles key generation and management
- `User.cs` & `UserKey.cs`: Data models
- `Program.cs`: API endpoints and configuration
- `ApplicationDbContext.cs`: Database context

## ⚠️ Security Notes

1. **Development vs Production**

   - The current setup with master key in appsettings.json is for demonstration only
   - For production:
     - Use Azure Key Vault or AWS KMS
     - Implement key rotation
     - Add proper authentication
     - Enable HTTPS only

2. **Data Protection**
   - Each user's data is encrypted with a unique key
   - Keys are encrypted with the master key
   - IVs are randomly generated for each encryption

## 🧪 Testing

1. **Using Swagger UI**

   - Navigate to `/swagger`
   - Try the test-encryption endpoint
   - Create and retrieve encrypted data

## 📦 Dependencies

```xml
 <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
 <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0" />
 <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.0" />
```

## 🔍 Common Issues

1. **Invalid Master Key**

   - Ensure the master key is a valid base64 string
   - Use the provided console application to generate keys

2. **Database Issues**
   - Ensure SQLite database file has write permissions
   - Check connection string in appsettings.json

## 📚 Additional Resources

- [AES Encryption in .NET](https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes)
- [Best Practices for Key Management](https://docs.microsoft.com/en-us/azure/key-vault/general/best-practices)
