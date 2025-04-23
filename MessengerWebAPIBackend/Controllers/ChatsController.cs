using System.Security.Claims;
using MessengerWebAPIBackend.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessengerWebAPIBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        ApplicationContext _context;
        public ChatsController(ApplicationContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            дШЫЕ
        }
    }
}
