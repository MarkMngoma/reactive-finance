using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Src.Main.Reactor;
using Src.Main.Reactor.Models.Dto;
using Xunit;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;

namespace Server.IntegrationTests.Main.Reactor.Currencies;

public class QueryCurrenciesIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{

  private readonly HttpClient _testHttpClient;

  public QueryCurrenciesIntegrationTest(WebApplicationFactory<Program> factory)
  {
    _testHttpClient = factory.CreateClient();
  }

  [Fact]
  public async Task GivenRequestedPathWhenClientRequestsCollectiveQueryThenSuccessIsReturned()
  {
    var response = await _testHttpClient.GetAsync("/v1/QueryCurrencyResource");
    response.EnsureSuccessStatusCode();
  }

  [Fact]
  public async Task GivenRequestedPathNotFoundWhenClientRequestsCollectiveQueryThen404IsReturned()
  {
    var response = await _testHttpClient.GetAsync("/QueryCurrencyResource");
    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }

  [Fact]
  public async Task GivenRequestedWhenClientRequestsSingleQueryThenSuccessWithSingleEntryIsReturned()
  {
    // Given -- party requests for ZAR
    var expectedCurrencyCode = "ZAR";

    // When -- requesting for single currency entry
    var response = await _testHttpClient.GetAsync($"/v1/QueryCurrencyResource/{expectedCurrencyCode}");

    // Then -- ensure success result contains desired entry
    response.EnsureSuccessStatusCode();
    var responsePayload = await JsonSerializer.DeserializeAsync<CurrencyDto[]>(await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true,
      PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    });

    Assert.Equal(expectedCurrencyCode, responsePayload[0].CurrencyCode);
  }


}
