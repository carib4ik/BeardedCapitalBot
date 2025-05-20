using BeardedCapitalBot.Data;
using Microsoft.AspNetCore.Mvc;

namespace BeardedCapitalBot.MiniApp;

[ApiController]
[Route("api/[controller]")]
public class WebAppController : ControllerBase
{
    [HttpPost("userdata")]
    public IActionResult PostUserData([FromBody] UserData userData)
    {
        // Тут можно что-то сохранять, логировать и т.д.
        Console.WriteLine($"User: {userData.TelegramName}, Email: {userData.Email}");
        return Ok(new { message = "Данные получены!" });
    }
}