using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.SharedContracts.Interfaces;
using Fcg.Users.Application.Features.Admin.Commands.DeactivateUser;
using Fcg.Users.Domain.Entitites;
using Fcg.Users.Domain.Enum;
using Fcg.Users.Domain.Repositories.Interfaces;
using Fcg.Users.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fcg.Users.Application.Tests.Features.Admin.Commands.DeactivateUser
{
    public class DeactivateUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ILogger<DeactivateUserCommandHandler>> _loggerMock = new();
        private readonly Mock<IIntegrationEventPublisher> _integrationEventPublisherMock = new();   
        private DeactivateUserCommandHandler CreateHandler() => new(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object,
            _integrationEventPublisherMock.Object);

        private static User BuildUser() =>
            new(new Name("User Teste"), new Email("teste@teste.com"), new Password("SenhaForte@123"));

        [Fact]
        public async Task Handle_ShouldDeactivateUser_WhenOperatorIsDifferentFromTarget()
        {
            var user = BuildUser();
            var command = new DeactivateUserCommand(user.Id, Guid.NewGuid(), DeactivationReason.Inactivity);

            _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

            var handler = CreateHandler();

            await handler.Handle(command, CancellationToken.None);

            user.IsActive.Should().BeFalse();
            user.DeactivationReason.Should().Be(DeactivationReason.Inactivity);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenOperatorTriesToDeactivateSelf()
        {
            var operatorId = Guid.NewGuid();
            var command = new DeactivateUserCommand(operatorId, operatorId, DeactivationReason.Inactivity);

            var handler = CreateHandler();

            var act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<DomainException>();
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenUserIsNotFound()
        {
            var command = new DeactivateUserCommand(Guid.NewGuid(), Guid.NewGuid(), DeactivationReason.Inactivity);

            _userRepositoryMock.Setup(r => r.GetByIdAsync(command.Id)).ReturnsAsync((User?)null);

            var handler = CreateHandler();

            var act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<DomainException>();
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenUserIsAlreadyDeactivated()
        {
            var user = BuildUser();
            user.DeactivateAccount();
            var command = new DeactivateUserCommand(user.Id, Guid.NewGuid(), DeactivationReason.Inactivity);

            _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

            var handler = CreateHandler();

            var act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<DomainException>();
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }
    }
}
