using MinimalApiRecordTests.Data;
using MinimalApiRecordTests.Models;

namespace MinimalApiRecordTests.Services;

public class UserLookupService2 : IService
{
    public static async Task<IResult> GetAllUsers2([AsParameters] RangeFilter range, UserRepository repository, CancellationToken cancellationToken = default)
    {
        try
        {
            return await repository.GetAll(range, cancellationToken) switch
            {
                { Count: > 0 } users => Results.Ok(users),
                { } => Results.NotFound()
            };
        }
        catch
        {
            return Results.BadRequest();
        }
    }

    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/users2", GetAllUsers2).WithTags(nameof(UserLookupService2)).WithName(nameof(GetAllUsers2));
    }
}
