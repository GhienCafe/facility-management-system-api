
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
}
