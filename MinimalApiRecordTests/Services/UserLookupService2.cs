using MinimalApiRecordTests.Data;
using MinimalApiRecordTests.Models;
using static MinimalApiRecordTests.Services.UserLookupResult;

namespace MinimalApiRecordTests.Services;

public class UserLookupService2 : IService
{
    public static async Task<IResult> GetAllUsers([AsParameters] RangeFilter range, UserRepository repository, CancellationToken cancellationToken = default)
    {
        return await repository.GetAll(range, cancellationToken) switch
        {
            { Count: > 0 } users => Results.Ok(users),
            { } => Results.NotFound()
        };
    }

    public static async Task<UserLookupResult> FindUser(int id, UserRepository repository, CancellationToken cancellationToken = default)
    {
        return await repository.GetUser(id, cancellationToken) switch
        {
            { Count: 1 } users => new FoundOne(users[0]),
            { Count: > 1 } => new Error("Duplicate item found"),
            { } => new NotFound()
        };
    }

    public static async Task<UserLookupResult> SearchUser(string? firstName, string? lastName, [AsParameters] RangeFilter range, UserRepository repository, CancellationToken cancellationToken = default)
    {
        firstName = firstName?.Trim();
        lastName = lastName?.Trim();

        return await repository.FindUserByName(firstName, lastName, range, cancellationToken) switch
        {
            { Count: 1 } users => new FoundOne(users[0]),
            { Count: > 1 } users => new FoundMany(users),
            { } => new NotFound(),
            null => new Error("Invalid search criteria")
        };
    }

    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/users", GetAllUsers).WithTags(nameof(UserLookupService2)).WithName(nameof(GetAllUsers));
        app.MapGet("/users/{id}", FindUser).WithTags(nameof(UserLookupService2)).WithName(nameof(FindUser));
        app.MapGet("/users/search", SearchUser).WithTags(nameof(UserLookupService2)).WithName(nameof(SearchUser));
    }
}
