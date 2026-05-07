namespace Fcg.Usuarios.Domain.Constants
{
    public class MensagensDominio
    {
        #region Usuario
        public static string UsuarioNomeObrigatorio = "O nome do usuário é obrigatório.";
        public static string UsuarioNomeAntigoObrigatorio = "O nome antigo do usuário é obrigatório.";
        public static string UsuarioNomeNovoObrigatorio = "O novo nome do usuário é obrigatório.";
        public static string UsuarioEmailObrigatorio = "O email do usuário é obrigatório.";
        public static string EmailNaoReal = "Por favor, informe um email real.";
        public static string EmailInvalido = "O email é inválido.";
        public static string EmailTamanhoInvalido = "O tamanho do email é inválido.";
        public static string UsuarioSenhaObrigatoria = "A senha do usuário é obrigatória.";
        public static string UsuarioConfirmacaoSenhaObrigatoria = "A confirmação de senha do usuário é obrigatória.";
        public static string UsuarioTamanhoNomeInvalido = "O nome do usuário deve conter entre 3 e 20 caracteres.";
        public static string UsuarioSenhaFraca = "A senha deve conter pelo menos 8 caracteres, incluindo letras maiúsculas, minúsculas, números e caracteres especiais.";
        public static string UsuarioSenhaConfirmacaoDiferente = "A senha e a confirmação de senha devem ser iguais.";
        public static string UsuarioInativo = "O usuário deve estar ativo";
        public static string UsuarioAtivo = "O usuário deve estar inativo";
        public static string UsuarioJaDesativado = "O usuário já foi desativado.";
        public static string UsuarioEmailAntigoObrigatorio = "O email antigo do usuário é obrigatório.";
        public static string UsuarioEmailAntigoInvalido = "O email antigo do usuário é inválido.";
        public static string UsuarioEmailNovoObrigatorio = "O novo email do usuário é obrigatório.";
        public static string UsuarioEmailNovoInvalido = "O novo email do usuário é inválido.";
        public static string UsuarioSenhaAntigaObrigatoria = "A senha antiga do usuário é obrigatória.";
        public static string UsuarioSenhaAntigaFraca = "A senha antiga do usuário é fraca.";
        public static string UsuarioSenhaNovaObrigatoria = "A senha nova do usuário é obrigatória.";
        public static string UsuarioSenhaNovaFraca = "A senha nova do usuário é fraca.";
        public static string UsuarioNaoEncontrado = "O usuário não foi encontrado.";
        public static string AdminNaoEncontrado = "O administrador não foi encontrado.";
        public static string UsuarioPerfilRebaixarInvalido = "O perfil do usuário não pode ser rebaixado para jogador, pois ele já é um jogador.";
        public static string UsuarioPerfilAdministradoInvalido = "O perfil do usuário pode ser promovido para administrador, pois ele já é um administrador.";
        public static string CrendenciasInvalidas = "Credenciais inválidas.";
        public static string EmailJaCadastrado = "O email já foi cadastrado.";
        public static string NomeUsuarioJaCadastrado = "O nome de usuário já foi cadastrado.";
        public static string NomeNaoReal = "Por favor, informe o seu nome real.";
        public static string OperacaoDesativarInvalida = "Operação inválida: você não pode deletar seu próprio perfil.";
        public static string OperacaoRebaixarInvalida = "Operação inválida: você não pode rebaixar seu próprio perfil.";
        public static string OperacaoDesativarAdminInvalida = "Não é possível desativar o último administrador.";
        public static string SenhaTamanhoInvalido = "O tamanho da senha é inválido.";
        #endregion
    }
}
