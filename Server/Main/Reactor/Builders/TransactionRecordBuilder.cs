using Server.Main.Reactor.Builders.Tables.Generated.Models;

namespace Server.Main.Reactor.Builders;

public class TransactionRecordBuilder
{
  private readonly TransactionsDto _dto = new();

  public TransactionRecordBuilder WithTransactionId(string transactionId)
  {
    _dto.TransactionId = transactionId;
    return this;
  }

  public TransactionRecordBuilder WithSubscriptionId(ulong subscriptionId)
  {
    _dto.SubscriptionId = subscriptionId;
    return this;
  }

  public TransactionRecordBuilder WithType(string type)
  {
    _dto.Type = type;
    return this;
  }

  public TransactionRecordBuilder WithAmount(decimal amount)
  {
    _dto.Amount = amount;
    return this;
  }

  public TransactionRecordBuilder WithCurrency(string currency)
  {
    _dto.Currency = currency;
    return this;
  }

  public TransactionRecordBuilder WithStatus(string status)
  {
    _dto.Status = status;
    return this;
  }

  public TransactionRecordBuilder WithAuthorizationId(string authorizationId)
  {
    _dto.AuthorizationId = authorizationId;
    return this;
  }

  public TransactionRecordBuilder WithTransactionDate(DateTime transactionDate)
  {
    _dto.TransactionDate = transactionDate;
    return this;
  }

  public TransactionRecordBuilder WithDescription(string description)
  {
    _dto.Description = description;
    return this;
  }

  public TransactionRecordBuilder WithCreatedAt(DateTime createdAt)
  {
    _dto.CreatedAt = createdAt;
    return this;
  }

  public TransactionRecordBuilder WithUpdatedAt(DateTime? updatedAt)
  {
    _dto.UpdatedAt = updatedAt;
    return this;
  }

  public TransactionsDto Build()
  {
    return _dto;
  }
}
