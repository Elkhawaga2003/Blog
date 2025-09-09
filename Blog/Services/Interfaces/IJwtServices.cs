using Blog.ApiModels.Auth;

namespace Blog.Services.Interfaces
{
    public interface IJwtServices
    {
        public Task<AuthApiModel> Register(RegisterApiModel model);
        public Task<AuthApiModel> GetToken(LoginApiModel model);
        public Task<AuthApiModel> RefreshToken(string refreshToken);
        public Task<bool> RevokeToken(string token);
    }
}
