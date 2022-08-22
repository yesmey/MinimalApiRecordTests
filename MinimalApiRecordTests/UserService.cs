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

public class UserService
{
    private readonly User[] _users;

    public UserService()
    {
        _users = GenerateUsers();

        User[] GenerateUsers()
        {
            var firstNames = new[] { "Willian", "Noah", "Alice", "Hugo", "Liam", "Alma" };
            var lastNames = new[] { "Andersson", "Johansson", "Karlsson", "Nilsson", "Eriksson", "Olsson", "Persson" };
            int id = 1;
            return firstNames
                .SelectMany(firstName => lastNames.Select(lastName => new User(id++, firstName, lastName)))
                .Append(new User(id, "Duplicate", "Duplicate"))
                .ToArray();
        }
    }

    public IEnumerable<User> AllUsers => _users;

    public IEnumerable<User> DuplicateUsers => _users
        .GroupBy(x => x.Id)
        .Where(x => x.Count() > 1)
        .Select(x => x.First());

    public UserLookupResult FindUser(int id)
    {
        try
        {
            var user = _users.SingleOrDefault(x => x.Id == id);
            if (user == null)
            {
                return new NotFound();
            }

            return new Ok(user);
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
