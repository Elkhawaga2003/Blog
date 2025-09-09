using System.ComponentModel.DataAnnotations;

namespace Blog.ApiModels.Auth
{
    public class RegisterApiModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]

        public string Name { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string ConfirmPassword { get; set; }
        public IFormFile? Image { get; set; }
    }
}
