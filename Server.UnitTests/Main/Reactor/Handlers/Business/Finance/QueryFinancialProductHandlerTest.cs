using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Server.Main.Reactor.Builders.Tables.Generated;
using Server.Main.Reactor.Builders.Tables.Generated.Models;
using Server.Main.Reactor.Handlers.Business.Finance;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Handlers.Domain;
using Server.Main.Reactor.Models.Dto.Queries;

namespace Server.UnitTests.Main.Reactor.Handlers.Business.Finance;

[TestFixture]
public class QueryFinancialProductHandlerTest : ReactiveTest
{
  private Mock<IFinancialProductDomainHandler> _financialProductDomainHandlerMock;
  private Mock<ICachingHandler> _cachingHandlerMock;
  private QueryFinancialProductHandler _handlerUnderTest;
  private TestScheduler _testScheduler;

  [SetUp]
  public void Setup()
  {
    _testScheduler = new TestScheduler();
    _financialProductDomainHandlerMock = new Mock<IFinancialProductDomainHandler>();
    _cachingHandlerMock = new Mock<ICachingHandler>();
    _handlerUnderTest = new QueryFinancialProductHandler(_financialProductDomainHandlerMock.Object, _cachingHandlerMock.Object);
  }

  [Test]
  public void Handle_WithCachedData_ReturnsCachedData()
  {
    // Given -- cache consists of data with id#123
    const string id = "123";
    var request = new QueryFinancialProductDto { Id = id };
    var financialProductDto = new FinancialProductDto { Id = id };
    _cachingHandlerMock
      .Setup(c => c.HandleGet<FinancialProductDto>(FinancialProductTable.TableName, id))
      .Returns(Observable.Return(financialProductDto));

    // When -- a handler is invoked
    _testScheduler.Schedule(() => _handlerUnderTest.Handle(request).Subscribe(result =>
    {
      Assert.That(result, Is.Not.Null);
      Assert.That(JObject.FromObject(result).GetValue("id"), Is.Not.Null);
      Assert.That(JsonRendererUtil.ConvertAndRender<FinancialProductDto>(result).Id, Is.EqualTo(id));

      // Then -- confirm 0 domain handler invocations
      _financialProductDomainHandlerMock.Verify(x => x.SelectFinancialProductUsingId(It.IsAny<string>()), Times.Never);
    }));
    _testScheduler.Start();
  }

  [Test]
  public void Handle_WithNoCachedData_QueriesDatabaseAndCaches()
  {
    // Given -- consists of data with id#123 but yet to write it to cache
    const string id = "123";
    var request = new QueryFinancialProductDto { Id = id };
    var financialProductDto = new FinancialProductDto { Id = id };

    _cachingHandlerMock
      .Setup(c => c.HandleGet<FinancialProductDto>(It.IsAny<string>(),It.IsAny<string>()))
      .Returns(Observable.Empty<FinancialProductDto>());

    _financialProductDomainHandlerMock
      .Setup(d => d.SelectFinancialProductUsingId(It.IsAny<string>()))
      .Returns(Observable.Return(financialProductDto));

    _cachingHandlerMock
      .Setup(c => c.HandleWrite(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<FinancialProductDto>()))
      .Returns(Observable.Return(financialProductDto));

    // When -- a handler is invoked
    _testScheduler.Schedule(() => _handlerUnderTest.Handle(request).Subscribe(result =>
    {
      Assert.That(result, Is.Not.Null);
      Assert.That(JObject.FromObject(result).GetValue("id"), Is.Not.Null);
      Assert.That(JsonRendererUtil.ConvertAndRender<FinancialProductDto>(result).Id, Is.EqualTo(id));

      // Then -- confirm 1 domain read and cache write handler invocations each
      _cachingHandlerMock.Verify(x => x.HandleGet<FinancialProductDto>(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      _financialProductDomainHandlerMock.Verify(x => x.SelectFinancialProductUsingId(It.IsAny<string>()), Times.Once);
      _cachingHandlerMock.Verify(x => x.HandleWrite(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<FinancialProductDto>()), Times.Once);
    }));
    _testScheduler.Start();
  }

  [Test]
  public void HandleCollection_WithCachedData_ReturnsCachedData()
  {
    // Given -- consists of data items
    var data = new List<FinancialProductDto>
    {
      new FinancialProductDto { Id = "1" },
      new FinancialProductDto { Id = "2" }
    };

    _cachingHandlerMock
      .Setup(c => c.HandleGet<IEnumerable<FinancialProductDto>>(FinancialProductTable.TableName, "fp-collection"))
      .Returns(Observable.Return(data));

    // When -- a handler is invoked
    _testScheduler.Schedule(() => _handlerUnderTest.Handle().Subscribe(result =>
    {
      Assert.That(result, Is.Not.Null);
      Assert.That(JsonRendererUtil.ConvertAndRender<List<FinancialProductDto>>(result).Count, Is.EqualTo(2));
      Assert.That(JsonRendererUtil.ConvertAndRender<List<FinancialProductDto>>(result).First(dto => "1".Equals(dto.Id)).Id, Is.EqualTo("1"));
      Assert.That(JsonRendererUtil.ConvertAndRender<List<FinancialProductDto>>(result).First(dto => "2".Equals(dto.Id)).Id, Is.EqualTo("2"));

      // Then -- confirm 0 domain handler invocations
      _financialProductDomainHandlerMock.Verify(x => x.SelectFinancialProducts(), Times.Never);
    }));
    _testScheduler.Start();
  }

  [Test]
  public void HandleCollection_WithNoCachedData_QueriesAndCaches()
  {
    // Given -- consists of data items
    var financialProductList = new List<FinancialProductDto>
    {
      new FinancialProductDto { Id = "1" }
    };

    _cachingHandlerMock
      .Setup(c => c.HandleGet<IEnumerable<FinancialProductDto>>(It.IsAny<string>(), It.IsAny<string>()))
      .Returns(Observable.Empty<IEnumerable<FinancialProductDto>>());

    _financialProductDomainHandlerMock
      .Setup(x => x.SelectFinancialProducts())
      .Returns(Observable.Return(financialProductList));

    _cachingHandlerMock
      .Setup(x => x.HandleWrite(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<FinancialProductDto>>()))
      .Returns(Observable.Return(financialProductList));

    // When -- a handler is invoked
    _testScheduler.Schedule(() => _handlerUnderTest.Handle().Subscribe(result =>
    {
      Assert.That(result, Is.Not.Null);
      Assert.That(JsonRendererUtil.ConvertAndRender<List<FinancialProductDto>>(result).Count, Is.EqualTo(1));
      Assert.That(JsonRendererUtil.ConvertAndRender<List<FinancialProductDto>>(result).First(dto => "1".Equals(dto.Id)).Id, Is.EqualTo("1"));

      // Then -- confirm 1 domain read and cache write handler invocations each
      _financialProductDomainHandlerMock.Verify(x => x.SelectFinancialProducts(), Times.Once);
      _cachingHandlerMock.Verify(x => x.HandleWrite(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<FinancialProductDto>>()),
        Times.Once);
    }));
    _testScheduler.Start();
  }
}
