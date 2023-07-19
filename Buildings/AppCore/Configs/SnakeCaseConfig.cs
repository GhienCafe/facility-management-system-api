using System.Globalization;
using System.Text.Json;
using AppCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AppCore.Configs;

public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return name.ToSnake();
    }
}

public class SnakeCaseQueryValueProvider : QueryStringValueProvider, IValueProvider
{
    public SnakeCaseQueryValueProvider(
        BindingSource bindingSource,
        IQueryCollection values,
        CultureInfo culture)
        : base(bindingSource, values, culture)
    {
    }

    public override bool ContainsPrefix(string prefix)
    {
        return base.ContainsPrefix(prefix.ToSnake());
    }

    public override ValueProviderResult GetValue(string key)
    {
        return base.GetValue(key.ToSnake());
    }
}

public class SnakeCaseQueryValueProviderFactory : IValueProviderFactory
{
    public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var valueProvider = new SnakeCaseQueryValueProvider(
            BindingSource.Query,
            context.ActionContext.HttpContext.Request.Query,
            CultureInfo.CurrentCulture);

        context.ValueProviders.Add(valueProvider);

        return Task.CompletedTask;
    }
}

public class SnakeCasingParameterOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null) operation.Parameters = new List<OpenApiParameter>();
        else
        {
            foreach (var item in operation.Parameters)
            {
                item.Name = item.Name.ToSnake();
            }
        }
    }
}

public class UrlRenameDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var keyValuePairs = new OpenApiPaths();
        foreach (var (key, value) in swaggerDoc.Paths)
        {
            var newKey = key.ToSnake();
            keyValuePairs.Add(newKey, value);
        }

        swaggerDoc.Paths = keyValuePairs;
    }
}