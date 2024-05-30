using ATONtest.DTO;
using ATONtest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ATONtest.Interfaces
{
    public interface IUsersController
    {
        //1
        Task<User> CreateUser(CreateUserDto model);
        //2
        Task<User> UpdateUserInfo(string login, UserInfoUpdateDto updateInfoModel);
        //3
        Task<User> UpdateUserPassword(string login, string currentPassword, string newPassword);
        //4
        Task<User> UpdateUserLogin(string currentLogin, string password, string newLogin);
        //5
        Task<IEnumerable<User>> GetActiveUsers();
        //6
        Task<User> CheckInfoByLogin(string login);
        //7
        Task<bool> ValidateUser(string username, string password);
        //8
        Task<IEnumerable<User>> GetUsersOlderThan(int age);
        //9
        Task SoftDelete(string login);
        Task HardDelete(string login);
        //10
        Task ClearRevokedFields(string login);
    }
}
