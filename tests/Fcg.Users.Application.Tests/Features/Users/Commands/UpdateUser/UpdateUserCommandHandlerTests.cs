using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Users.Application.Features.Users.Commands.UpdateUser;
using Fcg.Users.Domain.Common.Interfaces;
using Fcg.Users.Domain.Entitites;
using Fcg.Users.Domain.Repositories.Interfaces;
using Fcg.Users.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fcg.Users.Application.Tests.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ILogger<UpdateUserCommandHandler>> _loggerMock = new();

        private UpdateUserCommandHandler CreateHandler() => new(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);

        private static User BuildUser() =>
            new(new Name("User Teste"), new Email("teste@teste.com"), new Password("SenhaForte@123"));

        [Fact]
        public async Task Handle_ShouldUpdateUserAndReturnResponse_WhenDataIsValid()
        {
            var user = BuildUser();
            var command = new UpdateUserCommand(user.Id, "Novo Nome", "SenhaForte@123", "SenhaForte@123");

            _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.CheckNameInUseAsync(user.Id, command.Name)).ReturnsAsync(false);
            _passwordHasherMock.Setup(h => h.HashPassword(command.Password)).Returns("HashedPassword@123");
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

            var handler = CreateHandler();

            var result = await handler.Handle(command, CancellationToken.None);

            result.Name.Should().Be("Novo Nome");
            result.Email.Should().Be(user.Email.Value);
            user.Name.Value.Should().Be("Novo Nome");
            user.Password.Hash.Should().Be("HashedPassword@123");
            _userRepositoryMock.Verify(r => r.Update(user), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenUserIsNotFound()
        {
            var command = new UpdateUserCommand(Guid.NewGuid(), "Novo Nome", "SenhaForte@123", "SenhaForte@123");

            _userRepositoryMock.Setup(r => r.GetByIdAsync(command.UserId)).ReturnsAsync((User?)null);

            var handler = CreateHandler();

            var act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<DomainException>();
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenNameIsAlreadyInUse()
        {
            var user = BuildUser();
            var command = new UpdateUserCommand(user.Id, "Nome Em Uso", "SenhaForte@123", "SenhaForte@123");

            _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.CheckNameInUseAsync(user.Id, command.Name)).ReturnsAsync(true);

            var handler = CreateHandler();

            var act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<DomainException>();
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenUserIsInactive()
        {
            var user = BuildUser();
            user.DeactivateAccount();
            var command = new UpdateUserCommand(user.Id, "Novo Nome", "SenhaForte@123", "SenhaForte@123");

            _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.CheckNameInUseAsync(user.Id, command.Name)).ReturnsAsync(false);
            _passwordHasherMock.Setup(h => h.HashPassword(command.Password)).Returns("HashedPassword@123");

            var handler = CreateHandler();

            var act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<DomainException>();
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }
    }
}
