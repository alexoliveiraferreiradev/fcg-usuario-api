using Dapper;
using Fcg.Users.Domain.ValueObjects;
using System.Data;

namespace Fcg.Users.Infrastructure.DapperHandlers
{
    public class SenhaTypeHandler : SqlMapper.TypeHandler<Senha>
    {
        public override Senha? Parse(object value)
        {
            var valorStr = value?.ToString() ?? string.Empty;
            return new Senha(valorStr);
        }

        public override void SetValue(IDbDataParameter parameter, Senha? value)
        {
            parameter.Value = value?.Hash ?? (object)DBNull.Value;
        }
    }
}
