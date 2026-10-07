using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Users.Application.Features.Admin.Commands.PromoteUserToAdmin;
using Fcg.Users.Domain.Entitites;
using Fcg.Users.Domain.Enum;
using Fcg.Users.Domain.Repositories.Interfaces;
using Fcg.Users.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fcg.Users.Application.Tests.Features.Admin.Commands.PromoteUserToAdmin
{
    public class PromoteUserToAdminCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ILogger<PromoteUserToAdminCommandHandler>> _loggerMock = new();

        private PromoteUserToAdminCommandHandler CreateHandler() => new(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);

        private static User BuildUser() =>
            new(new Name("User Teste"), new Email("teste@teste.com"), new Password("SenhaForte@123"));

        [Fact]
        public async Task Handle_ShouldPromoteUserToAdminAndReturnResponse_WhenUserIsActivePlayer()
        {
            var user = BuildUser();
            var command = new PromoteUserToAdminCommand(user.Id, Guid.NewGuid());

            _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

            var handler = CreateHandler();

            var result = await handler.Handle(command, CancellationToken.None);

            user.Role.Should().Be(UserRole.Admin);
            result.PerfilUser.Should().Be(UserRole.Admin);
            _userRepositoryMock.Verify(r => r.Update(user), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenUserIsNotFound()
        {
            var command = new PromoteUserToAdminCommand(Guid.NewGuid(), Guid.NewGuid());

            _userRepositoryMock.Setup(r => r.GetByIdAsync(command.Id)).ReturnsAsync((User?)null);

            var handler = CreateHandler();

            var act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<DomainException>();
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenOperatorTriesToPromoteSelf()
        {
            var user = BuildUser();
            var command = new PromoteUserToAdminCommand(user.Id, user.Id);

            _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

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
            var command = new PromoteUserToAdminCommand(user.Id, Guid.NewGuid());

            _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

            var handler = CreateHandler();

            var act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<DomainException>();
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenUserIsAlreadyAdmin()
        {
            var user = BuildUser();
            user.PromoteRole();
            var command = new PromoteUserToAdminCommand(user.Id, Guid.NewGuid());

            _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

            var handler = CreateHandler();

            var act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<DomainException>();
        }
    }
}
