using MinimalApiRecordTests;
using static MinimalApiRecordTests.UserLookupResult;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<UserService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/users", (UserService userService) =>
{
    return userService.AllUsers;
})
.WithName("AllUsers");

app.MapGet("/users/duplicates", (UserService userService) =>
{
    return userService.DuplicateUsers;
})
.WithName("GetDuplicates");

app.MapGet("/users/{id}", (UserService userService, int id) =>
{
    return userService.FindUser(id) switch
    {
        Ok { User: var (userId, firstName, lastName) } => Results.Ok($"({userId}) {firstName}, {lastName}"),
        NotFound => Results.NotFound(),
        Error error => Results.Problem(error.ProblemDetails),
        _ => Results.StatusCode(StatusCodes.Status400BadRequest)
    };
})
.WithName("GetUser");

app.Run();
