using System;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Src.Main.Reactor;
using Src.Main.Reactor.Models.Dto;
using Xunit;

namespace Server.IntegrationTests.Main.Reactor.Currencies;

public class WriteCurrenciesIntegrationTest : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
  private readonly string _connectionString;
  private readonly HttpClient _testHttpClient;

  public WriteCurrenciesIntegrationTest(WebApplicationFactory<Program> factory)
  {
    var configuration = new ConfigurationBuilder()
      .SetBasePath(AppContext.BaseDirectory)
      .AddJsonFile("Src/Main/Infrastructure/Configuration/application.Development.json", optional: false, reloadOnChange: true)
      .Build();

    _connectionString = configuration.GetConnectionString("FinanceDatabase");
    _testHttpClient = factory.CreateClient();
  }

  [Fact]
  public async Task GivenClientRequestsExistingCurrencyWhenClientPostsExistingRandsThenFailCreationAsUnprocessableEntity()
  {
    // Given --  ZAR already exists as an entry
    await SimulateExistingSouthAfricanRand();

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

  [Fact]
  public async Task GivenClientRequestsNewCurrencyWhenClientPostsNewSwissCurrencyThenVerifyCreationAsCreatedSuccessfully()
  {
    // Given -- party requests for CHF creation
    var expectedCurrencyCode = "CHF";
    var currencyDto = new CurrencyDto
    {
      CurrencyCode = expectedCurrencyCode,
      CurrencyId = 756,
      CurrencyName = "Swiss Franc",
      CurrencySymbol = "CHF",
      CurrencyFlag = "🇨🇭"
    };

    // When -- posting the currency entry
    var content = new StringContent(
      JsonSerializer.Serialize(currencyDto, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower }),
      Encoding.UTF8,
      "application/json"
    );

    var response = await _testHttpClient.PostAsync("/v1/WriteCurrencyResource", content);

    // Then -- ensure result contains the created status code
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
  }

  [Fact]
  public async Task GivenClientRequestsNewCurrenciesWhenClientPostsBatchCurrenciesThenVerifyCreationAsCreatedSuccessfully()
  {
// Given -- party requests for batch creation
    var jsonBatchContent = JsonBatchContent();

    // When -- posting the currency entry
    var content = new StringContent(jsonBatchContent, Encoding.UTF8, "application/json");

    var response = await _testHttpClient.PostAsync("/v1/WriteCurrencyResource/Batch", content);

    // Then -- ensure result contains the created status code
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
  }

  private async Task SimulateExistingSouthAfricanRand()
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

    await _testHttpClient.PostAsync("/v1/WriteCurrencyResource", content);
  }

  public void Dispose()
  {
    var connection = new MySqlConnection(_connectionString);
    Observable.FromAsync(() => connection.OpenAsync())
      .SelectMany(_ => Observable.FromAsync(() => connection.ExecuteAsync("DELETE FROM dboFinance.CURRENCIES WHERE CURRENCY_ID IS NOT NULL")))
      .SelectMany(_ => Observable.FromAsync(() => connection.ExecuteAsync("CREATE DATABASE IF NOT EXISTS dboFinance;")))
      .Wait();
  }

  private String JsonBatchContent()
  {
    return """
           {
               "batch_currencies": [
                   {
                       "currency_id": 952,
                       "currency_code": "XOF",
                       "currency_name": "CFA Franc BCEAO (West African CFA franc)",
                       "currency_symbol": "CFA",
                       "currency_flag": "🇸🇳"
                   },
                   {
                       "currency_id": 950,
                       "currency_code": "XAF",
                       "currency_name": "CFA Franc BEAC (Central African CFA franc)",
                       "currency_symbol": "FCFA",
                       "currency_flag": "🇨🇲"
                   },
                   {
                       "currency_id": 710,
                       "currency_code": "ZAR",
                       "currency_name": "South African Rand",
                       "currency_symbol": "R",
                       "currency_flag": "🇿🇦"
                   },
                   {
                       "currency_id": 566,
                       "currency_code": "NGN",
                       "currency_name": "Nigerian Naira",
                       "currency_symbol": "₦",
                       "currency_flag": "🇳🇬"
                   },
                   {
                       "currency_id": 404,
                       "currency_code": "KES",
                       "currency_name": "Kenyan Shilling",
                       "currency_symbol": "KSh",
                       "currency_flag": "🇰🇪"
                   },
                   {
                       "currency_id": 800,
                       "currency_code": "UGX",
                       "currency_name": "Ugandan Shilling",
                       "currency_symbol": "USh",
                       "currency_flag": "🇺🇬"
                   },
                   {
                       "currency_id": 936,
                       "currency_code": "GHS",
                       "currency_name": "Ghanaian Cedi",
                       "currency_symbol": "₵",
                       "currency_flag": "🇬🇭"
                   },
                   {
                       "currency_id": 834,
                       "currency_code": "TZS",
                       "currency_name": "Tanzanian Shilling",
                       "currency_symbol": "TSh",
                       "currency_flag": "🇹🇿"
                   },
                   {
                       "currency_id": 967,
                       "currency_code": "ZMW",
                       "currency_name": "Zambian Kwacha",
                       "currency_symbol": "ZK",
                       "currency_flag": "🇿🇲"
                   },
                   {
                       "currency_id": 58,
                       "currency_code": "BWP",
                       "currency_name": "Botswana Pula",
                       "currency_symbol": "P",
                       "currency_flag": "🇧🇼"
                   },
                   {
                       "currency_id": 690,
                       "currency_code": "SCR",
                       "currency_name": "Seychellois Rupee",
                       "currency_symbol": "₨",
                       "currency_flag": "🇸🇨"
                   },
                   {
                       "currency_id": 516,
                       "currency_code": "NAD",
                       "currency_name": "Namibian Dollar",
                       "currency_symbol": "$",
                       "currency_flag": "🇳🇦"
                   },
                   {
                       "currency_id": 480,
                       "currency_code": "MUR",
                       "currency_name": "Mauritian Rupee",
                       "currency_symbol": "₨",
                       "currency_flag": "🇲🇺"
                   },
                   {
                       "currency_id": 454,
                       "currency_code": "MWK",
                       "currency_name": "Malawian Kwacha",
                       "currency_symbol": "MK",
                       "currency_flag": "🇲🇼"
                   },
                   {
                       "currency_id": 270,
                       "currency_code": "GMD",
                       "currency_name": "Gambian Dalasi",
                       "currency_symbol": "D",
                       "currency_flag": "🇬🇲"
                   },
                   {
                       "currency_id": 748,
                       "currency_code": "SZL",
                       "currency_name": "Swazi Lilangeni (Swaziland)",
                       "currency_symbol": "E",
                       "currency_flag": "🇸🇿"
                   },
                   {
                       "currency_id": 646,
                       "currency_code": "RWF",
                       "currency_name": "Rwandan Franc",
                       "currency_symbol": "RF",
                       "currency_flag": "🇷🇼"
                   },
                   {
                       "currency_id": 426,
                       "currency_code": "LSL",
                       "currency_name": "Lesotho Loti",
                       "currency_symbol": "L",
                       "currency_flag": "🇱🇸"
                   },
                   {
                       "currency_id": 706,
                       "currency_code": "SOS",
                       "currency_name": "Somali Shilling",
                       "currency_symbol": "Sh",
                       "currency_flag": "🇸🇴"
                   },
                   {
                       "currency_id": 694,
                       "currency_code": "SLL",
                       "currency_name": "Sierra Leonean Leone",
                       "currency_symbol": "Le",
                       "currency_flag": "🇸🇱"
                   },
                   {
                       "currency_id": 943,
                       "currency_code": "MZN",
                       "currency_name": "Mozambican Metical",
                       "currency_symbol": "MT",
                       "currency_flag": "🇲🇿"
                   },
                   {
                       "currency_id": 132,
                       "currency_code": "CVE",
                       "currency_name": "Cape Verdean Escudo",
                       "currency_symbol": "$",
                       "currency_flag": "🇨🇻"
                   },
                   {
                       "currency_id": 108,
                       "currency_code": "BIF",
                       "currency_name": "Burundian Franc",
                       "currency_symbol": "FBu",
                       "currency_flag": "🇧🇮"
                   },
                   {
                       "currency_id": 262,
                       "currency_code": "DJF",
                       "currency_name": "Djiboutian Franc",
                       "currency_symbol": "Fdj",
                       "currency_flag": "🇩🇯"
                   },
                   {
                       "currency_id": 232,
                       "currency_code": "ERN",
                       "currency_name": "Eritrean Nakfa",
                       "currency_symbol": "Nfk",
                       "currency_flag": "🇪🇷"
                   },
                   {
                       "currency_id": 230,
                       "currency_code": "ETB",
                       "currency_name": "Ethiopian Birr",
                       "currency_symbol": "Br",
                       "currency_flag": "🇪🇹"
                   },
                   {
                       "currency_id": 174,
                       "currency_code": "KMF",
                       "currency_name": "Comorian Franc",
                       "currency_symbol": "CF",
                       "currency_flag": "🇰🇲"
                   },
                   {
                       "currency_id": 973,
                       "currency_code": "AOA",
                       "currency_name": "Angolan Kwanza",
                       "currency_symbol": "Kz",
                       "currency_flag": "🇦🇴"
                   },
                   {
                       "currency_id": 678,
                       "currency_code": "STD",
                       "currency_name": "São Tomé and Príncipe Dobra",
                       "currency_symbol": "Db",
                       "currency_flag": "🇸🇹"
                   },
                   {
                       "currency_id": 938,
                       "currency_code": "SDG",
                       "currency_name": "Sudanese Pound",
                       "currency_symbol": "£SD",
                       "currency_flag": "🇸🇩"
                   }
               ]
           }
           """;
  }
}
