using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AppCore.Configs;

public class ConfigureSwaggerOption : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiDescriptionGroupCollectionProvider _groupCollectionProvider;
    private readonly IApiVersionDescriptionProvider _versionProvider;

    public ConfigureSwaggerOption(IApiDescriptionGroupCollectionProvider groupCollectionProvider,
        IApiVersionDescriptionProvider versionProvider)
    {
        _groupCollectionProvider = groupCollectionProvider;
        _versionProvider = versionProvider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        // add swagger document for every API version discovered

        foreach (var group in _groupCollectionProvider.ApiDescriptionGroups.Items.Select(x => x.GroupName))
        {
            foreach (var description in _versionProvider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(group, CreateVersionInfo(group, description));
            }
        }

        var securityScheme = new OpenApiSecurityScheme
        {
            Description = "JWT Authorization using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        };

        options.AddSecurityDefinition("Bearer", securityScheme);

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { securityScheme, new[] { "Bearer" } }
        });
    }

    public void Configure(string name, SwaggerGenOptions options)
    {
        Configure(options);
    }

    private static OpenApiInfo CreateVersionInfo(string group, ApiVersionDescription description)
    {
        var info = new OpenApiInfo
        {
            Title = $"{Assembly.GetEntryAssembly()?.GetName().Name} - {group}",
            Version = $"{group}-{description.GroupName}"
        };

        if (description.IsDeprecated)
        {
            info.Description += " This API version has been deprecated.";
        }

        return info;
    }
}