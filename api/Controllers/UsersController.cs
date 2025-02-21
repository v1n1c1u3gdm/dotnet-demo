using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Authorize]
    public class UsersController(DataContext ctx) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers() =>
            await ctx.Users.ToListAsync();

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AppUser>> GetUsers(int id) {
            var usr = await ctx.Users.FindAsync(id);

            if(usr == null)
                return NotFound();
            
            return usr;
        } 
    }
}