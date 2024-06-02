using ATONtest.Data;
using ATONtest.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ATONtest.Services
{
    
    public class AuthService
    {
        private readonly DbUsersContext _context;

        public AuthService(DbUsersContext context)
        {
            _context = context;
        }

        public User ValidateUser(string login, string password)
        {
            // Здесь должна быть логика для шифрования/проверки пароля,
            // если вы храните пароли в зашифрованном виде.
            return _context.Users
                .FirstOrDefault(u => u.Login == login && u.Password == password);
        }
    }
}
