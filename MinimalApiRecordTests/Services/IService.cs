using System.Reflection;

namespace MinimalApiRecordTests.Services;

public interface IService
{
    static abstract void MapEndpoints(IEndpointRouteBuilder app);
}

public static class ServiceExtensions
{
    public static IEndpointRouteBuilder MapServiceEndpoints(this IEndpointRouteBuilder app)
    {
        var serviceClasses = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && typeof(IService).IsAssignableFrom(type));

        foreach (var serviceClass in serviceClasses)
        {
            serviceClass.GetMethod(nameof(IService.MapEndpoints), BindingFlags.Public | BindingFlags.Static)!.Invoke(null, new[] { app });
        }

        return app;
    }
}