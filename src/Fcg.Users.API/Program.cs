using Dapper;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.WebApi.Security;
using Fcg.User.API.Endpoint.Admin;
using Fcg.User.API.Endpoint.User;
using Fcg.Users.Application.Common.Interfaces;
using Fcg.Users.Application.Features.Users.Commands.CadastrarUser;
using Fcg.Users.Domain.Common.Interfaces;
using Fcg.Users.Domain.Repositories.Interfaces;
using Fcg.Users.Infrastructure.DapperHandlers;
using Fcg.Users.Infrastructure.Persistance;
using Fcg.Users.Infrastructure.Repository;
using Fcg.Users.Infrastructure.Security;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSwaggerGen(options=> {
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FIAP Cloud Games API",
        Version = "v1",
        Description = "API para gestão de catálogo de jogos e processamento de pedidos.",
        Contact = new OpenApiContact
        {
            Name = "Alex Oliveira Ferreira"
        }
    });
});

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("UserConnection");  

builder.Services.AddDbContext<UserDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<UserDbContext>(o =>
    {
        o.UseSqlServer();
        o.UseBusOutbox();
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly);
});


builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserCommand).Assembly);

SqlMapper.AddTypeHandler(new NameTypeHandler());
SqlMapper.AddTypeHandler(new EmailTypeHandler());
SqlMapper.AddTypeHandler(new PasswordTypeHandler());

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddAuthorization();

builder.Services.AddScoped<IDbConnection>(sp=> sp.GetRequiredService<UserDbContext>().Database.GetDbConnection());
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<UserDbContext>();
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

#region Conta - Player/Admin
app.MapAcessoUserEndpoint();
app.MapGerenciaContaEndpoints();
#endregion 

#region Admin - Gerenciamento de Usuários
app.MapGerenciaUserEndpoints();
#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fiap Cloud Games");
});

app.UseHttpsRedirection();

app.UseAuthorization();



app.Run();
