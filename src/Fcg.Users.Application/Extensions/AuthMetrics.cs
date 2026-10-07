using Prometheus;

namespace Fcg.Users.Application.Extensions
{
    public class AuthMetrics
    {
        public static readonly Counter LoginAttemps = Metrics.CreateCounter(
            "auth_login_attempts_total",
            "Número total de tentativas de login.",
            new CounterConfiguration
            {
                LabelNames = new[] { "result" }
            });

        public static readonly Counter RegisterAttemps = Metrics.CreateCounter(
            "auth_register_attempts_total",
            "Número total de tentativas de cadastro.",
            new CounterConfiguration
            {
                LabelNames = new[] { "result" }
            });

        public static readonly Counter DeactiveAttemps = Metrics.CreateCounter(
            "auth_deactive_attempts_total",
            "Número total de tentativas de desativação de conta.",
            new CounterConfiguration
            {
                LabelNames = new[] { "result" }
            });

        public static readonly Counter UpdateAttemps = Metrics.CreateCounter(
            "auth_update_attempts_total",
            "Número total de tentativas de atualização de informações do usuário.",
            new CounterConfiguration
            {
                LabelNames = new[] { "result" }
            });

        public static readonly Counter UserRegistrations = Metrics.CreateCounter(
            "user_registered_total",
            "Número total de usuários registrados.");

        public static readonly Counter AdminActions = Metrics.CreateCounter(
            "user_admin_actions_total",
            "Ações administrativas realizadas sobre contas de usuário.",
            new CounterConfiguration
            {
                LabelNames = new[] { "action","result" }
            });

    }
}
