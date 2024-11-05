namespace TypicalTypistAPI.Models
{
    public class PostUserDTO
    {
        public string FirstName { get; set; } = null!;
        public DateTime Joined { get; set; }
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
