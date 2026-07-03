using Fcg.User.API.Filters;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Fcg.User.API.Extensions
{
    public static class SwaggerExtensions
    {
        public static WebApplicationBuilder AddSwaggerService(this WebApplicationBuilder builder)
        {
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Fiap Cloud Games - API de Usuários",
                    Version = "v1",
                    Description = "API de Gestão de Identidade e Usuários. Responsável pelo cadastro, autenticação e geração de tokens JWT para o ecossistema FCG.",
                    Contact = new OpenApiContact
                    {
                        Name = "Alex Oliveira Ferreira",
                        Email = "alexoliveiraferreiradev@gmail.com", 
                        Url = new Uri("https://github.com/alexoliveiraferreiradev")
                    }
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                var xmlFileApp = "Fcg.Users.Application.xml";
                var xmlPathApp = Path.Combine(AppContext.BaseDirectory, xmlFileApp);

                if (File.Exists(xmlPathApp))
                {
                    options.IncludeXmlComments(xmlPathApp);
                }

                options.SchemaFilter<EnumSchemaFilter>();
                options.DocumentFilter<OrderTagsDocumentFilter>();

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Insira o token JWT: Bearer {Seu Token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                 {
                     {
                         new OpenApiSecurityScheme
                         {
                             Reference = new OpenApiReference
                             {
                                 Type = ReferenceType.SecurityScheme,
                                 Id = "Bearer"
                             }
                         },
                         Array.Empty<string>()
                     }
                 });
            });
            return builder;
        }

        public static WebApplication UseSwaggerExtension(this WebApplication app) {

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fiap Cloud Games - API de Usuários");
            });

            return app;
        }
    }
}
