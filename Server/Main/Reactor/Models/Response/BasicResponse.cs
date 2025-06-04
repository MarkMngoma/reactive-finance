namespace Server.Main.Reactor.Models.Response;

public class BasicResponse
{
  public bool Success { get; set; }

  public BasicResponse(bool success)
  {
    Success = success;
  }
}
