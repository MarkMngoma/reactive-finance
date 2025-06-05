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
using NUnit.Framework;
using Server.Main.Reactor;
using Server.Main.Reactor.Models.Request;

namespace Server.IntegrationTests.Main.Reactor.Currencies;

[TestFixture]
public class WriteCurrenciesIntegrationTest
{
    private string _connectionString;
    private HttpClient _testHttpClient;
    private WebApplicationFactory<Program> _factory;

    [SetUp]
    public void OneTimeSetUp()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("Infrastructure/Configuration/application.Development.json", optional: false, reloadOnChange: true)
            .Build();

        _connectionString = configuration.GetConnectionString("FinanceDatabase");

        _factory = new WebApplicationFactory<Program>();
        _testHttpClient = _factory.CreateClient();
    }

    [Test]
    public async Task GivenClientRequestsExistingCurrencyWhenClientPostsExistingRandsThenFailCreationAsUnprocessableEntity()
    {
        // Given --  ZAR already exists as an entry
        await SimulateExistingSouthAfricanRand();

        // Given -- party requests for ZAR creation
        var expectedCurrencyCode = "ZAR";
        var currencyDto = new CurrencyRequest
        {
            CurrencyCode = expectedCurrencyCode,
            CurrencyId = 710,
            CurrencyName = "South African Rand",
            CurrencySymbol = "R",
            CurrencyFlag = "🇿🇦"
        };

        // When -- posting the currency entry
        var content = new StringContent(
          JsonSerializer.Serialize(currencyDto, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
          Encoding.UTF8,
          "application/json"
        );

        var response = await _testHttpClient.PostAsync("/v1/WriteCurrencyResource", content);

        // Then -- ensure result contains the expected status code
        Assert.That(HttpStatusCode.UnprocessableEntity, Is.EqualTo(response.StatusCode));
    }

    [Test]
    public async Task GivenClientRequestsNewCurrencyWhenClientPostsNewSwissCurrencyThenVerifyCreationAsCreatedSuccessfully()
    {
        // Given -- party requests for CHF creation
        var expectedCurrencyCode = "CHF";
        var currencyDto = new CurrencyRequest
        {
            CurrencyCode = expectedCurrencyCode,
            CurrencyId = 756,
            CurrencyName = "Swiss Franc",
            CurrencySymbol = "CHF",
            CurrencyFlag = "🇨🇭"
        };

        // When -- posting the currency entry
        var content = new StringContent(
          JsonSerializer.Serialize(currencyDto, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
          Encoding.UTF8,
          "application/json"
        );

        var response = await _testHttpClient.PostAsync("/v1/WriteCurrencyResource", content);

        // Then -- ensure result contains the created status code
        Assert.That(HttpStatusCode.OK, Is.EqualTo(response.StatusCode));

        var queryResponse = await _testHttpClient.GetAsync($"/v1/QueryCurrencyResource/{expectedCurrencyCode}");

        // Then -- ensure success result contains desired entry
        queryResponse.EnsureSuccessStatusCode();
        var responsePayload = await JsonSerializer.DeserializeAsync<CurrencyRequest>(await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.That(756, Is.EqualTo(responsePayload.CurrencyId));
        Assert.That(expectedCurrencyCode, Is.EqualTo(responsePayload.CurrencyCode));
    }

    [Test]
    public async Task GivenClientRequestsNewCurrenciesWhenClientPostsBatchCurrenciesThenVerifyCreationAsCreatedSuccessfully()
    {
        // Given -- party requests for batch creation
        var jsonBatchContent = JsonBatchContent();

        // When -- posting the currency entry
        var content = new StringContent(jsonBatchContent, Encoding.UTF8, "application/json");

        var response = await _testHttpClient.PostAsync("/v1/WriteCurrencyResource/Batch", content);

        // Then -- ensure result contains the created status code
        Assert.That(HttpStatusCode.OK, Is.EqualTo(response.StatusCode));
    }

    private async Task SimulateExistingSouthAfricanRand()
    {
        // Given -- party requests for ZAR creation
        var expectedCurrencyCode = "ZAR";
        var currencyDto = new CurrencyRequest
        {
            CurrencyCode = expectedCurrencyCode,
            CurrencyId = 710,
            CurrencyName = "South African Rand",
            CurrencySymbol = "R",
            CurrencyFlag = "🇿🇦"
        };

        // When -- posting the currency entry
        var content = new StringContent(
          JsonSerializer.Serialize(currencyDto, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
          Encoding.UTF8,
          "application/json"
        );

        await _testHttpClient.PostAsync("/v1/WriteCurrencyResource", content);
    }

    [TearDown]
    public void OneTimeTearDown()
    {
        _testHttpClient?.Dispose();
        _factory?.Dispose();

        var connection = new MySqlConnection(_connectionString);
        Observable.FromAsync(() => connection.OpenAsync())
          .SelectMany(_ => Observable.FromAsync(() => connection.ExecuteAsync("DELETE FROM dboFinance.CURRENCIES WHERE CURRENCY_ID IS NOT NULL")))
          .SelectMany(_ => Observable.FromAsync(() => connection.ExecuteAsync("CREATE DATABASE IF NOT EXISTS dboFinance;")))
          .Wait();
    }

    private static string JsonBatchContent()
    {
        return """
           {
                "batchCurrencies": [
                    {
                        "currencyId": 952,
                        "currencyCode": "XOF",
                        "currencyName": "CFA Franc BCEAO (West African CFA franc)",
                        "currencySymbol": "CFA",
                        "currencyFlag": "🇸🇳"
                    },
                    {
                        "currencyId": 950,
                        "currencyCode": "XAF",
                        "currencyName": "CFA Franc BEAC (Central African CFA franc)",
                        "currencySymbol": "FCFA",
                        "currencyFlag": "🇨🇲"
                    },
                    {
                        "currencyId": 710,
                        "currencyCode": "ZAR",
                        "currencyName": "South African Rand",
                        "currencySymbol": "R",
                        "currencyFlag": "🇿🇦"
                    },
                    {
                        "currencyId": 566,
                        "currencyCode": "NGN",
                        "currencyName": "Nigerian Naira",
                        "currencySymbol": "₦",
                        "currencyFlag": "🇳🇬"
                    },
                    {
                        "currencyId": 404,
                        "currencyCode": "KES",
                        "currencyName": "Kenyan Shilling",
                        "currencySymbol": "KSh",
                        "currencyFlag": "🇰🇪"
                    },
                    {
                        "currencyId": 800,
                        "currencyCode": "UGX",
                        "currencyName": "Ugandan Shilling",
                        "currencySymbol": "USh",
                        "currencyFlag": "🇺🇬"
                    },
                    {
                        "currencyId": 936,
                        "currencyCode": "GHS",
                        "currencyName": "Ghanaian Cedi",
                        "currencySymbol": "₵",
                        "currencyFlag": "🇬🇭"
                    },
                    {
                        "currencyId": 834,
                        "currencyCode": "TZS",
                        "currencyName": "Tanzanian Shilling",
                        "currencySymbol": "TSh",
                        "currencyFlag": "🇹🇿"
                    },
                    {
                        "currencyId": 967,
                        "currencyCode": "ZMW",
                        "currencyName": "Zambian Kwacha",
                        "currencySymbol": "ZK",
                        "currencyFlag": "🇿🇲"
                    },
                    {
                        "currencyId": 58,
                        "currencyCode": "BWP",
                        "currencyName": "Botswana Pula",
                        "currencySymbol": "P",
                        "currencyFlag": "🇧🇼"
                    },
                    {
                        "currencyId": 690,
                        "currencyCode": "SCR",
                        "currencyName": "Seychellois Rupee",
                        "currencySymbol": "₨",
                        "currencyFlag": "🇸🇨"
                    },
                    {
                        "currencyId": 516,
                        "currencyCode": "NAD",
                        "currencyName": "Namibian Dollar",
                        "currencySymbol": "$",
                        "currencyFlag": "🇳🇦"
                    },
                    {
                        "currencyId": 480,
                        "currencyCode": "MUR",
                        "currencyName": "Mauritian Rupee",
                        "currencySymbol": "₨",
                        "currencyFlag": "🇲🇺"
                    },
                    {
                        "currencyId": 454,
                        "currencyCode": "MWK",
                        "currencyName": "Malawian Kwacha",
                        "currencySymbol": "MK",
                        "currencyFlag": "🇲🇼"
                    },
                    {
                        "currencyId": 270,
                        "currencyCode": "GMD",
                        "currencyName": "Gambian Dalasi",
                        "currencySymbol": "D",
                        "currencyFlag": "🇬🇲"
                    },
                    {
                        "currencyId": 748,
                        "currencyCode": "SZL",
                        "currencyName": "Swazi Lilangeni (Swaziland)",
                        "currencySymbol": "E",
                        "currencyFlag": "🇸🇿"
                    },
                    {
                        "currencyId": 646,
                        "currencyCode": "RWF",
                        "currencyName": "Rwandan Franc",
                        "currencySymbol": "RF",
                        "currencyFlag": "🇷🇼"
                    },
                    {
                        "currencyId": 426,
                        "currencyCode": "LSL",
                        "currencyName": "Lesotho Loti",
                        "currencySymbol": "L",
                        "currencyFlag": "🇱🇸"
                    },
                    {
                        "currencyId": 706,
                        "currencyCode": "SOS",
                        "currencyName": "Somali Shilling",
                        "currencySymbol": "Sh",
                        "currencyFlag": "🇸🇴"
                    },
                    {
                        "currencyId": 694,
                        "currencyCode": "SLL",
                        "currencyName": "Sierra Leonean Leone",
                        "currencySymbol": "Le",
                        "currencyFlag": "🇸🇱"
                    },
                    {
                        "currencyId": 943,
                        "currencyCode": "MZN",
                        "currencyName": "Mozambican Metical",
                        "currencySymbol": "MT",
                        "currencyFlag": "🇲🇿"
                    },
                    {
                        "currencyId": 132,
                        "currencyCode": "CVE",
                        "currencyName": "Cape Verdean Escudo",
                        "currencySymbol": "$",
                        "currencyFlag": "🇨🇻"
                    },
                    {
                        "currencyId": 108,
                        "currencyCode": "BIF",
                        "currencyName": "Burundian Franc",
                        "currencySymbol": "FBu",
                        "currencyFlag": "🇧🇮"
                    },
                    {
                        "currencyId": 262,
                        "currencyCode": "DJF",
                        "currencyName": "Djiboutian Franc",
                        "currencySymbol": "Fdj",
                        "currencyFlag": "🇩🇯"
                    },
                    {
                        "currencyId": 232,
                        "currencyCode": "ERN",
                        "currencyName": "Eritrean Nakfa",
                        "currencySymbol": "Nfk",
                        "currencyFlag": "🇪🇷"
                    },
                    {
                        "currencyId": 230,
                        "currencyCode": "ETB",
                        "currencyName": "Ethiopian Birr",
                        "currencySymbol": "Br",
                        "currencyFlag": "🇪🇹"
                    },
                    {
                        "currencyId": 174,
                        "currencyCode": "KMF",
                        "currencyName": "Comorian Franc",
                        "currencySymbol": "CF",
                        "currencyFlag": "🇰🇲"
                    },
                    {
                        "currencyId": 973,
                        "currencyCode": "AOA",
                        "currencyName": "Angolan Kwanza",
                        "currencySymbol": "Kz",
                        "currencyFlag": "🇦🇴"
                    },
                    {
                        "currencyId": 678,
                        "currencyCode": "STD",
                        "currencyName": "São Tomé and Príncipe Dobra",
                        "currencySymbol": "Db",
                        "currencyFlag": "🇸🇹"
                    },
                    {
                        "currencyId": 938,
                        "currencyCode": "SDG",
                        "currencyName": "Sudanese Pound",
                        "currencySymbol": "£SD",
                        "currencyFlag": "🇸🇩"
                    },
                    {
                        "currencyId": 932,
                        "currencyCode": "ZWL",
                        "currencyName": "Zimbabwean Dollar",
                        "currencySymbol": "Z$",
                        "currencyFlag": "🇿🇼"
                    },
                    {
                        "currencyId": 840,
                        "currencyCode": "USD",
                        "currencyName": "United States Dollar",
                        "currencySymbol": "$",
                        "currencyFlag": "🇺🇸"
                    },
                    {
                        "currencyId": 826,
                        "currencyCode": "GBP",
                        "currencyName": "British Pound Sterling",
                        "currencySymbol": "£",
                        "currencyFlag": "🇬🇧"
                    },
                    {
                        "currencyId": 784,
                        "currencyCode": "AED",
                        "currencyName": "United Arab Emirates Dirham",
                        "currencySymbol": "د.إ",
                        "currencyFlag": "🇦🇪"
                    },
                    {
                        "currencyId": 682,
                        "currencyCode": "SAR",
                        "currencyName": "Saudi Riyal",
                        "currencySymbol": "ر.س",
                        "currencyFlag": "🇸🇦"
                    },
                    {
                        "currencyId": 414,
                        "currencyCode": "KWD",
                        "currencyName": "Kuwaiti Dinar",
                        "currencySymbol": "د.ك",
                        "currencyFlag": "🇰🇼"
                    },
                    {
                        "currencyId": 634,
                        "currencyCode": "QAR",
                        "currencyName": "Qatari Riyal",
                        "currencySymbol": "ر.ق",
                        "currencyFlag": "🇶🇦"
                    },
                    {
                        "currencyId": 512,
                        "currencyCode": "OMR",
                        "currencyName": "Omani Rial",
                        "currencySymbol": "ر.ع",
                        "currencyFlag": "🇴🇲"
                    },
                    {
                        "currencyId": 48,
                        "currencyCode": "BHD",
                        "currencyName": "Bahraini Dinar",
                        "currencySymbol": "ب.د",
                        "currencyFlag": "🇧🇭"
                    },
                    {
                        "currencyId": 376,
                        "currencyCode": "ILS",
                        "currencyName": "Israeli New Shekel",
                        "currencySymbol": "₪",
                        "currencyFlag": "🇮🇱"
                    },
                    {
                        "currencyId": 364,
                        "currencyCode": "IRR",
                        "currencyName": "Iranian Rial",
                        "currencySymbol": "﷼",
                        "currencyFlag": "🇮🇷"
                    },
                    {
                        "currencyId": 949,
                        "currencyCode": "TRY",
                        "currencyName": "Turkish Lira",
                        "currencySymbol": "₺",
                        "currencyFlag": "🇹🇷"
                    },
                    {
                        "currencyId": 886,
                        "currencyCode": "YER",
                        "currencyName": "Yemeni Rial",
                        "currencySymbol": "﷼",
                        "currencyFlag": "🇾🇪"
                    },
                    {
                        "currencyId": 422,
                        "currencyCode": "LBP",
                        "currencyName": "Lebanese Pound",
                        "currencySymbol": "ل.ل",
                        "currencyFlag": "🇱🇧"
                    },
                    {
                        "currencyId": 400,
                        "currencyCode": "JOD",
                        "currencyName": "Jordanian Dinar",
                        "currencySymbol": "د.ا",
                        "currencyFlag": "🇯🇴"
                    },
                    {
                        "currencyId": 760,
                        "currencyCode": "SYP",
                        "currencyName": "Syrian Pound",
                        "currencySymbol": "£S",
                        "currencyFlag": "🇸🇾"
                    }
                ]
            }
           """;
    }
}
