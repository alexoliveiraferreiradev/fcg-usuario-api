using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;
using Fcg.Users.Domain.Entitites;
using Fcg.Users.Domain.Enum;
using Fcg.Users.Domain.ValueObjects;

namespace Fcg.Users.Domain.Tests.Entitites
{
    public class UserTests
    {
        private Name ObterNomeValido() => new Name("User Teste");
        private Email ObterEmailValido() => new Email("usuairio.teste@email.com");
        private Password ObterSenhaValida() => new Password("Password@123");

        #region Construtor Tests

        [Fact]
        public void Construtor_ComDadosValidos_DeveCriarUserAtivoComPerfilJogador()
        {
            // Arrange
            var Name = ObterNomeValido();
            var email = ObterEmailValido();
            var Password = ObterSenhaValida();

            // Act
            var User = new User(Name, email, Password);

            // Assert
            Assert.Equal(Name, User.Name);
            Assert.Equal(email, User.Email);
            Assert.Equal(Password, User.Password);
            Assert.Equal(UserRole.Player, User.Role);
            Assert.True(User.IsActive);
            Assert.Null(User.DeactivationReason);
            Assert.True((DateTime.UtcNow - User.CreatedAt).TotalSeconds < 5);
            Assert.Equal(User.CreatedAt, User.UpdatedAt);
        }

        [Fact]
        public void Construtor_SemNome_DeveLancarDomainException()
        {
            // Arrange
            Name nomeNulo = null!;
            var email = ObterEmailValido();
            var Password = ObterSenhaValida();

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => new User(nomeNulo, email, Password));
            Assert.Equal(MensagensDominio.UsuarioNomeObrigatorio, excecao.Message);
        }

        #endregion

        #region Deactivate Tests

        [Fact]
        public void Desativar_ComMotivo_DeveDesativarEMarcarMotivo()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());
            var reason = DeactivationReason.Inatividade;

            // Act
            User.Deactivate(reason);

            // Assert
            Assert.False(User.IsActive);
            Assert.Equal(reason, User.DeactivationReason);
            Assert.True((DateTime.UtcNow - User.UpdatedAt).TotalSeconds < 5);
        }

        [Fact]
        public void Desativar_UserJaDesativado_DeveLancarDomainException()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());
            User.Deactivate(DeactivationReason.SolicitacaoDoUser);

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => User.Deactivate(DeactivationReason.Inatividade));
            Assert.Equal(MensagensDominio.UsuarioJaDesativado, excecao.Message);
        }

        [Fact]
        public void DesativarConta_UserAtivo_DeveDesativar()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());

            // Act
            User.DeactivateAccount();

            // Assert
            Assert.False(User.IsActive);
            Assert.Null(User.DeactivationReason);
            Assert.True((DateTime.UtcNow - User.UpdatedAt).TotalSeconds < 5);
        }

        [Fact]
        public void DesativarConta_UserJaDesativado_DeveLancarDomainException()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());
            User.DeactivateAccount();

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => User.DeactivateAccount());
            Assert.Equal(MensagensDominio.UsuarioJaDesativado, excecao.Message);
        }

        #endregion

        #region Update Tests

        [Fact]
        public void Atualizar_ComDadosValidos_DeveAtualizarCampos()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());
            var newName = new Name("Jane Doe");
            var newPassword = new Password("NewStrongPassword456!");

            // Act
            User.Update(newName, newPassword);

            // Assert
            Assert.Equal(newName, User.Name);
            Assert.Equal(newPassword, User.Password);
            Assert.True((DateTime.UtcNow - User.UpdatedAt).TotalSeconds < 5);
        }

        [Fact]
        public void Atualizar_UserInativo_DeveLancarDomainException()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());
            User.DeactivateAccount();

            var newName = new Name("Jane Doe");
            var newPassword = new Password("NewStrongPassword456!");

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => User.Update(newName, newPassword));
            Assert.Equal(MensagensDominio.UsuarioInativo, excecao.Message);
        }

        #endregion

        #region Role Tests

        [Fact]
        public void PromoverPerfil_UserJogador_DeveAlterarPerfilParaAdministrador()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());

            // Act
            User.PromoteRole();

            // Assert
            Assert.Equal(UserRole.Admin, User.Role);
            Assert.True((DateTime.UtcNow - User.UpdatedAt).TotalSeconds < 5);
        }

        [Fact]
        public void RebaixarPerfil_UserAdministrador_DeveAlterarPerfilParaJogador()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());
            User.PromoteRole(); 

            // Act
            User.DemoteRole();

            // Assert
            Assert.Equal(UserRole.Player, User.Role);
        }

        [Fact]
        public void RebaixarPerfil_UserJaJogador_DeveLancarDomainException()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => User.DemoteRole());
            Assert.Equal(MensagensDominio.UsuarioPerfilRebaixarInvalido, excecao.Message);
        }

        #endregion

        #region Reactivate Tests

        [Fact]
        public void Reativar_UserInativo_DeveReativarELimparMotivo()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());
            User.Deactivate(DeactivationReason.Inatividade);

            // Act
            User.Reactivate();

            // Assert
            Assert.True(User.IsActive);
            Assert.Null(User.DeactivationReason);
            Assert.True((DateTime.UtcNow - User.UpdatedAt).TotalSeconds < 5);
        }

        [Fact]
        public void Reativar_UserAtivo_DeveLancarDomainException()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => User.Reactivate());
            Assert.Equal(MensagensDominio.UsuarioAtivo, excecao.Message);
        }

        #endregion
    }
}
