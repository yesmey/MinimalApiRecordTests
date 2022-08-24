namespace MinimalApiRecordTests.Model;

public class User
{
    public int Id { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }

    public override string ToString()
    {
        return $"({Id}) {FirstName} {LastName}";
    }
}