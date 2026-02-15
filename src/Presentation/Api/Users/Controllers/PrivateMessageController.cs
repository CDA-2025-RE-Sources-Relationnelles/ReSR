using Microsoft.AspNetCore.Mvc;

namespace ReSR.Presentation.Api.Users.Controllers;
[ApiController]
[Route(ROUTE)]
public class PrivateMessageController : ControllerBase {

    public const string ROUTE = "/private-messages";
    
}
