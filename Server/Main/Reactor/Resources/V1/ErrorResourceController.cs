using log4net;
using Microsoft.AspNetCore.Mvc;

namespace Server.Main.Reactor.Resources.V1
{
  [Route("api/[controller]")]
  [Produces("application/json")]
  [ApiController]
  public class ErrorResourceController : ControllerBase
  {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(ErrorResourceController));


    [HttpGet]
    [Route("Error")]
    public IActionResult GetError()
    {
      Logger.Error("An error occurred while processing the request.");
      return StatusCode(StatusCodes.Status417ExpectationFailed, new
      {
        Message = "An unexpected error occurred. Please try again later."
      });
    }

  }
}
