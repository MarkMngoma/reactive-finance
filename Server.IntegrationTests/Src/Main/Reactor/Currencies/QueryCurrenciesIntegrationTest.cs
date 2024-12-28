using System;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Src.Main.Reactor;
using Xunit;

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

  public void Dispose()
  {
    _testHttpClient.Dispose();
    var connection = new MySqlConnection(_connectionString);
    Observable.FromAsync(() => connection.OpenAsync())
      .SelectMany(_ => Observable.FromAsync(() => connection.ExecuteAsync("CREATE DATABASE IF NOT EXISTS dboFinance;")))
      .Wait();
  }
}
