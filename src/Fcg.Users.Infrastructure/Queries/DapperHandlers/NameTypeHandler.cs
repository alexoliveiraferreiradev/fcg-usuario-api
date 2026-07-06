using Dapper;
using Fcg.Users.Domain.ValueObjects;
using System.Data;

namespace Fcg.Users.Infrastructure.Queries.DapperHandlers
{
    public class NameTypeHandler : SqlMapper.TypeHandler<Name>
    {
        public override Name? Parse(object value)
        {
            var valorStr = value?.ToString() ?? string.Empty;
            return new Name(valorStr); 
        }

        public override void SetValue(IDbDataParameter parameter, Name? value)
        {
            parameter.Value = value?.Valor ?? (object)DBNull.Value;
        }
    }
}
