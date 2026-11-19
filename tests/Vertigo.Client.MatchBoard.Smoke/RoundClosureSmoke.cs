using System.ServiceModel;
using System.Threading.Channels;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Contracts.Models;

internal static class RoundClosureSmoke
{
    private const int MaximumRoundActions = 100;
    private const int MaximumPlacementDifference = 10;
    private static readonly TimeSpan NotificationTimeout = TimeSpan.FromSeconds(15);

    public static async Task<bool> VerifyAsync(
        (ILobbyClient Lobby, UserSession Session) first,
        (ILobbyClient Lobby, UserSession Session) second,
        string lobbyCode)
    {
        Channel<MatchBoardChanged> firstChanges = Channel.CreateUnbounded<MatchBoardChanged>();
        Channel<MatchBoardChanged> secondChanges = Channel.CreateUnbounded<MatchBoardChanged>();
        first.Lobby.MatchBoardUpdated += ObserveFirst;
        second.Lobby.MatchBoardUpdated += ObserveSecond;

        try
        {
            MatchBoardSnapshot board = first.Lobby.GetMatchBoard(lobbyCode, first.Session.Token);
            Check(board.RoundNumber == 1 && board.RoundSummaries.Count == 0, "Expected the first round in progress.");
            bool dismantled = false;
            for (int step = 0; step < MaximumRoundActions && board.RoundNumber == 1; step++)
            {
                (ILobbyClient Lobby, UserSession Session) active =
                    board.CurrentTurnPlayerId == first.Session.PlayerId ? first : second;
                TurnResult turn = PlayTurn(active, board, lobbyCode);
                dismantled |= turn.Kind == MatchBoardActionKind.Dismantle;

                Check(turn.Board.Revision == board.Revision + 1, "A turn must increase the board revision once.");
                using CancellationTokenSource timeout = new(NotificationTimeout);
                MatchBoardChanged firstChange = await firstChanges.Reader.ReadAsync(timeout.Token);
                MatchBoardChanged secondChange = await secondChanges.Reader.ReadAsync(timeout.Token);
                CheckNotice(firstChange, turn, active.Session.Username);
                CheckNotice(secondChange, turn, active.Session.Username);
                board = turn.Board;
            }

            Check(board.RoundNumber == 2 && board.RoundSummaries.Count == 1, "The first round did not close.");
            VerifySummary(board, first.Lobby.GetMatchBoard(lobbyCode, first.Session.Token));
            VerifySummary(board, second.Lobby.GetMatchBoard(lobbyCode, second.Session.Token));

            second.Lobby.Disconnect();
            second.Lobby.JoinLobby(lobbyCode, second.Session.Token);
            VerifySummary(board, second.Lobby.GetMatchBoard(lobbyCode, second.Session.Token));

            return dismantled;
        }
        finally
        {
            first.Lobby.MatchBoardUpdated -= ObserveFirst;
            second.Lobby.MatchBoardUpdated -= ObserveSecond;
        }

        void ObserveFirst(object? sender, MatchBoardChanged change)
        {
            firstChanges.Writer.TryWrite(change);
        }

        void ObserveSecond(object? sender, MatchBoardChanged change)
        {
            secondChanges.Writer.TryWrite(change);
        }
    }

    private static TurnResult PlayTurn(
        (ILobbyClient Lobby, UserSession Session) active, MatchBoardSnapshot board, string lobbyCode)
    {
        MatchPrivateHand hand = active.Lobby.GetMatchPrivateHand(lobbyCode, active.Session.Token);
        (int Card, int Pile) placement = (from card in hand.Cards
                                         from pile in board.Piles
                                         where pile.TopCardNumber is int top
                                             && card - top is >= 1 and <= MaximumPlacementDifference
                                         select (card, pile.Position)).FirstOrDefault();
        int existingTower = board.Piles.First(pile => pile.TopCardNumber.HasValue).Position;
        MatchDismantleRequest request = new()
        {
            LobbyCode = lobbyCode,
            SessionToken = active.Session.Token,
            PilePosition = existingTower,
            ExpectedRevision = board.Revision
        };

        TurnResult result;
        (int First, int Second, int Pile) chain = FindMasterPlan(board, hand);
        if (chain.Pile != 0)
        {
            MatchBoardSnapshot updated = active.Lobby.UseMatchMasterPlan(new MatchMasterPlanRequest
            {
                LobbyCode = lobbyCode,
                SessionToken = active.Session.Token,
                CardNumbers = new List<int> { chain.First, chain.Second },
                PilePosition = chain.Pile,
                ExpectedRevision = board.Revision
            });
            MatchBoardMove latest = updated.RecentMoves.Last();
            Check(latest.ActionKind == MatchBoardActionKind.MasterPlan && latest.Revision == updated.Revision
                && latest.PlayerId == active.Session.PlayerId && latest.PlayerUsername == active.Session.Username
                && latest.PilePosition == chain.Pile
                && latest.CardNumbers.SequenceEqual(new[] { chain.First, chain.Second }),
                "The master plan did not produce one history entry with the expected chain.");
            Console.WriteLine("PASS: MASTER PLAN CHAIN IN PUBLIC MOVE HISTORY");
            result = new TurnResult(updated, MatchBoardActionKind.MasterPlan, chain.Pile);
        }
        else
        {
            result = placement.Pile == 0 ? PlayDismantle(active.Lobby, request)
                : PlayPlacement(active.Lobby, request, placement);
        }

        return result;
    }

    private static (int First, int Second, int Pile) FindMasterPlan(MatchBoardSnapshot board, MatchPrivateHand hand)
    {
        (int First, int Second, int Pile) chain = default;
        if (hand.MasterPlans > 0)
        {
            chain = (from pile in board.Piles
                     from first in hand.Cards
                     from second in hand.Cards
                     where pile.TopCardNumber is int top && first - top is >= 1 and <= MaximumPlacementDifference
                         && second - first is >= 1 and <= MaximumPlacementDifference
                     select (first, second, pile.Position)).FirstOrDefault();
        }

        return chain;
    }

    private static TurnResult PlayDismantle(ILobbyClient lobby, MatchDismantleRequest request)
    {
        MatchBoardSnapshot board = lobby.DismantleMatchTower(request);
        bool rejected = false;
        try
        {
            lobby.DismantleMatchTower(request);
        }
        catch (FaultException<ServiceFault> ex)
        {
            rejected = ex.Detail.MessageKey == "Match.Error.Stale";
        }

        Check(rejected, "Repeating the dismantle request did not return the stale revision fault.");

        return new TurnResult(board, MatchBoardActionKind.Dismantle, request.PilePosition);
    }

    private static TurnResult PlayPlacement(
        ILobbyClient lobby, MatchDismantleRequest request, (int Card, int Pile) placement)
    {
        bool rejected = false;
        try
        {
            lobby.DismantleMatchTower(request);
        }
        catch (FaultException<ServiceFault> ex)
        {
            rejected = ex.Detail.MessageKey == "Match.Error.PlacementAvailable";
        }

        Check(rejected, "The server accepted dismantling when a placement was available.");
        MatchBoardSnapshot board = lobby.PlaceMatchCard(new MatchPlaceRequest
        {
            LobbyCode = request.LobbyCode,
            SessionToken = request.SessionToken,
            CardNumber = placement.Card,
            PilePosition = placement.Pile,
            ExpectedRevision = request.ExpectedRevision
        });

        return new TurnResult(board, MatchBoardActionKind.Placement, placement.Pile);
    }

    private static void CheckNotice(MatchBoardChanged notice, TurnResult turn, string username)
    {
        Check(notice.MatchId == turn.Board.MatchId && notice.Revision == turn.Board.Revision
            && notice.ActionKind == turn.Kind && notice.PilePosition == turn.Pile && notice.PlayerUsername == username,
            "A client received the wrong action notification.");
    }

    private static void VerifySummary(MatchBoardSnapshot expected, MatchBoardSnapshot received)
    {
        Check(received.RoundNumber == expected.RoundNumber && received.Revision == expected.Revision
            && received.CurrentTurnPlayerId == expected.CurrentTurnPlayerId,
            "The clients disagree on the opening player or board revision.");
        MatchRoundSummary actual = received.RoundSummaries.Single();
        MatchRoundSummary target = expected.RoundSummaries.Single();
        Check(actual.RoundNumber == 1 && actual.ClosedByPlayerId == target.ClosedByPlayerId
            && actual.Players.Count == 2 && actual.Players.OrderBy(score => score.PlayerId).Select(Score)
                .SequenceEqual(target.Players.OrderBy(score => score.PlayerId).Select(Score)),
            "The clients received different round summaries.");
        Check(actual.Players.All(score =>
            score.TotalFloors == score.FloorsBuilt + score.BonusFloors
            && received.Players.Single(player => player.PlayerId == score.PlayerId).GlobalFloors == score.TotalFloors),
            "The round summary does not add up to the board totals.");
    }

    private static (Guid PlayerId, int FloorsBuilt, int BonusFloors, int TotalFloors) Score(MatchRoundPlayerScore score)
    {
        return (score.PlayerId, score.FloorsBuilt, score.BonusFloors, score.TotalFloors);
    }

    private static void Check(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private sealed record TurnResult(MatchBoardSnapshot Board, MatchBoardActionKind Kind, int Pile);
}
