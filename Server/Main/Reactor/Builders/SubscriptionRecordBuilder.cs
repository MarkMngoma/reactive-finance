using Server.Main.Reactor.Builders.Tables.Generated.Models;

namespace Server.Main.Reactor.Builders;

public class SubscriptionRecordBuilder
{

  private readonly SubscriptionsDto _dto = new();

public SubscriptionRecordBuilder WithBusinessId(ulong businessId)
  {
    _dto.BusinessId = businessId;
    return this;
  }

  public SubscriptionRecordBuilder WithFinancialProductId(ulong financialProductId)
  {
    _dto.FinancialProductId = financialProductId;
    return this;
  }

  public SubscriptionRecordBuilder WithStatus(string status)
  {
    _dto.Status = status;
    return this;
  }

  public SubscriptionRecordBuilder WithStartDate(DateTime startDate)
  {
    _dto.StartDate = startDate;
    return this;
  }

  public SubscriptionRecordBuilder WithEndDate(DateTime? endDate)
  {
    _dto.EndDate = endDate;
    return this;
  }

  public SubscriptionRecordBuilder WithDescription(DateTime endDate)
  {
    _dto.EndDate = endDate;
    return this;
  }

    public SubscriptionRecordBuilder WithCreatedAt(DateTime createdAt)
  {
    _dto.CreatedAt = createdAt;
    return this;
  }

  public SubscriptionRecordBuilder WithUpdatedAt(DateTime? updatedAt)
  {
    _dto.UpdatedAt = updatedAt;
    return this;
  }

  public SubscriptionsDto Build()
  {
    return _dto;
  }
}
