using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ATONtest.Data;
using ATONtest.Models;
using ATONtest.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Any;
using ATONtest.Services;
using ATONtest.Interfaces;

namespace ATONtest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DbUsersContext _context;
        private readonly IUsersController _userscontext;

        public UsersController(DbUsersContext context, IUsersController userscontext)
        {
            _context = context;
            _userscontext = userscontext;
        }
        // 1) Создание пользователя по логину, паролю, имени, полу и дате рождения + указание будет ли пользователь админом(Доступно Админам)  
        [HttpPost("CreateUsers")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto model)
        {
            try
            {
            var createduser = await _userscontext.CreateUser(model);
            return Ok(createduser);
            }
            catch(Exception ex)
            {
            return BadRequest(ex.Message);
            }       
        }
        //2) Изменение имени, пола или даты рождения пользователя(Может менять Администратор, либо лично пользователь, если он активен (отсутствует RevokedOn))
        [HttpPatch("UpdateUserInfo/{login}")]
        public async Task<IActionResult> UpdateUser(string login, [FromBody] UserInfoUpdateDto updateInfoModel)
        {
            try
            {
                var updatedUser = await _userscontext.UpdateUserInfo(login, updateInfoModel);
                return Ok(updatedUser);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }    
        //3) Изменение пароля(Пароль может менять либо Администратор, либо лично пользователь, если он активен (отсутствует RevokedOn))
        [HttpPatch("UpdatePass/{login}")]
        public async Task<IActionResult> UpdateUserPassword(string login, string pass, [FromBody] UpdatePassDTO updatePassDTO)
        {
            try
            {
                var updatedUser = await _userscontext.UpdateUserPassword(login, pass, updatePassDTO.Password);
                return Ok(updatedUser);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }  
        //4) Изменение логина(Логин может менять либо Администратор, либо лично пользователь, если он активен (отсутствует RevokedOn), логин должен оставаться уникальным)
        [HttpPatch("UpdateLogin/{login}")]
        public async Task<IActionResult> UpdateUserLogin(string login, string pass, [FromBody] UpdateLoginDTO updateLoginDTO)
        {
            try
            {
                var updatedUser = await _userscontext.UpdateUserLogin(login, pass, updateLoginDTO.Login);
                return Ok(updatedUser);
            }
            catch (ArgumentException ex)
            {
                return ex.Message.Contains("уже используется") ? BadRequest(ex.Message) : NotFound(ex.Message);
            }
        }
        //5) Запрос списка всех активных (отсутствует RevokedOn) пользователей, список отсортирован по CreatedOn(Доступно Админам)          
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<User>>> GetActiveUsers()
        {
            var users = await _userscontext.GetActiveUsers();
            return Ok(users);
        }    
        //6) Запрос пользователя по логину, в списке долны быть имя, пол и дата рождения статус активный или нет(Доступно Админам)
        [HttpGet("CheckInfoByLogin/{login}")]
        public async Task<ActionResult> CheckInfo(string login)
        {
            var info = await _userscontext.CheckInfoByLogin(login);

            if (info == null)
            {
                return NotFound(new { message = "Пользователь не найден" });
            }

            return Ok(info);
        }
        //7) Запрос пользователя по логину и паролю(Доступно только самому пользователю, если он активен (отсутствует RevokedOn))
        [HttpGet("Validate")]
        public async Task<ActionResult> ValidateUser(string username, string password)
        {
            var isValidUser = await _userscontext.ValidateUser(username, password);

            if (!isValidUser)
            {
                return NotFound(new { message = "Пользователь не найден или доступ к аккаунту отозван." });
            }

            return Ok(new { message = "Есть такой пользователь с данным паролем" });
        }
        // 8) Запрос всех пользователей старше определённого возраста(Доступно Админам)
        [HttpGet("olderthan/{age}")]
            public async Task<ActionResult<IEnumerable<User>>> GetUsersOlderThan(int age)
            {
            var users = await _userscontext.GetUsersOlderThan(age);

            if (!users.Any())
            {
                return NotFound(new { message = "Пользователь старше данного числа не найден" });
            }

            return Ok(users);
            }
        //Delete 9) Удаление пользователя по логину полное или мягкое(При мягком удалении должнапроисходить простановка RevokedOn и RevokedBy) (Доступно Админам)
        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser(string login, [FromQuery] string method)
        {
            if (await _context.Users.AnyAsync(u => u.Login == login))
            {
                if (method == "soft")
                {
                    await _userscontext.SoftDelete(login);
                }
                else if (method == "hard")
                {
                    await _userscontext.HardDelete(login);
                }
                return Ok(new { message = $"Пользователь '{login}' был успешно удален." });
            }

            return NotFound(new { message = "Пользователь не найден." });
        }

        //Update-2 10) Восстановление пользователя - Очистка полей(RevokedOn, RevokedBy) (Доступно Админам)
        [HttpPut("ClearRevokedFields")]
        public async Task<IActionResult> ClearRevokedFields(string login)
        {
            try
            {
                await _userscontext.ClearRevokedFields(login);
                return Ok(new { message = $"Для пользователя '{login}' очищены поля RevokedOn и RevokedBy." });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }

}
