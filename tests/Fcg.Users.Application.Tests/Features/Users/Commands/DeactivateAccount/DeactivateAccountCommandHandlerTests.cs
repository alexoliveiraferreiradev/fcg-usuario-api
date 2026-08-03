using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Users.Application.Features.Users.Commands.DeactivateAccount;
using Fcg.Users.Domain.Entitites;
using Fcg.Users.Domain.Repositories.Interfaces;
using Fcg.Users.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fcg.Users.Application.Tests.Features.Users.Commands.DeactivateAccount
{
    public class DeactivateAccountCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ILogger<DesativarContaCommandHandler>> _loggerMock = new();

        private DesativarContaCommandHandler CreateHandler() => new(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);

        private static User BuildUser(bool asAdmin = false)
        {
            var user = new User(new Name("User Teste"), new Email("teste@teste.com"), new Password("SenhaForte@123"));
            if (asAdmin) user.PromoteRole();
            return user;
        }

        [Fact]
        public async Task Handle_ShouldDeactivateUser_WhenUserIsPlayer()
        {
            var user = BuildUser();
            var command = new DeactiveAccountCommand(user.Id);

            _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

            var handler = CreateHandler();

            await handler.Handle(command, CancellationToken.None);

            user.IsActive.Should().BeFalse();
            _userRepositoryMock.Verify(r => r.Update(user), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldDeactivateUser_WhenUserIsAdminAndThereAreOtherAdmins()
        {
            var user = BuildUser(asAdmin: true);
            var command = new DeactiveAccountCommand(user.Id);

            _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.HasMultipleAdminsAsync()).ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

            var handler = CreateHandler();

            await handler.Handle(command, CancellationToken.None);

            user.IsActive.Should().BeFalse();
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenUserIsNotFound()
        {
            var command = new DeactiveAccountCommand(Guid.NewGuid());

            _userRepositoryMock.Setup(r => r.GetByIdAsync(command.Id)).ReturnsAsync((User?)null);

            var handler = CreateHandler();

            var act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<DomainException>();
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenUserIsTheOnlyAdmin()
        {
            var user = BuildUser(asAdmin: true);
            var command = new DeactiveAccountCommand(user.Id);

            _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.HasMultipleAdminsAsync()).ReturnsAsync(false);

            var handler = CreateHandler();

            var act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<DomainException>();
            user.IsActive.Should().BeTrue();
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }
    }
}
