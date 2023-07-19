using Microsoft.Extensions.DependencyInjection;

namespace AppCore.Configs;

public static class RegisterService
{
    public static IServiceCollection RegisAllService(this IServiceCollection services, string[] projects,
        string[] ignoreProjects = null)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => x.FullName != null && projects.Any(z => x.FullName.Split(",")[0] == z))
            .ToList();
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes().Where(x =>
                    x.Name.EndsWith("Service") ||
                    x.Name.EndsWith("SubService") ||
                    x.Name.EndsWith("Repository") ||
                    x.Name.EndsWith("SubRepository") ||
                    x.Name.EndsWith("UnitOfWork")
                )
                .ToList();
            if (!types.Any())
                continue;
            if (ignoreProjects != null && ignoreProjects.Any())
                types = types.Where(x => !ignoreProjects.Contains(x.Name)).ToList();

            foreach (var type in types)
            {
                if (type.BaseType != null)
                {
                    var interfaceType = type.GetInterfaces().Except(type.BaseType.GetInterfaces()).FirstOrDefault();

                    if (interfaceType != null)
                    {
                        services.AddScoped(interfaceType, type);
                    }
                }
            }
        }

        return services;
    }
}