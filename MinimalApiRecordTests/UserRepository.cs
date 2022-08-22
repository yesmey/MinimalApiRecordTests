namespace MinimalApiRecordTests;

public class UserRepository
{
    private readonly User[] _users = GenerateUsers();

    public IEnumerable<User> AllUsers => _users;

    public IEnumerable<User> DuplicateUsers => _users
        .GroupBy(x => x.Id)
        .Where(x => x.Count() > 1)
        .Select(x => x.First());

    internal User? GetUser(int id)
    {
        return _users.SingleOrDefault(x => x.Id == id);
    }

    private static User[] GenerateUsers()
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
