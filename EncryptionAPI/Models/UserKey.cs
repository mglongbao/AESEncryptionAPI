namespace EncryptionAPI.Models;

public class UserKey
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string EncryptedKey { get; set; } = string.Empty;
    public string IV { get; set; } = string.Empty;

    public User User { get; set; } = null!;
}
