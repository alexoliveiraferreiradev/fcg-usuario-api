using Fcg.User.API.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
builder.AddServices();
var app = builder.Build();
await app.AddAplicationExtension();
app.MapHealthChecks("/health/liveness", new HealthCheckOptions { Predicate = check => check.Tags.Contains("live") });
app.MapHealthChecks("/health/readiness", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });
app.Run();
