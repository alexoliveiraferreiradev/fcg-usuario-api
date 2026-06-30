using Dapper;
using Fcg.Usuarios.Domain.ValueObjects;
using System.Data;

namespace Fcg.Usuarios.Infrastructure.DapperHandlers
{
    public class EmailTypeHandler : SqlMapper.TypeHandler<Email>
    {
        public override Email? Parse(object value)
        {
            var valorStr = value?.ToString() ?? string.Empty;
            return new Email(valorStr);
        }

        public override void SetValue(IDbDataParameter parameter, Email? value)
        {
            parameter.Value = value?.Valor ?? (object)DBNull.Value;
        }
    }
}
