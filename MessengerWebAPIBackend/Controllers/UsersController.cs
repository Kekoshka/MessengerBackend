using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MessengerWebAPIBackend.Common;
using MessengerWebAPIBackend.Context;
using MessengerWebAPIBackend.Hubs;
using MessengerWebAPIBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MessengerWebAPIBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        ApplicationContext _context;
        IHubContext<MessengerHub> _hub;
        public UsersController(ApplicationContext context, IHubContext<MessengerHub> hub) 
        {
            _context = context;
            _hub = hub;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string login, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login && u.Password == password);
            if (user is null) return Unauthorized();
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.Issuer,
                audience: user.Id.ToString(),
                claims: null,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),SecurityAlgorithms.HmacSha256));
            return Ok(new JwtSecurityTokenHandler().WriteToken(jwt));
        }
        [HttpPost]
        public async Task<IActionResult> Post(User user)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Login == user.Login);
            if (existingUser is not null) return Conflict("User with this mail already exists");
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [Authorize]
        [HttpPatch]
        public async Task<IActionResult> Update(DateTime lastAction)
        {
            int userId = Convert.ToInt32(User.Identity?.Name);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
                return NotFound();
            user.LastAction = lastAction;
            await _context.SaveChangesAsync();

            await _hub.Clients.All.SendAsync("UpdateUserLastAction", user.Id, lastAction);

            return NoContent();
        }
    }
}
