using System;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Src.Main.Reactor;
using Src.Main.Reactor.Models.Dto;
using Xunit;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;

namespace Server.IntegrationTests.Main.Reactor.Currencies;

public class QueryCurrenciesIntegrationTest : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
  private readonly string _connectionString;
  private readonly HttpClient _testHttpClient;

  public QueryCurrenciesIntegrationTest(WebApplicationFactory<Program> factory)
  {
    var configuration = new ConfigurationBuilder()
      .SetBasePath(AppContext.BaseDirectory)
      .AddJsonFile("Src/Main/Infrastructure/Configuration/application.Development.json", optional: false, reloadOnChange: true)
      .Build();

    _connectionString = configuration.GetConnectionString("FinanceDatabase");
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

    responsePayload[0].CurrencyCode.Should().Match(expectedCurrencyCode);
  }

  public void Dispose()
  {
    var connection = new MySqlConnection(_connectionString);
    Observable.FromAsync(() => connection.OpenAsync())
      .SelectMany(_ => Observable.FromAsync(() => connection.ExecuteAsync("DELETE FROM dboFinance.CURRENCIES WHERE CURRENCY_ID=756;")))
      .SelectMany(_ => Observable.FromAsync(() => connection.ExecuteAsync("CREATE DATABASE IF NOT EXISTS dboFinance;")))
      .Wait();
  }
}
