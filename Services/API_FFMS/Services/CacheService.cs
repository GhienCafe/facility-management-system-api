using System.Text.Json;
using AppCore.Extensions;
using StackExchange.Redis;

namespace API_FFMS.Services;

public interface ICacheService
{
    T? GetData<T>(string key);
    bool SetData<T>(string key, T value, DateTimeOffset? expirationTime);
    object RemoveData(string key);
}

public class CacheService : ICacheService
{
    private readonly IDatabase _cacheDb;
    internal Guid? AccountId;
    internal readonly IHttpContextAccessor HttpContextAccessor;

    public CacheService(IDatabase cacheDb, IHttpContextAccessor httpContextAccessor)
    {
        _cacheDb = cacheDb;
        HttpContextAccessor = httpContextAccessor;
        AccountId = httpContextAccessor.HttpContext?.User.GetUserId();
    }

    public T? GetData<T>(string key)
    {
        var value = _cacheDb.StringGet(key+AccountId);
        if (!string.IsNullOrEmpty(value))
            return JsonSerializer.Deserialize<T>(value);

        return default;
    }

    public bool SetData<T>(string key, T value, DateTimeOffset? expirationTime)
    {
        var expiredTime = expirationTime.HasValue
            ? expirationTime.Value.DateTime.Subtract(DateTime.Now)
            : TimeSpan.FromMinutes(5); // Default expiration of 5 minutes
        
        var isSet = _cacheDb.StringSet(key+AccountId, JsonSerializer.Serialize(value), expiredTime);
        return isSet;
    }

    public object RemoveData(string key)
    {
        var _exist = _cacheDb.KeyExists(key+AccountId);
        if(_exist)
            return _cacheDb. KeyDelete(key+AccountId);
        return false;
    }
}