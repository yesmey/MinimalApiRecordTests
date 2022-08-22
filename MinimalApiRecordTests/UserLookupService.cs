using Microsoft.AspNetCore.Mvc;
using static MinimalApiRecordTests.UserLookupResult;

namespace MinimalApiRecordTests;

public record UserLookupResult
{
    private UserLookupResult() { }

    public record Ok(User User) : UserLookupResult;
    public record NotFound() : UserLookupResult;
    public record Error(ProblemDetails ProblemDetails) : UserLookupResult;
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
                User user => new Ok(user),
                _ => new NotFound()
            };
        }
        catch (Exception ex)
        {
            return new Error(new ProblemDetails
            {
                Title = $"Error in {nameof(FindUser)}",
                Status = StatusCodes.Status500InternalServerError,
                Detail = ex.Message
            });
        }
    }
}
