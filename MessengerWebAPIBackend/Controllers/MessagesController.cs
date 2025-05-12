using MessengerWebAPIBackend.Context;
using MessengerWebAPIBackend.Hubs;
using MessengerWebAPIBackend.Models;
using MessengerWebAPIBackend.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MessengerWebAPIBackend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        ApplicationContext _context;
        IHubContext<MessengerHub> _hub;
        public MessagesController(ApplicationContext context, IHubContext<MessengerHub> hub)
        {
            _context = context;
            _hub = hub;
        }
        [HttpGet]
        public async Task<IActionResult> Get(int userId)
        {
            int thisUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var messages = await _context.Messages.Include(m => m.UserMessages).Where(m => (m.UserMessages.ToList()[0].UserId == thisUserId && m.UserMessages.ToList()[1].UserId == userId) ||
                                                        (m.UserMessages.ToList()[1].UserId == thisUserId && m.UserMessages.ToList()[0].UserId == userId))
                                                        .Select(m => new MessageDTO
                                                        {
                                                            Id = m.Id,
                                                            MessageText = m.MessageText,
                                                            PublicationDate = m.PublicationDate,
                                                            Users = m.UserMessages.OrderBy(um => um.Id).Select(um => new UserDTO
                                                            {
                                                                Id = um.User.Id,
                                                                Name = um.User.Name
                                                            }).ToList()
                                                        }).ToListAsync();
            if (messages is null)
                return NotFound();
            return Ok(messages);
        }
        [HttpPost]
        public async Task<IActionResult> Post(string message, int userId)
        {
            int thisUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var messageUsers = await _context.Users
                                    .Where(u => u.Id == thisUserId || u.Id == userId)
                                    .ToListAsync(); 
            if (messageUsers.Count < 2)
                return NotFound("User not found");
            var newMessage = new Message
            {
                MessageText = message,
                PublicationDate = DateTime.Now
            };
            await _context.Messages.AddAsync(newMessage);
            await _context.SaveChangesAsync();
            newMessage.UserMessages =
            [
                new UserMessages
                {
                    UserId = thisUserId,
                    MessageId = newMessage.Id,
                },
                new UserMessages
                {
                    UserId = userId,
                    MessageId = newMessage.Id,
                },
            ];
            await _context.SaveChangesAsync();

            MessageDTO responseMessage = new MessageDTO
            {
                MessageText = newMessage.MessageText,
                PublicationDate = newMessage.PublicationDate,
                Users = newMessage.UserMessages.Select(um => new UserDTO
                {
                    Id = um.User.Id,
                    Name = um.User.Name
                }).ToList()
            };

            await _hub.Clients.User(userId.ToString()).SendAsync("ReceiveMessage", responseMessage);
            
            return Ok(responseMessage);
        }
        [HttpPut]
        public async Task<IActionResult> Update(string messageText, int messageId)
        {
            int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var message = await _context.Messages.Include(m => m.UserMessages).FirstOrDefaultAsync(m => m.UserMessages.ToList()[0].Id == userId && m.Id == messageId);
            if (message is null)
                return NotFound("Message not found or user haven't access");
            message.MessageText = messageText;
            await _context.SaveChangesAsync();

            await _hub.Clients.Client(message.UserMessages.ToList()[1].UserId.ToString()).SendAsync("UpdateMessage", message);

            return Ok(message);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int messageId)
        {
            int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == messageId && m.UserMessages.ToList()[0].UserId == userId);
            if (message is null)
                return NotFound("Message not found or user haven't access");
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            await _hub.Clients.Client(message.UserMessages.ToList()[1].Id.ToString()).SendAsync("DeleteMessage", message);

            return Ok();
        }
    }
}
