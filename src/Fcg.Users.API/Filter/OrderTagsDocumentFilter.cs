using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Fcg.User.API.Filter
{
    public class OrderTagsDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var orderedTags = new List<OpenApiTag>
            {
                new OpenApiTag { Name = "Autenticação do Usuário", Description = "Endpoints de login e registro de novos usuários." },
                new OpenApiTag { Name = "Admin - Gerenciamento de Usuários", Description = "Endpoints administrativos para gerenciar perfis, ativação e reativação de contas." },
                new OpenApiTag { Name = "Minha Conta", Description = "Endpoints para o próprio usuário gerenciar seus dados cadastrais e conta." }
            };

            var existingTags = swaggerDoc.Tags != null ? swaggerDoc.Tags.ToList() : new List<OpenApiTag>();
            var finalTags = new List<OpenApiTag>(orderedTags);

            foreach (var tag in existingTags)
            {
                if (!finalTags.Any(t => t.Name.Equals(tag.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    finalTags.Add(tag);
                }
            }

            swaggerDoc.Tags = finalTags;
        }
    }
}
