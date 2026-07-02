using System.ComponentModel;

namespace Fcg.Users.Domain.Enum
{
    public enum DeactivationReason
    {
        [Description("Solicitado pelo usuário")]
        SolicitacaoDoUser = 1,
        [Description("Inatividade prolongada")]
        Inatividade = 2,
        [Description("Violação dos Termos de Uso")]
        ViolacaoTermos = 3,
        [Description("Comportamento tóxico ou inadequado")]
        ComportamentoInadequado = 4,
        [Description("Uso de trapaças ou softwares de terceiros (Cheat/Bot)")]
        FraudeOuTrapaca = 5,
        [Description("Duplicidade de conta")]
        ContaDuplicada = 6,
        [Description("Outros motivos")]
        Outros = 99
    }
}
