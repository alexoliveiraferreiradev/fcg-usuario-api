using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.SharedContracts.Interfaces;
using Fcg.Users.Application.Features.Admin.Commands.ReactivateAccount;
using Fcg.Users.Domain.Entitites;
using Fcg.Users.Domain.Repositories.Interfaces;
using Fcg.Users.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fcg.Users.Application.Tests.Features.Admin.Commands.ReactivateAccount
{
    public class ReactivateAccountCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ILogger<ReactivateAccountCommandHandler>> _loggerMock = new();
        private readonly Mock<IIntegrationEventPublisher> _integrationEventPublisherMock = new();
        private ReactivateAccountCommandHandler CreateHandler() => new(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object,
            _integrationEventPublisherMock.Object);

        private static User BuildUser() =>
            new(new Name("User Teste"), new Email("teste@teste.com"), new Password("SenhaForte@123"));

        [Fact]
        public async Task Handle_ShouldReactivateUser_WhenUserIsInactive()
        {
            var user = BuildUser();
            user.DeactivateAccount();
            var command = new ReactivateAccountCommand(user.Id);

            _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

            var handler = CreateHandler();

            await handler.Handle(command, CancellationToken.None);

            user.IsActive.Should().BeTrue();
            user.DeactivationReason.Should().BeNull();
            _userRepositoryMock.Verify(r => r.Update(user), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenUserIsNotFound()
        {
            var command = new ReactivateAccountCommand(Guid.NewGuid());

            _userRepositoryMock.Setup(r => r.GetByIdAsync(command.UserId)).ReturnsAsync((User?)null);

            var handler = CreateHandler();

            var act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<DomainException>();
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenUserIsAlreadyActive()
        {
            var user = BuildUser();
            var command = new ReactivateAccountCommand(user.Id);

            _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

            var handler = CreateHandler();

            var act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<DomainException>();
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }
    }
}
