using Fcg.Users.Application.Common.Interfaces;
using Fcg.Users.Application.Features.Users.Queries.GetMyAccount;
using Fcg.Users.Application.Features.Users.Responses;
using FluentAssertions;
using Moq;

namespace Fcg.Users.Application.Tests.Features.Users.Queries.GetMyAccount
{
    public class GetMyAccountQueryHandlerTests
    {
        private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock = new();

        private GetMyAccountQueryHandler CreateHandler() => new(_userQueryRepositoryMock.Object);

        [Fact]
        public async Task Handle_ShouldReturnAccountOfTokenUser_WhenUserExists()
        {
            var userId = Guid.NewGuid();
            var account = new UserAccountResponse { Name = "Ana Teste", Email = "ana@teste.com" };
            using var cts = new CancellationTokenSource();

            _userQueryRepositoryMock
                .Setup(r => r.GetUserByIdAsync(userId, cts.Token))
                .ReturnsAsync(account);

            var result = await CreateHandler().Handle(new GetMyAccountQuery(userId), cts.Token);

            result.Should().BeEquivalentTo(account);
            _userQueryRepositoryMock.Verify(r => r.GetUserByIdAsync(userId, cts.Token), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenUserDoesNotExist()
        {
            _userQueryRepositoryMock
                .Setup(r => r.GetUserByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserAccountResponse?)null);

            var result = await CreateHandler().Handle(new GetMyAccountQuery(Guid.NewGuid()), CancellationToken.None);

            result.Should().BeNull();
        }
    }
}
