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
        public void Constructor_ShouldCreateActiveUserWithPlayerRole_WhenDataIsValid()
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
        public void Constructor_ShouldThrowDomainException_WhenNameIsNull()
        {
            // Arrange
            Name nomeNulo = null!;
            var email = ObterEmailValido();
            var Password = ObterSenhaValida();

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => new User(nomeNulo, email, Password));
            Assert.Equal(DomainMessages.UserNameRequired, excecao.Message);
        }

        #endregion

        #region Deactivate Tests

        [Fact]
        public void Deactivate_ShouldDeactivateAndSetReason_WhenReasonIsProvided()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());
            var reason = DeactivationReason.Inactivity;

            // Act
            User.Deactivate(reason);

            // Assert
            Assert.False(User.IsActive);
            Assert.Equal(reason, User.DeactivationReason);
            Assert.True((DateTime.UtcNow - User.UpdatedAt).TotalSeconds < 5);
        }

        [Fact]
        public void Deactivate_ShouldThrowDomainException_WhenUserIsAlreadyDeactivated()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());
            User.Deactivate(DeactivationReason.UserRequested);

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => User.Deactivate(DeactivationReason.Inactivity));
            Assert.Equal(DomainMessages.UserAlreadyDeactivated, excecao.Message);
        }

        [Fact]
        public void DeactivateAccount_ShouldDeactivate_WhenUserIsActive()
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
        public void DeactivateAccount_ShouldThrowDomainException_WhenUserIsAlreadyDeactivated()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());
            User.DeactivateAccount();

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => User.DeactivateAccount());
            Assert.Equal(DomainMessages.UserAlreadyDeactivated, excecao.Message);
        }

        #endregion

        #region Update Tests

        [Fact]
        public void Update_ShouldUpdateFields_WhenDataIsValid()
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
        public void Update_ShouldThrowDomainException_WhenUserIsInactive()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());
            User.DeactivateAccount();

            var newName = new Name("Jane Doe");
            var newPassword = new Password("NewStrongPassword456!");

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => User.Update(newName, newPassword));
            Assert.Equal(DomainMessages.UserMustBeActive, excecao.Message);
        }

        #endregion

        #region Role Tests

        [Fact]
        public void PromoteRole_ShouldChangeRoleToAdmin_WhenUserIsPlayer()
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
        public void DemoteRole_ShouldChangeRoleToPlayer_WhenUserIsAdmin()
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
        public void DemoteRole_ShouldThrowDomainException_WhenUserIsAlreadyPlayer()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => User.DemoteRole());
            Assert.Equal(DomainMessages.UserProfileDemoteInvalid, excecao.Message);
        }

        #endregion

        #region Reactivate Tests

        [Fact]
        public void Reactivate_ShouldReactivateAndClearReason_WhenUserIsInactive()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());
            User.Deactivate(DeactivationReason.Inactivity);

            // Act
            User.Reactivate();

            // Assert
            Assert.True(User.IsActive);
            Assert.Null(User.DeactivationReason);
            Assert.True((DateTime.UtcNow - User.UpdatedAt).TotalSeconds < 5);
        }

        [Fact]
        public void Reactivate_ShouldThrowDomainException_WhenUserIsActive()
        {
            // Arrange
            var User = new User(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => User.Reactivate());
            Assert.Equal(DomainMessages.UserMustBeInactive, excecao.Message);
        }

        #endregion
    }
}
