using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.Business.Finance;
using Server.Main.Reactor.Utils;

namespace Server.Main.Reactor.Resources.V1;

[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Route("v1/financialResource")]
[Produces("application/json")]
public class FinancialResourceController : ControllerBase
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(FinancialResourceController));

  private readonly QueryFinancialProductHandler _queryFinancialProductHandler;

  public FinancialResourceController(QueryFinancialProductHandler queryFinancialProductHandler)
  {
    _queryFinancialProductHandler = queryFinancialProductHandler;
  }

  [HttpGet]
  public Task<IActionResult> GetFinancialProducts()
  {
    Logger.Info("FinancialResourceController@GetFinancialProducts initiated...");
    return _queryFinancialProductHandler.Handle()
      .Catch<IActionResult, Exception>(ex => ContentResultUtil.Throw(ex, StatusCodes.Status404NotFound))
      .ToTask();
  }

  [HttpGet]
  [Route("{id}")]
  public Task<IActionResult> GetFinancialProducts(string id)
  {
    Logger.Info($"FinancialResourceController@GetFinancialProducts initiated for id: {id}");
    return _queryFinancialProductHandler.Handle(id)
      .Catch<IActionResult, Exception>(ex => ContentResultUtil.Throw(ex, StatusCodes.Status404NotFound))
      .ToTask();
  }

}
