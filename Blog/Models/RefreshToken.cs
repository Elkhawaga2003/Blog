using Microsoft.EntityFrameworkCore;

namespace Blog.Models
{
    [Owned]
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ReVokedOn { get; set; }
        public DateTime ExpireOn { get; set; }
        public bool IsActive => ReVokedOn == null && !IsExpired;
        public bool IsExpired => ExpireOn <= DateTime.UtcNow;
    }
}
