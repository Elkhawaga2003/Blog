using System.ComponentModel.DataAnnotations;

namespace Blog.ApiModels.Auth
{
    public class LoginApiModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
