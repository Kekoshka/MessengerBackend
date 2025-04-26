using MessengerWebAPIBackend.Context;
using MessengerWebAPIBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MessengerWebAPIBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        ApplicationContext _context;
        public MessagesController(ApplicationContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Get(int userId)
        {
            int thisUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var messages = _context.Messages.Where(m => (m.UserFromId == thisUserId && m.UserToId == userId) ||
                                                        (m.UserToId == thisUserId && m.UserFromId == userId));
            if (messages is null)
                return NotFound();
            return Ok(messages);
        }
        [HttpPost]
        public async Task<IActionResult> Post(string message, int userId)
        {
            int thisUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userTo = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (userTo is null)
                return NotFound("User not found");
            var newMessage = new Message
            {
                UserFromId = thisUserId,
                UserToId = userId,
                MessageText = message
            };
            await _context.Messages.AddAsync(newMessage);
            await _context.SaveChangesAsync();
            return Ok(newMessage);
        }
        [HttpPut]
        public async Task<IActionResult> Update(string messageText, int messageId)
        {
            int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var message = await _context.Messages.FirstOrDefaultAsync(m => m.UserFromId == userId && m.Id == messageId);
            if (message is null)
                return NotFound("Message not found or user haven't access");
            message.MessageText = messageText;
            await _context.SaveChangesAsync();
            return Ok(message);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int messageId)
        {
            int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var message = _context.Messages.FirstOrDefaultAsync(m => m.Id == messageId && m.UserFromId == userId);
            if (message is null)
                return NotFound("Message not found or user haven't access");
            _context.SaveChangesAsync();
            return Ok();
        }
    }
}
