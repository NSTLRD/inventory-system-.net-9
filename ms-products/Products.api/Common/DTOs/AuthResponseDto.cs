namespace Products.Api.Common.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public DateTime Expires { get; set; }
    }
}