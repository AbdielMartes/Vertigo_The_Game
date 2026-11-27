using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Vertigo.Client.UI.ViewModels.Presentation;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UI.ViewModels;

public sealed partial class MatchViewModel
{
    private readonly HashSet<long> _hiddenBallots = new();

    public ObservableCollection<MatchExpulsionPrompt> ExpulsionPrompts { get; } = new();
    public IRelayCommand ShowExpulsionPromptsCommand { get; }
    public bool HasExpulsionPrompts => ExpulsionPrompts.Count > 0;
    public bool HasHiddenExpulsionPrompts => _hiddenBallots.Count > 0;
    public string ShowExpulsionPromptsText => _text.Get("Match.Expulsion.Show");
    public string ExpelledByVoteText => _text.Get("Match.Expulsion.Removed");
    public bool WasExpelledByVote => _session?.GetActiveSession()?.PlayerId is Guid playerId
        && _voting.Board?.ExpelledByVotePlayerIds.Contains(playerId) == true;

    private bool CanSubmitExpulsion()
    {
        return _hasCurrentHand && !NeedsReconnect && !MatchUnavailable && !IsLoadingBoard
            && !_isSubmittingTurn && !_isLeavingMatch && !IsExitConfirmationOpen
            && _turnClock.GetRemainingSeconds() is > 0;
    }

    private bool CanRequestExpulsion(Guid targetId, long ballotId)
    {
        MatchExpulsionVoteSummary? ballot = _voting.FindBallot(targetId);
        return _voting.IsEligibleTarget(targetId)
            && (ballot is null or { IsRejected: true }) && (ballot?.BallotId ?? 0) == ballotId;
    }

    private bool CanRespondToExpulsion(Guid targetId, long ballotId)
    {
        return ballotId > 0 && CanSubmitExpulsion() && _voting.CanVoteFor(targetId)
            && _voting.FindBallot(targetId) is { IsRejected: false } ballot && ballot.BallotId == ballotId;
    }

    private bool CanSubmitExpulsionDecision(Guid targetId, MatchExpulsionDecision decision, long ballotId)
    {
        return decision switch
        {
            MatchExpulsionDecision.Request => CanSubmitExpulsion() && CanRequestExpulsion(targetId, ballotId),
            MatchExpulsionDecision.Support => CanSubmitExpulsion() && _voting.CanVoteFor(targetId)
                && _voting.FindBallot(targetId) is { BallotId: 0, IsRejected: false },
            MatchExpulsionDecision.Accept or MatchExpulsionDecision.Reject => CanRespondToExpulsion(targetId, ballotId),
            _ => false
        };
    }

    private Task SubmitExpulsionAsync(Guid targetId, MatchExpulsionDecision decision, long ballotId)
    {
        bool canSubmit = CanSubmitExpulsionDecision(targetId, decision, ballotId);
        string lobbyCode = _lobbyCode;
        Guid matchId = _matchId;
        long votingRevision = _voting.Revision;

        return canSubmit
            ? SubmitTurnAsync((lobbies, token) => lobbies.VoteToExpelMatchPlayer(new MatchExpulsionVoteRequest
            {
                LobbyCode = lobbyCode,
                SessionToken = token,
                MatchId = matchId,
                TargetPlayerId = targetId,
                ExpectedVotingRevision = votingRevision,
                Decision = decision,
                BallotId = ballotId
            }))
            : Task.CompletedTask;
    }

    private void RefreshExpulsionPrompts()
    {
        MatchBoardSnapshot? board = _voting.Board;
        Guid? localId = _session?.GetActiveSession()?.PlayerId;
        List<MatchExpulsionPrompt> prompts = new();
        if (board is { EndReason: MatchEndReason.None } && board.Players.Exists(player => player.PlayerId == localId))
        {
            _hiddenBallots.RemoveWhere(id => !board.ExpulsionVotes.Exists(ballot => ballot.BallotId == id));
            foreach (MatchExpulsionVoteSummary ballot in board.ExpulsionVotes.OrderBy(vote => vote.BallotId))
            {
                MatchBoardPlayer? target = board.Players.Find(player => player.PlayerId == ballot.TargetPlayerId);
                if (ballot.BallotId > 0 && target is not null && !_hiddenBallots.Contains(ballot.BallotId))
                {
                    prompts.Add(CreateExpulsionPrompt(ballot, target, target.PlayerId == localId));
                }
            }
        }
        else
        {
            _hiddenBallots.Clear();
        }

        if (!ExpulsionPrompts.SequenceEqual(prompts))
        {
            ExpulsionPrompts.Clear();
            foreach (MatchExpulsionPrompt prompt in prompts)
            {
                ExpulsionPrompts.Add(prompt);
            }
        }

        OnPropertyChanged(nameof(HasExpulsionPrompts));
        OnPropertyChanged(nameof(HasHiddenExpulsionPrompts));
    }

    private MatchExpulsionPrompt CreateExpulsionPrompt(
        MatchExpulsionVoteSummary ballot, MatchBoardPlayer target, bool isTarget)
    {
        long id = ballot.BallotId;
        Guid targetId = target.PlayerId;
        bool voted = _voting.HasVoted(targetId);
        string heading = isTarget
            ? _text.Get("Match.Expulsion.TargetNotice")
            : string.Format(_text.Get("Match.Expulsion.Notice"), target.Username);
        string counts = string.Format(_text.Get("Match.Expulsion.Count"),
            ballot.VoteCount, ballot.RejectedCount, _voting.RequiredVotes);
        string status = ballot.IsRejected ? _text.Get("Match.Expulsion.Rejected")
            : voted ? _text.Get("Match.Expulsion.Recorded") : string.Empty;

        MatchExpulsionPrompt? previous = ExpulsionPrompts.FirstOrDefault(prompt => prompt.BallotId == id);
        return new MatchExpulsionPrompt(id, heading, counts, status, !isTarget && !voted && !ballot.IsRejected,
            voted || ballot.IsRejected, _text.Get("Match.Expulsion.Accept"), _text.Get("Match.Expulsion.Reject"),
            _text.Get("Match.Expulsion.Hide"),
            previous?.AcceptCommand ?? new AsyncRelayCommand(
                () => SubmitExpulsionAsync(targetId, MatchExpulsionDecision.Accept, id),
                () => CanRespondToExpulsion(targetId, id)),
            previous?.RejectCommand ?? new AsyncRelayCommand(
                () => SubmitExpulsionAsync(targetId, MatchExpulsionDecision.Reject, id),
                () => CanRespondToExpulsion(targetId, id)),
            previous?.HideCommand ?? new RelayCommand(() => HideExpulsionPrompt(id)));
    }

    private void HideExpulsionPrompt(long ballotId)
    {
        _hiddenBallots.Add(ballotId);
        RefreshExpulsionPrompts();
    }

    private void ShowHiddenExpulsionPrompts()
    {
        _hiddenBallots.Clear();
        RefreshExpulsionPrompts();
    }
}
