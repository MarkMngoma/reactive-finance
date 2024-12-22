using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Src.Main.Reactor;
using Src.Main.Reactor.Models.Dto;
using Xunit;

namespace Server.IntegrationTests.Main.Reactor.Currencies;

public class WriteCurrenciesIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
  private readonly HttpClient _testHttpClient;

  public WriteCurrenciesIntegrationTest(WebApplicationFactory<Program> factory)
  {
    _testHttpClient = factory.CreateClient();
  }

  [Fact]
  public async Task GivenClientRequestsExistingCurrencyWhenClientPostsExistingRandsThenFailCreationAsUnprocessableEntity()
  {
    // Given -- party requests for ZAR creation
    var expectedCurrencyCode = "ZAR";
    var currencyDto = new CurrencyDto
    {
      CurrencyCode = expectedCurrencyCode,
      CurrencyId = 710,
      CurrencyName = "South African Rand",
      CurrencySymbol = "R",
      CurrencyFlag = "🇿🇦"
    };

    // When -- posting the currency entry
    var content = new StringContent(
      JsonSerializer.Serialize(currencyDto, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower }),
      Encoding.UTF8,
      "application/json"
    );

    var response = await _testHttpClient.PostAsync("/v1/WriteCurrencyResource", content);

    // Then -- ensure result contains the expected status code
    Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
  }
}
