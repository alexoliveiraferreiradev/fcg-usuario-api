using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;
using Fcg.Users.Domain.Entitites;
using Fcg.Users.Domain.Enum;
using Fcg.Users.Domain.ValueObjects;

namespace Fcg.Users.Domain.Tests.Entitites
{
    public class UserTests
    {
        private Nome ObterNomeValido() => new Nome("User Teste");
        private Email ObterEmailValido() => new Email("usuairio.teste@email.com");
        private Senha ObterSenhaValida() => new Senha("Senha@123");

        #region Construtor Tests

        [Fact]
        public void Construtor_ComDadosValidos_DeveCriarUserAtivoComPerfilJogador()
        {
            // Arrange
            var nome = ObterNomeValido();
            var email = ObterEmailValido();
            var senha = ObterSenhaValida();

            // Act
            var User = new User(nome, email, senha);

            // Assert
            Assert.Equal(nome, User.NomeUser);
            Assert.Equal(email, User.EmailUser);
            Assert.Equal(senha, User.Senha);
            Assert.Equal(TipoUser.Jogador, User.Perfil);
            Assert.True(User.Ativo);
            Assert.Null(User.MotivoDesativacao);
            Assert.True((DateTime.UtcNow - User.DataCadastro).TotalSeconds < 5);
            Assert.Equal(User.DataCadastro, User.DataAlteracao);
        }

        [Fact]
        public void Construtor_SemNome_DeveLancarDomainException()
        {
            // Arrange
            Nome nomeNulo = null!;
            var email = ObterEmailValido();
            var senha = ObterSenhaValida();

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => new User(nomeNulo, email, senha));
            Assert.Equal(MensagensDominio.UsuarioNomeObrigatorio, excecao.Message);
        }

        #endregion

        #region Desativar Tests

        [Fact]
        public void Desativar_ComMotivo_DeveDesativarEMarcarMotivo()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());
            var motivo = MotivoDesativacao.Inatividade;

            // Act
            User.Desativar(motivo);

            // Assert
            Assert.False(User.Ativo);
            Assert.Equal(motivo, User.MotivoDesativacao);
            Assert.True((DateTime.UtcNow - User.DataAlteracao).TotalSeconds < 5);
        }

        [Fact]
        public void Desativar_UserJaDesativado_DeveLancarDomainException()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());
            User.Desativar(MotivoDesativacao.SolicitacaoDoUser);

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => User.Desativar(MotivoDesativacao.Inatividade));
            Assert.Equal(MensagensDominio.UsuarioJaDesativado, excecao.Message);
        }

        [Fact]
        public void DesativarConta_UserAtivo_DeveDesativar()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());

            // Act
            User.DesativarConta();

            // Assert
            Assert.False(User.Ativo);
            Assert.Null(User.MotivoDesativacao);
            Assert.True((DateTime.UtcNow - User.DataAlteracao).TotalSeconds < 5);
        }

        [Fact]
        public void DesativarConta_UserJaDesativado_DeveLancarDomainException()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());
            User.DesativarConta();

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => User.DesativarConta());
            Assert.Equal(MensagensDominio.UsuarioJaDesativado, excecao.Message);
        }

        #endregion

        #region Atualizar Tests

        [Fact]
        public void Atualizar_ComDadosValidos_DeveAtualizarCampos()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());
            var novoNome = new Nome("Jane Doe");
            var novaSenha = new Senha("NewStrongPassword456!");

            // Act
            User.Atualizar(novoNome, novaSenha);

            // Assert
            Assert.Equal(novoNome, User.NomeUser);
            Assert.Equal(novaSenha, User.Senha);
            Assert.True((DateTime.UtcNow - User.DataAlteracao).TotalSeconds < 5);
        }

        [Fact]
        public void Atualizar_UserInativo_DeveLancarDomainException()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());
            User.DesativarConta();

            var novoNome = new Nome("Jane Doe");
            var novaSenha = new Senha("NewStrongPassword456!");

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => User.Atualizar(novoNome, novaSenha));
            Assert.Equal(MensagensDominio.UsuarioInativo, excecao.Message);
        }

        #endregion

        #region Perfil Tests

        [Fact]
        public void PromoverPerfil_UserJogador_DeveAlterarPerfilParaAdministrador()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());

            // Act
            User.PromoverPerfil();

            // Assert
            Assert.Equal(TipoUser.Administrador, User.Perfil);
            Assert.True((DateTime.UtcNow - User.DataAlteracao).TotalSeconds < 5);
        }

        [Fact]
        public void RebaixarPerfil_UserAdministrador_DeveAlterarPerfilParaJogador()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());
            User.PromoverPerfil(); 

            // Act
            User.RebaixarPerfil();

            // Assert
            Assert.Equal(TipoUser.Jogador, User.Perfil);
        }

        [Fact]
        public void RebaixarPerfil_UserJaJogador_DeveLancarDomainException()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => User.RebaixarPerfil());
            Assert.Equal(MensagensDominio.UsuarioPerfilRebaixarInvalido, excecao.Message);
        }

        #endregion

        #region Reativar Tests

        [Fact]
        public void Reativar_UserInativo_DeveReativarELimparMotivo()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());
            User.Desativar(MotivoDesativacao.Inatividade);

            // Act
            User.Reativar();

            // Assert
            Assert.True(User.Ativo);
            Assert.Null(User.MotivoDesativacao);
            Assert.True((DateTime.UtcNow - User.DataAlteracao).TotalSeconds < 5);
        }

        [Fact]
        public void Reativar_UserAtivo_DeveLancarDomainException()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => User.Reativar());
            Assert.Equal(MensagensDominio.UsuarioAtivo, excecao.Message);
        }

        #endregion
    }
}
