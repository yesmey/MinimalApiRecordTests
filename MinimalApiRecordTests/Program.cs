using MinimalApiRecordTests;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<UserLookupService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/users", (UserRepository repository) => repository.AllUsers).WithName("AllUsers");
app.MapGet("/users/duplicates", (UserRepository repository) => repository.DuplicateUsers).WithName("GetDuplicates");
app.MapUserLookupEndpoints();

app.Run();
