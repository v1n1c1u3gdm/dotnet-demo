using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class UsersController(DataContext ctx) : ControllerBase
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