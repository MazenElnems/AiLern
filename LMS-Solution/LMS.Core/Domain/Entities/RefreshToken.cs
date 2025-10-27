namespace LMS.Shared.Domain.Entities
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime ExpiresOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? RevokedOn { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
        public bool IsActive => !IsExpired && RevokedOn is null;

        // Foreign Keys
        public int ApplicationUserId { get; set; }

        // Navigation property
        public ApplicationUser ApplicationUser { get; set; }
    }
}
