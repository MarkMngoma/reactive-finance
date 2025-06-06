namespace Server.Main.Reactor;

public class Program
{
  public static void Main(string[] args)
  {
    var reactorCommand = new ReactorCommand();
    reactorCommand.Run(args);
  }
}
