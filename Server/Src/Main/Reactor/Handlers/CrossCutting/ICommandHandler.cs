namespace Src.Main.Reactor.Handlers.CrossCutting;

public interface ICommandHandler<REQ, RES>
{
  public IObservable<RES> Handle(REQ request);
}
