using Fcg.Core.Abstractions.Interfaces;
using Fcg.Usuario.API.Endpoint.Admin;
using Fcg.Usuario.API.Endpoint.Usuario;
using Fcg.Usuarios.Application.Common.Interfaces;
using Fcg.Usuarios.Application.Features.Usuarios.Commands.CadastrarUsuario;
using Fcg.Usuarios.Domain.Common.Interfaces;
using Fcg.Usuarios.Domain.Repositories.Interfaces;
using Fcg.Usuarios.Infrastructure.Persistance;
using Fcg.Usuarios.Infrastructure.Repository;
using Fcg.Usuarios.Infrastructure.Security;
using Fcg.Core.WebApi.Security;
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

var connectionString = builder.Configuration.GetConnectionString("UsuarioConnection");  

builder.Services.AddDbContext<UsuarioDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<UsuarioDbContext>(o =>
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
    cfg.RegisterServicesFromAssembly(typeof(CadastrarUsuarioCommand).Assembly);
});


builder.Services.AddValidatorsFromAssembly(typeof(CadastrarUsuarioCommand).Assembly);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));


builder.Services.AddScoped<IDbConnection>(sp=> sp.GetRequiredService<UsuarioDbContext>().Database.GetDbConnection());
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<UsuarioDbContext>();
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

var app = builder.Build();

#region Conta - Jogador/Admin
app.MapAcessoUsuarioEndpoint();
app.MapGerenciaContaEndpoints();
#endregion 

#region Admin - Gerenciamento de Usuários
app.MapGerenciaUsuarioEndpoints();
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
