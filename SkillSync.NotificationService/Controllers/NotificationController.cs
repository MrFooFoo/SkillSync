using Microsoft.AspNetCore.Mvc;
namespace SkillSync.NotificationService.Controllers{

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("Notification Service Running");
}

}
