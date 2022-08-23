using static MinimalApiRecordTests.UserLookupResult;

namespace MinimalApiRecordTests;

public record UserLookupResult
{
    private UserLookupResult() { }
    public record FoundOne(User User) : UserLookupResult;
    public record FoundMany(IReadOnlyList<User> Users) : UserLookupResult;
    public record NotFound() : UserLookupResult;
    public record Error(string ErrorMessage, Exception? Exception = null) : UserLookupResult;
}

public class UserLookupService
{
    private readonly UserRepository _userRepository;
    public UserLookupService(UserRepository userRepository) => _userRepository = userRepository;

    public UserLookupResult FindUser(int id)
    {
        try
        {
            return _userRepository.GetUser(id) switch
            {
                { Length: 1 } users => new FoundOne(users[0]),
                { Length: > 1 } => new Error("Duplicate item found"),
                { } => new NotFound()
            };
        }
        catch (Exception ex)
        {
            return new Error("Unknown error", ex);
        }
    }

    public UserLookupResult SearchUser(string? firstName, string? lastName)
    {
        try
        {
            firstName = firstName?.Trim();
            lastName = lastName?.Trim();

            return _userRepository.FindUserByName(firstName, lastName) switch
            {
                { Length: 1 } users => new FoundOne(users[0]),
                { Length: > 1 } users => new FoundMany(users),
                { } => new NotFound(),
                null => new Error("Invalid search criteria")
            };
        }
        catch (Exception ex)
        {
            return new Error("Unknown error", ex);
        }
    }
}

public static class UserLookupServiceExtensions
{
    public static void MapUserLookupEndpoints(this WebApplication app)
    {
        app.MapGet("/users/{id}", GetUser).WithName(nameof(GetUser));
        app.MapGet("/users/search", SearchUser).WithName(nameof(SearchUser));

        static IResult GetUser(UserLookupService userService, int id)
        {
            return userService.FindUser(id) switch
            {
                FoundOne { User: var user } => Results.Ok(user),
                NotFound => Results.NotFound(),
                Error error => Results.BadRequest(error.ErrorMessage),
                { } or null => Results.StatusCode(StatusCodes.Status501NotImplemented)
            };
        }

        static IResult SearchUser(UserLookupService userService, string? firstName, string? lastName)
        {
            return userService.SearchUser(firstName, lastName) switch
            {
                FoundOne { User: var user } => Results.Ok(user),
                FoundMany { Users: var users } => Results.Ok(users),
                NotFound => Results.NotFound(),
                Error error => Results.BadRequest(error.ErrorMessage),
                { } or null => Results.StatusCode(StatusCodes.Status501NotImplemented)
            };
        }
    }
}
