using Dapper;
using Fcg.Users.Domain.ValueObjects;
using System.Data;

namespace Fcg.Users.Infrastructure.Queries.DapperHandlers
{
    public class PasswordTypeHandler : SqlMapper.TypeHandler<Password>
    {
        public override Password? Parse(object value)
        {
            var valorStr = value?.ToString() ?? string.Empty;
            return new Password(valorStr);
        }

        public override void SetValue(IDbDataParameter parameter, Password? value)
        {
            parameter.Value = value?.Hash ?? (object)DBNull.Value;
        }
    }
}
