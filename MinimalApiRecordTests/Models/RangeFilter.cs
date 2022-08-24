using Microsoft.AspNetCore.Mvc;

namespace MinimalApiRecordTests.Models;

public record RangeFilter([FromQuery] int? Skip, [FromQuery] int? Take);
