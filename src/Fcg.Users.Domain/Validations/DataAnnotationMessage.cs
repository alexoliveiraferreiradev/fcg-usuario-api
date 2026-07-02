namespace Fcg.Users.Domain.Validations
{
    public class DataAnnotationMessage : Attribute
    {
        public const string ErroCaracteres = "O campo {0} precisa ter entre {2} e {1} caracteres";
        public const string ErroRequired = "O campo {0} é obrigatório";
        public const string ErroFormato = "O campo {0} está em formato incorreto";
        public const string ErroConfirmacaoSenha = "O campo {0} é diferente da Password";
        public const string ErroNomeReal = "Por favor, insira um Name real.";
        public const string ErroEmail = "E-mails do domínio @example.com não são permitidos.";
    }
}
