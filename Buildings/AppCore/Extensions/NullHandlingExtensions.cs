
namespace AppCore.Extensions;

public static class NullHandlingExtensions
{
    public static TResult SafeOrDefault<TSource, TResult>(
        this TSource source,
        Func<TSource, TResult> selector)
    {
        if (source == null)
        {
            return default(TResult);
        }

        var value = selector(source);
        if (value == null && typeof(TResult).IsEnum)
        {
            return default(TResult);
        }

        return value;
    }

    public static void CheckNullOrEmpty(object entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        var properties = entity.GetType().GetProperties();

        foreach (var property in properties)
        {
            if (property.CanRead && property.CanWrite)
            {
                var value = property.GetValue(entity);
                if (value == null)
                {
                    var propertyType = property.PropertyType;
                    if (propertyType == typeof(string))
                    {
                        property.SetValue(entity, "");
                    }
                    else if (propertyType == typeof(int))
                    {
                        property.SetValue(entity, 0);
                    }
                    else if (propertyType == typeof(double))
                    {
                        property.SetValue(entity, 0.0);
                    }
                }
            }
        }
    }

}
