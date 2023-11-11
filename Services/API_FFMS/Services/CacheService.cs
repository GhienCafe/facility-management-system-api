using System.Text.Json;
using AppCore.Extensions;
using StackExchange.Redis;

namespace API_FFMS.Services;

public interface ICacheService
{
    T? GetData<T>(string key);
    bool SetData<T>(string key, T value, DateTimeOffset expirationTime);
    object RemoveData(string key);
}

public class CacheService : ICacheService
{
    private readonly IDatabase _cacheDb;

    public CacheService(IDatabase cacheDb)
    {
        _cacheDb = cacheDb;
    }

    public T? GetData<T>(string key)
    {
        var value = _cacheDb.StringGet(key);
        if (!string.IsNullOrEmpty(value))
            return JsonSerializer.Deserialize<T>(value);

        return default;
    }

    public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
    {
        var expiredTime = expirationTime.DateTime.Subtract(DateTime.Now);
        var isSet = _cacheDb.StringSet(key, JsonSerializer.Serialize(value), expiredTime);
        return isSet;
    }

    public object RemoveData(string key)
    {
        var _exist = _cacheDb.KeyExists(key);
        if(_exist)
            return _cacheDb. KeyDelete(key);
        return false;
    }
}