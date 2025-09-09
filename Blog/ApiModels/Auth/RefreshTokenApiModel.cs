using System.ComponentModel.DataAnnotations;

namespace Blog.ApiModels.Auth
{
    public class RefreshTokenApiModel
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
