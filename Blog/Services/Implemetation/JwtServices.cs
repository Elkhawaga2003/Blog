using Blog.Helpers;
using Blog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using static Blog.Services.Implemetation.JwtServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blog.Services.Interfaces;
using Blog.ApiModels.Auth;
using Microsoft.EntityFrameworkCore;

namespace Blog.Services.Implemetation
{
    
        public class JwtServices : IJwtServices
        {
            #region Fields
            private readonly UserManager<User> _userManger;
            private readonly JwtOptions _jwtOptions;
        private readonly IFileServices _fileServices;

        public JwtServices(IFileServices fileServices, UserManager<User> userManager, IOptions<JwtOptions> jwtOptions)
            {
            _fileServices = fileServices;

            _userManger = userManager;
                _jwtOptions = jwtOptions.Value;
            }

            #endregion
           
            public async Task<AuthApiModel> Register(RegisterApiModel model)
            {
                var user = await _userManger.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    return new AuthApiModel
                    {
                        Message = "User already exists"
                    };
                }
            user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                };
            if (model.Image != null)
            {
                var filePath = await _fileServices.UploadAsync(model.Image, "/ProfileImage/");
                if (filePath.StartsWith("/ProfileImage/"))
                {
                    user.ImageUrl = filePath;
                }
            }
                var result = await _userManger.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    return new AuthApiModel
                    {
                        Message = string.Join(", ", result.Errors.Select(e => e.Description))
                    };
                }
                await _userManger.AddToRoleAsync(user, "User");
                var roles = await _userManger.GetRolesAsync(user);
                var auth = new AuthApiModel();
                auth.Email = user.Email;
                auth.UserName = user.UserName;
                auth.IsAuthenticated = true;
                auth.Roles = new List<string> { "User" };
                auth.Token = new JwtSecurityTokenHandler().WriteToken(await CreateToken(user));
                return auth;
            }

            public async Task<AuthApiModel> GetToken(LoginApiModel model)
            {
                var auth = new AuthApiModel();
                var user = await _userManger.FindByEmailAsync(model.Email);
                if (user == null || !await _userManger.CheckPasswordAsync(user, model.Password))
                {
                    auth.Message = "invalid email or password";
                    return auth;
                }
                var roles = await _userManger.GetRolesAsync(user);
                auth.Email = user.Email;
                auth.UserName = user.UserName;
                auth.IsAuthenticated = true;
                auth.Roles = roles.ToList();
                auth.Token = new JwtSecurityTokenHandler().WriteToken(await CreateToken(user));
                if (user.RefreshTokens.Any(Rtoken => Rtoken.IsActive))
                {
                    var refreshToken = user.RefreshTokens.FirstOrDefault(Rtoken => Rtoken.IsActive);
                    auth.RefreshToken = refreshToken.Token;
                    auth.RefreshTokenExpiration = refreshToken.ExpireOn;
                }
                else
                {
                    var refreshToken = CreateRefreshToken();
                    user.RefreshTokens.Add(refreshToken);
                    await _userManger.UpdateAsync(user);
                    auth.RefreshToken = refreshToken.Token;
                    auth.RefreshTokenExpiration = refreshToken.ExpireOn;
                }
                return auth;
            }

            public async Task<AuthApiModel> RefreshToken(string refreshToken)
            {
                var user = await _userManger.Users.FirstOrDefaultAsync(u => u.RefreshTokens.Any(r => r.Token == refreshToken));
                if (user == null || !user.RefreshTokens.Any(r => r.Token == refreshToken && r.IsActive))
                {
                    return new AuthApiModel
                    {
                        Message = "Invalid refresh token"
                    };
                }
                var token = user.RefreshTokens.FirstOrDefault(r => r.Token == refreshToken && r.IsActive);
                token.ReVokedOn = DateTime.UtcNow;
                var newRefreshToken = CreateRefreshToken();
                user.RefreshTokens.Add(newRefreshToken);
                await _userManger.UpdateAsync(user);
                var jwtSecurityToken = await CreateToken(user);
                var userRoles = await _userManger.GetRolesAsync(user);
                var auth = new AuthApiModel
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    IsAuthenticated = true,
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                    RefreshToken = newRefreshToken.Token,
                    RefreshTokenExpiration = newRefreshToken.ExpireOn,
                    Roles = userRoles.ToList(),
                };
                return auth;
            }

            public async Task<bool> RevokeToken(string token)
            {
                var user = await _userManger.Users.FirstOrDefaultAsync(u => u.RefreshTokens.Any(r => r.Token == token));
                if (user == null)
                {
                    return false;
                }
                var refreshToken = user.RefreshTokens.FirstOrDefault(r => r.Token == token && r.IsActive);
                if (refreshToken == null)
                {
                    return false;
                }
                refreshToken.ReVokedOn = DateTime.UtcNow;
                return true;
            }
      
        #region CreateToken 
        private async Task<JwtSecurityToken> CreateToken(User user)
            {
                var userClaims = await _userManger.GetClaimsAsync(user);
                var userRoles = await _userManger.GetRolesAsync(user);
                var Rolesclaims = new List<Claim>();
                foreach (var role in userRoles)
                {
                    Rolesclaims.Add(new Claim(ClaimTypes.Role, role));
                }
                var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            }.Union(userClaims).Union(Rolesclaims);
                var jwtSecurityToken = new JwtSecurityToken(

                    issuer: _jwtOptions.Issuer,
                     audience: _jwtOptions.Audience,
                     claims: claims,
                     expires: DateTime.UtcNow.AddDays(_jwtOptions.Expiration),
                     signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(
                         new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwtOptions.Key)),
                         Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256)
                     );
                return jwtSecurityToken;
            }
            #endregion
            #region CreateRefreshToken
            private RefreshToken CreateRefreshToken()
            {
                var randomNumber = new byte[32];
                using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomNumber);
                    return new RefreshToken
                    {
                        ExpireOn = DateTime.UtcNow.AddDays(7),
                        Token = Convert.ToBase64String(randomNumber),
                        CreatedOn = DateTime.UtcNow
                    };
                }
            }


            #endregion
        }
    }

