using System.Text.Json.Serialization;

namespace Blog.ApiModels.Auth
{
    public class AuthApiModel
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public List<string> Roles { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Message { get; set; }
        [JsonIgnore]
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
