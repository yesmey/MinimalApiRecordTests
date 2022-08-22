using MinimalApiRecordTests;
using static MinimalApiRecordTests.UserLookupResult;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<UserLookupService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/users", (UserRepository repository) => repository.AllUsers).WithName("AllUsers");
app.MapGet("/users/duplicates", (UserRepository repository) => repository.DuplicateUsers).WithName("GetDuplicates");

app.MapGet("/users/{id}", (UserLookupService userService, int id) =>
{
    return userService.FindUser(id) switch
    {
        Ok { User: var user } => Results.Ok(user.ToString()),
        NotFound => Results.NotFound(),
        Error error => Results.Problem(error.ProblemDetails),
        _ => Results.StatusCode(StatusCodes.Status501NotImplemented)
    };
}).WithName("GetUser");

app.Run();
