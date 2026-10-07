using Fcg.Users.Application.Common.Interfaces;
using Fcg.Users.Application.Features.Admin.Queries.GetUserById;
using Fcg.Users.Application.Features.Users.Responses;
using Fcg.Users.Domain.Enum;
using FluentAssertions;
using Moq;

namespace Fcg.Users.Application.Tests.Features.Admin.Queries.GetUserById
{
    public class GetUserByIdQueryHandlerTests
    {
        private readonly Mock<IAdminQueryRepository> _adminQueryRepositoryMock = new();

        private GetUserByIdQueryHandler CreateHandler() => new(_adminQueryRepositoryMock.Object);

        [Fact]
        public async Task Handle_ShouldReturnUser_WhenUserExists()
        {
            var userId = Guid.NewGuid();
            var user = new UserResponse { Id = userId, Name = "User Teste", Email = "teste@teste.com", PerfilUser = UserRole.Player };

            _adminQueryRepositoryMock
                .Setup(r => r.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var handler = CreateHandler();

            var result = await handler.Handle(new GetUserByIdQuery(userId), CancellationToken.None);

            result.Should().BeEquivalentTo(user);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();

            _adminQueryRepositoryMock
                .Setup(r => r.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserResponse?)null);

            var handler = CreateHandler();

            var result = await handler.Handle(new GetUserByIdQuery(userId), CancellationToken.None);

            result.Should().BeNull();
        }
    }
}
