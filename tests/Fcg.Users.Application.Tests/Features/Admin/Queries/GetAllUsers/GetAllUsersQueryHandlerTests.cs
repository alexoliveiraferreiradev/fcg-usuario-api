using Fcg.Users.Application.Common.Interfaces;
using Fcg.Users.Application.Features.Admin.Queries.GetAllUsers;
using Fcg.Users.Application.Features.Users.Responses;
using Fcg.Users.Domain.Enum;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fcg.Users.Application.Tests.Features.Admin.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandlerTests
    {
        private readonly Mock<ILogger<GetAllUsersQueryHandler>> _loggerMock = new();
        private readonly Mock<IAdminQueryRepository> _adminQueryRepositoryMock = new();

        private GetAllUsersQueryHandler CreateHandler() => new(
            _loggerMock.Object,
            _adminQueryRepositoryMock.Object);

        [Fact]
        public async Task Handle_ShouldReturnUsers_WhenRepositoryHasUsers()
        {
            var users = new List<UserResponse>
            {
                new() { Id = Guid.NewGuid(), Name = "User A", Email = "a@teste.com", PerfilUser = UserRole.Player },
                new() { Id = Guid.NewGuid(), Name = "User B", Email = "b@teste.com", PerfilUser = UserRole.Admin }
            };

            _adminQueryRepositoryMock
                .Setup(r => r.GetAllUsersAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(users);

            var handler = CreateHandler();

            var result = await handler.Handle(new GetAllUsersQuery(), CancellationToken.None);

            result.Should().BeEquivalentTo(users);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenRepositoryHasNoUsers()
        {
            _adminQueryRepositoryMock
                .Setup(r => r.GetAllUsersAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Empty<UserResponse>());

            var handler = CreateHandler();

            var result = await handler.Handle(new GetAllUsersQuery(), CancellationToken.None);

            result.Should().BeEmpty();
        }
    }
}
