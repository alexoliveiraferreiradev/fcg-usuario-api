using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Users.Application.Features.Admin.Commands.DemoteUserToPlayer;
using Fcg.Users.Domain.Entitites;
using Fcg.Users.Domain.Enum;
using Fcg.Users.Domain.Repositories.Interfaces;
using Fcg.Users.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fcg.Users.Application.Tests.Features.Admin.Commands.DemoteUserToPlayer
{
    public class DemoteUserToPlayerCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ILogger<DemoteUserToPlayerCommandHandler>> _loggerMock = new();

        private DemoteUserToPlayerCommandHandler CreateHandler() => new(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);

        private static User BuildAdminUser()
        {
            var user = new User(new Name("User Teste"), new Email("teste@teste.com"), new Password("SenhaForte@123"));
            user.PromoteRole();
            return user;
        }

        [Fact]
        public async Task Handle_ShouldDemoteUserToPlayerAndReturnResponse_WhenUserIsActiveAdmin()
        {
            var user = BuildAdminUser();
            var command = new DemoteUserToPlayerCommand(user.Id, Guid.NewGuid());

            _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

            var handler = CreateHandler();

            var result = await handler.Handle(command, CancellationToken.None);

            user.Role.Should().Be(UserRole.Player);
            result.PerfilUser.Should().Be(UserRole.Player);
            _userRepositoryMock.Verify(r => r.Update(user), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenOperatorTriesToDemoteSelf()
        {
            var operatorId = Guid.NewGuid();
            var command = new DemoteUserToPlayerCommand(operatorId, operatorId);

            var handler = CreateHandler();

            var act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<DomainException>();
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenUserIsNotFound()
        {
            var command = new DemoteUserToPlayerCommand(Guid.NewGuid(), Guid.NewGuid());

            _userRepositoryMock.Setup(r => r.GetByIdAsync(command.Id)).ReturnsAsync((User?)null);

            var handler = CreateHandler();

            var act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<DomainException>();
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenUserIsInactive()
        {
            var user = BuildAdminUser();
            user.DeactivateAccount();
            var command = new DemoteUserToPlayerCommand(user.Id, Guid.NewGuid());

            _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

            var handler = CreateHandler();

            var act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<DomainException>();
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenUserIsAlreadyPlayer()
        {
            var user = new User(new Name("User Teste"), new Email("teste@teste.com"), new Password("SenhaForte@123"));
            var command = new DemoteUserToPlayerCommand(user.Id, Guid.NewGuid());

            _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

            var handler = CreateHandler();

            var act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<DomainException>();
        }
    }
}
