using System.Collections.Immutable;

namespace MinimalApiRecordTests;

public class UserRepository
{
    private readonly ImmutableArray<User> _users = GenerateUsers();

    public IEnumerable<User> AllUsers => _users;

    public IEnumerable<User> DuplicateUsers => _users
        .GroupBy(x => x.Id)
        .Where(x => x.Count() > 1)
        .Select(x => x.First());

    internal ImmutableArray<User> GetUser(int id)
    {
        return _users.Where(x => x.Id == id).ToImmutableArray();
    }

    internal ImmutableArray<User>? FindUserByName(string? firstName, string? lastName)
    {
        return (string.IsNullOrEmpty(firstName), string.IsNullOrEmpty(lastName)) switch
        {
            (false, false) => _users.Where(x => x.FirstName == firstName && x.LastName == lastName).ToImmutableArray(),
            (false, true) => _users.Where(x => x.FirstName == firstName).ToImmutableArray(),
            (true, false) => _users.Where(x => x.LastName == lastName).ToImmutableArray(),
            (true, true) => null
        };
    }

    private static ImmutableArray<User> GenerateUsers()
    {
        int id = 1;

        var builder = ImmutableArray.CreateBuilder<User>();
        foreach (var firstName in new[] { "William", "Noah", "Alice", "Hugo", "Liam", "Alma" })
        {
            foreach (var lastName in new[] { "Andersson", "Johansson", "Karlsson", "Nilsson", "Eriksson", "Olsson", "Persson" })
                builder.Add(new(id++, firstName, lastName));
        }
        builder.Add(new(id, "Duplicate", "Duplicate"));
        builder.Add(new(id, "Duplicate", "Duplicate"));
        return builder.ToImmutable();
    }
}
