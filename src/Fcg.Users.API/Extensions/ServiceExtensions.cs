using Dapper;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.WebApi.Security;
using Fcg.Users.Application.Common.Interfaces;
using Fcg.Users.Application.Extensions;
using Fcg.Users.Application.Features.Users.Commands.RegisterUser;
using Fcg.Users.Domain.Common.Interfaces;
using Fcg.Users.Domain.Repositories.Interfaces;
using Fcg.Users.Infrastructure.Extensions;
using Fcg.Users.Infrastructure.MessageBroker;
using Fcg.Users.Infrastructure.Persistence;
using Fcg.Users.Infrastructure.Queries;
using Fcg.Users.Infrastructure.Queries.DapperHandlers;
using Fcg.Users.Infrastructure.Repository;
using Fcg.Users.Infrastructure.Security;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Data;
using System.Text;

namespace Fcg.User.API.Extensions
{
    public static class ServiceExtensions
    {
        public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
        {
            builder.AddAuthorizationExtension()
                .AddSerilogExtension()
                .AddSwaggerExtension();

            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddAplicationServices();                     
            return builder;
        }
      
    }
}
