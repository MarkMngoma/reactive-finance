using System.Reactive.Linq;
using System.Text.Json;
using Hazelcast;
using log4net;

namespace Server.Main.Reactor.Handlers.CrossCutting;

public interface ICachingHandler
{
  IObservable<T> HandleWrite<T>(string index, string key, T value);
  IObservable<T?> HandleGet<T>(string index, string key);
  IObservable<T> HandleEviction<T>(string index, string key, T value);
}
public class CachingHandler : ICachingHandler
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(CachingHandler));

  private readonly IHazelcastClient _hazelcastClient;

  public CachingHandler(IHazelcastClient hazelcastClient)
  {
    _hazelcastClient = hazelcastClient;
  }

  public IObservable<T> HandleWrite<T>(string index, string key, T value)
  {
    return Observable.FromAsync(async () =>
    {
      var map = await _hazelcastClient.GetMapAsync<string, byte[]>(index);
      var bytes = JsonSerializer.SerializeToUtf8Bytes(value);
      await map.SetAsync(key, bytes);
      return value;
    });
  }

  public IObservable<T?> HandleGet<T>(string index, string key)
  {
    return Observable.FromAsync(async () =>
    {
      var map = await _hazelcastClient.GetMapAsync<string, byte[]>(index);
      var bytes = await map.GetAsync(key);
      Logger.Debug($"CachingHandler@HandleGet - Cache hit '{key}' in map index '{index}'");
      return bytes == null ? default : JsonSerializer.Deserialize<T>(bytes);
    });
  }

  public IObservable<T> HandleEviction<T>(string index, string key, T value)
  {
    return Observable.FromAsync(async () =>
    {
      var map = await _hazelcastClient.GetMapAsync<string, byte[]>(index);
      Logger.Debug($"CachingHandler@HandleEviction - Evicting key '{key}' from map index '{index}'");
      await map.DeleteAsync(key);
      return value;
    });
  }
}
