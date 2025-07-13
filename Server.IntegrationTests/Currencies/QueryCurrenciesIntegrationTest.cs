using System;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Dapper;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using Server.Main.Reactor;

namespace Server.IntegrationTests.Currencies;

[TestFixture]
public class QueryCurrenciesIntegrationTest
{
  private string _connectionString;
  private HttpClient _testHttpClient;
  private WebApplicationFactory<Program> _factory;

  [SetUp]
  public void OneTimeSetUp()
  {
    var testContextAppender = new TestContextAppender();
    BasicConfigurator.Configure(testContextAppender);

    var configuration = new ConfigurationBuilder()
      .SetBasePath(AppContext.BaseDirectory)
      .AddYamlFile("Infrastructure/Configuration/config.development.yaml", optional: false, reloadOnChange: true)
      .Build();

    _connectionString = configuration.GetConnectionString("FinanceDatabase");

    _factory = new WebApplicationFactory<Program>()
      .WithWebHostBuilder(builder =>
      {
        builder.ConfigureLogging(logging =>
        {
          logging.ClearProviders();
          logging.AddConsole();
          logging.SetMinimumLevel(LogLevel.Debug);
        });
      });

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
    _testHttpClient?.Dispose();
    _factory?.Dispose();

    using var connection = new MySqlConnection(_connectionString);
    Observable.FromAsync(() => connection.OpenAsync())
        .SelectMany(_ => Observable.FromAsync(() => connection.ExecuteAsync("CREATE DATABASE IF NOT EXISTS dboFinance;")))
        .Wait();
  }
}

