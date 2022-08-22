namespace MinimalApiRecordTests;

public record User(int Id, string FirstName, string LastName)
{
    public override string ToString()
    {
        return $"({Id}) {FirstName} {LastName}";
    }
}
