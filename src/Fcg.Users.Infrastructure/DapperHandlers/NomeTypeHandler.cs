using Dapper;
using Fcg.Users.Domain.ValueObjects;
using System.Data;

namespace Fcg.Users.Infrastructure.DapperHandlers
{
    public class NomeTypeHandler : SqlMapper.TypeHandler<Nome>
    {
        public override Nome? Parse(object value)
        {
            var valorStr = value?.ToString() ?? string.Empty;
            return new Nome(valorStr); 
        }

        public override void SetValue(IDbDataParameter parameter, Nome? value)
        {
            parameter.Value = value?.Valor ?? (object)DBNull.Value;
        }
    }
}
