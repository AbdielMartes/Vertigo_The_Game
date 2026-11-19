using Vertigo.Contracts.Internal;
using Vertigo.Server.Game.Matches.Implementations;

namespace Vertigo.Server.Game.Tests.Matches;

public sealed class InMemoryMatchSessionStarterTests
{
    [Fact]
    public void Start_ValidRequest_CreatesMatchingSession()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);
        MatchStartRequest request = CreateRequest();

        MatchSessionSnapshot result = starter.Start(request);

        Assert.NotEqual(Guid.Empty, result.MatchId);
        Assert.Equal(request.LobbyInstanceId, result.LobbyInstanceId);
        Assert.Equal(request.PlayerIds, result.PlayerIds);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(10)]
    public void Start_UnsupportedPlayerCount_RejectsRequest(int playerCount)
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);
        Guid[] players = Enumerable.Range(0, playerCount).Select(_ => Guid.NewGuid()).ToArray();
        MatchStartRequest request = CreateRequest() with { HostPlayerId = players[0], PlayerIds = players };

        ArgumentException exception = Assert.Throws<ArgumentException>(() => starter.Start(request));

        Assert.Equal("request", exception.ParamName);
    }

    [Fact]
    public void Start_RepeatedRequest_ReturnsSameSession()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);
        MatchStartRequest request = CreateRequest();

        MatchSessionSnapshot first = starter.Start(request);
        MatchSessionSnapshot second = starter.Start(request);

        Assert.Same(first, second);
    }

    [Fact]
    public void Start_ConcurrentRequests_CreatesOnlyOneMatch()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);
        MatchStartRequest request = CreateRequest();
        Guid[] identifiers = new Guid[32];

        Parallel.For(0, identifiers.Length, index =>
        {
            identifiers[index] = starter.Start(request).MatchId;
        });

        Assert.Single(identifiers.Distinct());
    }

    [Fact]
    public void Start_DifferentParticipantsForSameLobby_RejectsConflict()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);
        MatchStartRequest request = CreateRequest();
        starter.Start(request);
        MatchStartRequest changed = request with
        {
            PlayerIds = new[] { request.HostPlayerId, Guid.NewGuid() }
        };

        Assert.Throws<InvalidOperationException>(() => starter.Start(changed));
    }

    [Fact]
    public void Start_AnotherLobbyInstanceWithSameCode_CreatesNewMatch()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);
        MatchStartRequest request = CreateRequest();

        Guid first = starter.Start(request).MatchId;
        Guid second = starter.Start(request with { LobbyInstanceId = Guid.NewGuid() }).MatchId;

        Assert.NotEqual(first, second);
    }

    private static MatchStartRequest CreateRequest()
    {
        Guid host = Guid.NewGuid();
        return new MatchStartRequest
        {
            LobbyInstanceId = Guid.NewGuid(),
            LobbyCode = "ABCDEF",
            HostPlayerId = host,
            PlayerIds = new[] { host, Guid.NewGuid() }
        };
    }
}
