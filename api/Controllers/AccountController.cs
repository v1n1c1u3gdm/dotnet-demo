using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    public class AccountController(DataContext ctx, ITokenService tokenSvc) : BaseApiController
    {
        [HttpGet("GetTestUser")]
        public async Task<ActionResult<UserCredential>> GetTestUser() {
            if(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                return await this.Login(new LoginRequestParameters{
                    UserName = "mike",
                    Password = "#{i-doN)lTQc"
                });
            else return NotFound("?");
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserCredential>> Login(LoginRequestParameters req){
            try
            {
                var user = await ctx.Users.SingleAsync(u => u.UserName == req.UserName.ToLower());
                using var hmac = new HMACSHA512(user.PassowordSalt);

                var givenHashedPassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(req.Password));
                if (!givenHashedPassword.SequenceEqual(user.PasswordHash)) return Unauthorized("Invalid password");

                return MapAndCreateToken(user);
            }
            catch (Exception ex)
            {
                return Unauthorized($"Credentials are invalid ({ex.Message})");
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserCredential>> Register(RegisterRequestParameters req){
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

            return MapAndCreateToken(user);
        }

        private UserCredential MapAndCreateToken(AppUser user) =>
            new ()
            {
                UserName = user.UserName,
                Token = tokenSvc.CreateToken(user)
            };

        private Task<bool> UserExists(string username) =>
            ctx.Users.AnyAsync(u => u.UserName.ToLower().Equals(username.ToLower()));
    }
}