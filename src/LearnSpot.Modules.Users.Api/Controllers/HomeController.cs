using Microsoft.AspNetCore.Mvc;

namespace LearnSpot.Modules.Users.Api.Controllers;

[Route(UsersModule.BasePath)]
internal class HomeController : ControllerBase
{
    [HttpGet]
    public ActionResult<string> Get() => "Users API";
}