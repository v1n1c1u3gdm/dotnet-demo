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
        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login(LoginRequestParameters req){
            try
            {
                var user = await ctx.Users.SingleAsync(u => u.UserName == req.UserName.ToLower());
                using var hmac = new HMACSHA512(user.PassowordSalt);

                var givenPassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(req.Password));
                if(!givenPassword.SequenceEqual(user.PasswordHash)) return Unauthorized("Invalid password");

                return user;
            }
            catch (Exception ex)
            {
                return Unauthorized($"Credentials are invalid ({ex.Message})");
            }
        }

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