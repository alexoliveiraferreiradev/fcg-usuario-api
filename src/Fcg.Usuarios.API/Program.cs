using Fcg.Usuarios.Domain.Repositories.Interfaces;
using Fcg.Usuarios.Infrastructure.Persistance;
using Fcg.Usuarios.Infrastructure.Repository;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<UsuarioDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("UsuarioConnection"));
});

builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<UsuarioDbContext>(o =>
    {
        o.UseSqlServer();
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));
    });
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});


builder.Services.AddScoped<UsuarioDbContext>();
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

var app = builder.Build();

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

app.MapControllers();

app.Run();
