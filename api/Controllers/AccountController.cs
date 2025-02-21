using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    public class AccountController(DataContext ctx) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterRequestParameters req){
            if (await UserExists(req.UserName))
                return BadRequest($"Username {req.UserName} already in use. Please choose another name");

            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = req.UserName.ToLowerInvariant(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(req.Password)),
                PassowordSalt = hmac.Key
            };

            ctx.Users.Add(user);
            await ctx.SaveChangesAsync();

            return user;
        }

        private Task<bool> UserExists(string username) =>
            ctx.Users.AnyAsync(u => u.UserName.ToLower().Equals(username.ToLower()));
    }
}