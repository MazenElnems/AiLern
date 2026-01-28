namespace LMS.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresOn { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? RevokesOn { get; set; }
    public int UserId { get; set; } 
    public bool IsActive => RevokesOn == null && ExpiresOn > DateTime.UtcNow;

    // Navigation Property
    public ApplicationUser User { get; set; }
}
