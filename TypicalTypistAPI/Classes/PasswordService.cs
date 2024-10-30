namespace TypicalTypistAPI.Classes
{
    public class PasswordService
    {
        public string HashPassword(string textPassword)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(textPassword);
            return hashedPassword;
        }

        public bool VerifyPassword(string textPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(textPassword, hashedPassword);
        }
    }
}
