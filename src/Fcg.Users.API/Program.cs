using Fcg.User.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddServices();
var app = builder.Build();
await app.AddAplicationExtension();
app.Run();
