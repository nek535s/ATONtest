using ATONtest.Data;
using ATONtest.DTO;
using ATONtest.Interfaces;
using ATONtest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace ATONtest.Services
{
    public class UserControllerService : IUsersController
    {
        private DbUsersContext _context;
       
        public UserControllerService(DbUsersContext context )
        {
            _context = context;
           
        }

        public async Task<User> CreateUser(CreateUserDto model)
        {
            if (await _context.Users.AnyAsync(u => u.Login == model.Login))
            {
                throw new Exception("Логин уже существует.");
            }
            if (model.Gender < 0 || model.Gender >= 2)
            {
                throw new Exception("Пол должен быть от 0 до 2");
            }

            var user = new User
            {
                Login = model.Login,
                Password = model.Password,
                Name = model.Name,
                Gender = model.Gender,
                Birthday = model.Birthday,
                Admin = model.Admin,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            };
            //if (user.Admin == true)
            //{
            //    await _roleManager.SetRoleNameAsync(, "Admin");
            //}
            //else
            //{
            //    await _roleManager.SetRoleNameAsync(, "User");
            //}

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }
        public async Task<User> UpdateUserInfo(string login, UserInfoUpdateDto updateInfoModel)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login && u.RevokedOn == null);

            if (user == null)
            {
                throw new KeyNotFoundException("Не найден пользователь либо неактивен.");
            }

            user.Name = updateInfoModel.Name;
            user.Gender = updateInfoModel.Gender ?? user.Gender;
            user.Birthday = updateInfoModel.Birthday ?? user.Birthday;
            user.ModifiedOn = DateTime.Now;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return user;
        }
        public async Task<User> UpdateUserPassword(string login, string currentPassword, string newPassword)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == login && u.Password == currentPassword && u.RevokedOn == null);

            if (user == null)
            {
                throw new KeyNotFoundException("Не найден пользователь либо неактивен.");
            }

            user.Password = newPassword;
            user.ModifiedOn = DateTime.Now;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return user;
        }
        public async Task<User> UpdateUserLogin(string currentLogin, string password, string newLogin)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == currentLogin && u.Password == password && u.RevokedOn == null);

            if (user == null)
            {
                throw new ArgumentException("Не найден пользователь, либо учётная запись неактивна, или неверный пароль.");
            }

            if (await _context.Users.AnyAsync(u => u.Login == newLogin))
            {
                throw new ArgumentException("Логин уже используется другим пользователем.");
            }

            user.Login = newLogin;
            user.ModifiedOn = DateTime.Now;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return user;
        }
        public async Task<IEnumerable<User>> GetActiveUsers()
        {
            var users = await _context.Users
                .Where(u => u.RevokedOn == null)
                .OrderBy(u => u.CreatedOn)
                .ToListAsync();

            return users;
        }
        public async Task<User> CheckInfoByLogin(string login)
        {
            var info = await _context.Users
                .Select(u => new User
                {
                    Login = u.Login,
                    Name = u.Name,
                    Gender = u.Gender,
                    Birthday = u.Birthday,
                    RevokedOn = u.RevokedOn,
                    RevokedBy = u.RevokedBy
                })
                .FirstOrDefaultAsync(u => u.Login == login);

            return info;
        }
        public async Task<bool> ValidateUser(string username, string password)
        {
            return await _context.Users.AnyAsync(u => u.Login == username && password == u.Password && u.RevokedOn != DateTime.Today);
        }
        public async Task<IEnumerable<User>> GetUsersOlderThan(int age)
        {
            DateTime olderThanDate = DateTime.Today.AddYears(-age);
            var users = await _context.Users.Where(u => u.Birthday <= olderThanDate).ToListAsync();
            return users;
        }
        public async Task SoftDelete(string login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
            if (user != null)
            {
                user.RevokedOn = DateTime.Now;
                user.RevokedBy = "Admin";
                user.ModifiedOn = DateTime.Now;
                user.ModifiedBy = "Admin";
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }
        public async Task HardDelete(string login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
        public async Task ClearRevokedFields(string login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
            if (user != null)
            {
                user.RevokedOn = null;
                user.RevokedBy = null;
                user.ModifiedBy = "Admin";
                user.ModifiedOn = DateTime.Now;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Пользователь не найден.");
            }
        }
    }
}

