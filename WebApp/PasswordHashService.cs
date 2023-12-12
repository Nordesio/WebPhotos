namespace WebApp
{

    public class PasswordHashService : IPasswordHashService
    {
        public string HashPassword(string password)
        {
            // Генерация соли для уникальности хеша
            string salt = BCrypt.Net.BCrypt.GenerateSalt(12); // 12 - количество раундов хеширования

            // Хеширование пароля с использованием соли
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

            return hashedPassword;
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
