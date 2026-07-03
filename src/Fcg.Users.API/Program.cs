using Fcg.User.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddServices();
var app = builder.Build();
app.AddAplicationExtension();
app.Run();
