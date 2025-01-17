namespace EncryptionAPI.Models;

public class User
{
    public int Id { get; set; }
    public string EncryptedData { get; set; } = string.Empty;

    public UserKey Key { get; set; } = null!;
}
