namespace Products.Api.Common.DTOs
{
    public class RegisterUserDto
    {
        public string UserName { get; set; } = null!;
        public string Email    { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role     { get; set; } = "User";
    }
}