using System.Security.Cryptography;
using System.Text.Json;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Synchronization;

public static class MatchSynchronization
{
    private const int FingerprintBytes = 16;

    public static MatchSyncCursor Describe(MatchSyncState state)
    {
        return new MatchSyncCursor
        {
            Revision = state.Board.Revision,
            Piles = Fingerprint(state.Board.Piles),
            Players = Fingerprint(state.Board.Players),
            RecentMoves = Fingerprint(state.Board.RecentMoves),
            Rounds = Fingerprint(state.Board.RoundSummaries),
            Status = Fingerprint(CollectStatus(state.Board)),
            Hand = FingerprintHand(state.Hand)
        };
    }

    public static MatchSyncUpdate Compare(MatchSyncState state, Guid playerId, MatchSyncCursor? known)
    {
        MatchBoardSnapshot board = state.Board;
        if (known is not { Revision: > 0 } || known.Revision > board.Revision)
        {
            known = null;
        }

        MatchSyncStatus status = CollectStatus(board);
        bool handChanged = known is null || known.Hand != FingerprintHand(state.Hand);
        return new MatchSyncUpdate
        {
            MatchId = board.MatchId,
            PlayerId = playerId,
            BaseRevision = known?.Revision,
            Revision = board.Revision,
            RoundNumber = board.RoundNumber,
            CurrentTurnPlayerId = board.CurrentTurnPlayerId,
            TurnDeadlineUtc = board.TurnDeadlineUtc,
            ServerTimeUtc = board.ServerTimeUtc,
            EndReason = board.EndReason,
            VotingRevision = board.VotingRevision,
            ExpulsionVotesRequired = board.ExpulsionVotesRequired,
            Piles = Changed(board.Piles, known?.Piles),
            Players = Changed(board.Players, known?.Players),
            RecentMoves = Changed(board.RecentMoves, known?.RecentMoves),
            Rounds = Changed(board.RoundSummaries, known?.Rounds),
            Status = Changed(status, known?.Status),
            HandChanged = handChanged,
            Hand = handChanged ? state.Hand : null
        };
    }

    public static MatchSyncState Apply(MatchSyncUpdate update, MatchSyncState? baseline, Guid playerId)
    {
        if (update.MatchId == Guid.Empty || update.PlayerId != playerId || update.Revision <= 0)
        {
            throw new InvalidDataException("Invalid match update identity.");
        }

        MatchBoardSnapshot? old = null;
        if (update.BaseRevision is long revision)
        {
            if (baseline?.Board.MatchId != update.MatchId || baseline.Board.Revision != revision
                || update.Revision < revision)
            {
                throw new InvalidDataException("The match update has no matching base revision.");
            }

            old = baseline.Board;
        }

        MatchSyncStatus status = update.Status ?? (old is null ? throw MissingSection() : CollectStatus(old));
        MatchBoardSnapshot board = AssembleBoard(update, old, status);
        MatchPrivateHand? hand = ReadHand(update, old is null ? null : baseline?.Hand, status);
        return new MatchSyncState(board, hand);
    }

    private static MatchBoardSnapshot AssembleBoard(MatchSyncUpdate update, MatchBoardSnapshot? old, MatchSyncStatus status)
    {
        return new MatchBoardSnapshot
        {
            MatchId = update.MatchId,
            Revision = update.Revision,
            RoundNumber = update.RoundNumber,
            CurrentTurnPlayerId = update.CurrentTurnPlayerId,
            TurnDeadlineUtc = update.TurnDeadlineUtc,
            ServerTimeUtc = update.ServerTimeUtc,
            EndReason = update.EndReason,
            VotingRevision = update.VotingRevision,
            ExpulsionVotesRequired = update.ExpulsionVotesRequired,
            Piles = update.Piles ?? old?.Piles ?? throw MissingSection(),
            Players = update.Players ?? old?.Players ?? throw MissingSection(),
            RecentMoves = update.RecentMoves ?? old?.RecentMoves ?? throw MissingSection(),
            RoundSummaries = update.Rounds ?? old?.RoundSummaries ?? throw MissingSection(),
            WinnerIds = status.Winners,
            ExpelledPlayerIds = status.Expelled,
            InactivePlayerIds = status.Inactive,
            ExpelledByVotePlayerIds = status.ExpelledByVote ?? new List<Guid>(),
            PresenceDeadlinesUtc = status.PresenceDeadlinesUtc,
            ExpulsionVotes = status.ExpulsionVotes
        };
    }

    private static MatchPrivateHand? ReadHand(MatchSyncUpdate update, MatchPrivateHand? previous, MatchSyncStatus status)
    {
        MatchPrivateHand? hand = null;
        if (!status.Expelled.Contains(update.PlayerId))
        {
            MatchPrivateHand? source = update.HandChanged ? update.Hand : previous;
            if (source is null || source.MatchId != update.MatchId || source.PlayerId != update.PlayerId
                || (update.HandChanged && source.Revision != update.Revision))
            {
                throw new InvalidDataException("The private hand does not match the board update.");
            }

            hand = new MatchPrivateHand
            {
                MatchId = source.MatchId,
                PlayerId = source.PlayerId,
                Cards = source.Cards,
                MasterPlans = source.MasterPlans,
                VotingRevision = source.VotingRevision,
                VotedExpulsionTargetIds = source.VotedExpulsionTargetIds,
                Revision = update.Revision
            };
        }

        return hand;
    }

    private static MatchSyncStatus CollectStatus(MatchBoardSnapshot board)
    {
        return new MatchSyncStatus
        {
            Expelled = board.ExpelledPlayerIds,
            Inactive = board.InactivePlayerIds,
            ExpelledByVote = board.ExpelledByVotePlayerIds.Count > 0 ? board.ExpelledByVotePlayerIds : null,
            Winners = board.WinnerIds,
            PresenceDeadlinesUtc = board.PresenceDeadlinesUtc,
            ExpulsionVotes = board.ExpulsionVotes
        };
    }

    private static T? Changed<T>(T value, string? known) where T : class
    {
        return known is null || known != Fingerprint(value) ? value : null;
    }

    private static string FingerprintHand(MatchPrivateHand? hand)
    {
        return hand is null ? string.Empty : Fingerprint(new
        {
            hand.PlayerId,
            hand.Cards,
            hand.MasterPlans,
            hand.VotingRevision,
            hand.VotedExpulsionTargetIds
        });
    }

    private static string Fingerprint<T>(T value)
    {
        byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(value);
        return Convert.ToBase64String(SHA256.HashData(bytes).AsSpan(0, FingerprintBytes));
    }

    private static InvalidDataException MissingSection()
    {
        return new InvalidDataException("The full match update is incomplete.");
    }
}
