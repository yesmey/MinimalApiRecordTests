using Microsoft.EntityFrameworkCore;
using MinimalApiRecordTests.Data;
using MinimalApiRecordTests.Data.Compiled;
using MinimalApiRecordTests.Data.Model;
using MinimalApiRecordTests.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextPool<DataContext>(o => o.UseModel(DataContextModel.Instance).UseSqlite("Data Source=test.db"));

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserLookupService>();
builder.Services.AddScoped<UserLookupService2>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/data/seed/{numberOfUsers}", async (DataContext context, int numberOfUsers) =>
{
    var firstNames = new[] { "William", "Noah", "Alice", "Hugo", "Liam", "Alma" };
    var lastNames = new[] { "Andersson", "Johansson", "Karlsson", "Nilsson", "Eriksson", "Olsson", "Persson" };

    await context.Database.EnsureDeletedAsync();
    await context.Database.EnsureCreatedAsync();
    context.Users.AddRange(Enumerable.Range(1, numberOfUsers).Select(i => new UserData
    {
        Id = i,
        FirstName = firstNames[Random.Shared.Next(firstNames.Length)],
        LastName = lastNames[Random.Shared.Next(lastNames.Length)]
    }));
    await context.SaveChangesAsync();
});

app.MapUserLookupEndpoints();

// invoke static MapEndpoints on all IService, just a hacky way to test
_ = AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(assembly => assembly.GetTypes())
    .Where(type => typeof(IService).IsAssignableFrom(type) && type.IsClass)
    .Select(x => x.GetMethod(nameof(IService.MapEndpoints), BindingFlags.Public | BindingFlags.Static).Invoke(null, new[] { app } ))
    .ToArray();

app.Run();
