using System.ComponentModel;

namespace Fcg.Users.Domain.Enum
{
    public enum DeactivationReason
    {
        [Description("Solicitado pelo usuário")]
        UserRequested = 1,
        [Description("Inatividade prolongada")]
        Inactivity = 2,
        [Description("Violação dos Termos de Uso")]
        TermsViolation = 3,
        [Description("Comportamento tóxico ou inadequado")]
        InappropriateBehavior = 4,
        [Description("Uso de trapaças ou softwares de terceiros (Cheat/Bot)")]
        FraudOrCheating = 5,
        [Description("Duplicidade de conta")]
        DuplicateAccount = 6,
        [Description("Outros motivos")]
        Other = 99
    }
}
