using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MessengerWebAPIBackend.Common;
using MessengerWebAPIBackend.Context;
using MessengerWebAPIBackend.Hubs;
using MessengerWebAPIBackend.Models;
using MessengerWebAPIBackend.Models.DTO;
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
        [HttpPost("login")]
        public async Task<IActionResult> Post(SecurityData loginData)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == loginData.Login && u.Password == loginData.Password);
            if (user is null)
                return Unauthorized("Login or password is incorrect");
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name)
            };
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.Issuer,
                audience: user.Id.ToString(),
                expires: DateTime.UtcNow.AddMinutes(15),
                claims: claims,
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            return Ok(new JwtSecurityTokenHandler().WriteToken(jwt));
        }
        [Authorize]
        [HttpGet("search")]
        public async Task<IActionResult> Get(string name = "")
        {
            int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var messages = _context.Messages.Where(m => m.UserMessages.ToList()[0].UserId == userId || m.UserMessages.ToList()[1].UserId == userId);
            var users = await _context.Users.Include(u => u.UserMessages).ThenInclude(um => um.Message).Where(u => u.Name
                                            .Contains(name) && u.Id != userId)
                                            .Select(u => new User
                                            {
                                                Id = u.Id,
                                                Name = u.Name,
                                                LastAction = u.LastAction,
                                                Photo = u.Photo,
                                                LastMessage = messages
                                                                    .OrderBy(m => m.PublicationDate)
                                                                    .LastOrDefault(m => m.UserMessages.ToList()[0].UserId == userId && m.UserMessages.ToList()[1].UserId == u.Id 
                                                                    || m.UserMessages.ToList()[1].UserId == userId && m.UserMessages.ToList()[0].UserId == u.Id)
                                            })
                                            .ToListAsync();
            if (users is null)
                return NotFound();
            return Ok(users);
        }
        [HttpPost]
        public async Task<IActionResult> Post(User user)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Login == user.Login);
            if (existingUser is not null) return Conflict("User with this mail already exists");
            user.LastAction = DateTime.Now;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [Authorize]
        [HttpPatch]
        public async Task<IActionResult> Update()
        {
            int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
                return NotFound();
            user.LastAction = DateTime.Now;
            await _context.SaveChangesAsync();

            await _hub.Clients.All.SendAsync("UpdateUserLastAction", user.Id, user.LastAction);

            return NoContent();
        }
    }
}
