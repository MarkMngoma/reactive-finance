namespace Server.Main.Reactor.Models.DTO;

public class BasicResponseDto
{
  public bool Success { get; set; }

  public BasicResponseDto(bool success)
  {
    Success = success;
  }
}
