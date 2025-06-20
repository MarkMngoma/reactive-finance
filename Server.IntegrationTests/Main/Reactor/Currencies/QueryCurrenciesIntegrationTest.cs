using System;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using Server.Main.Reactor;

namespace Server.IntegrationTests.Main.Reactor.Currencies;

[TestFixture]
public class QueryCurrenciesIntegrationTest
{
  private string _connectionString;
  private HttpClient _testHttpClient;
  private WebApplicationFactory<Program> _factory;

  [SetUp]
  public void OneTimeSetUp()
  {
    var configuration = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddYamlFile("Infrastructure/Configuration/config.development.yaml", optional: false, reloadOnChange: true)
        .Build();

    _connectionString = configuration.GetConnectionString("FinanceDatabase");

    _factory = new WebApplicationFactory<Program>();
    _testHttpClient = _factory.CreateClient();
  }

  [Test]
  public async Task GivenRequestedPathWhenClientRequestsCollectiveQueryThenSuccessIsReturned()
  {
    var response = await _testHttpClient.GetAsync("/v1/queryCurrencyResource");
    response.EnsureSuccessStatusCode();
    Assert.That(HttpStatusCode.OK, Is.EqualTo(response.StatusCode));
  }

  [Test]
  public async Task GivenRequestedPathNotFoundWhenClientRequestsCollectiveQueryThen404IsReturned()
  {
    var response = await _testHttpClient.GetAsync("/queryCurrencyResource");
    Assert.That(HttpStatusCode.NotFound, Is.EqualTo(response.StatusCode));
  }

  [TearDown]
  public void OneTimeTearDown()
  {
    Console.SetOut(TestContext.Progress);
    _testHttpClient?.Dispose();
    _factory?.Dispose();

    using var connection = new MySqlConnection(_connectionString);
    Observable.FromAsync(() => connection.OpenAsync())
        .SelectMany(_ => Observable.FromAsync(() => connection.ExecuteAsync("CREATE DATABASE IF NOT EXISTS dboFinance;")))
        .Wait();
  }
}

