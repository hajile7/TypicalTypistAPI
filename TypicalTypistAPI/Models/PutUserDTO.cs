namespace TypicalTypistAPI.Models
{
    public class PutUserDTO
    {
        public string? FirstName { get; set; } = null!;
        public string? LastName { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public string? UserName { get; set; } = null!;
    }
}
