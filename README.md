# MinimalApiRecordTests
Playground for some new dotnet features

-------------------------

### EF Core Commands

**Compile models:**

`dotnet ef dbcontext optimize --output-dir Data/Compiled --namespace MinimalApiRecordTests.Data.Compiled`

**Add migrations:**

`dotnet ef migrations add InitialCreate --output-dir Data/Migrations --namespace MinimalApiRecordTests.Data.Migrations`

**Update database:**

`dotnet ef database update`
