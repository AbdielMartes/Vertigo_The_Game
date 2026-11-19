using System.ServiceModel;
using Vertigo.Client.Networking.Callbacks;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;

namespace Vertigo.Client.Networking.Proxies;

public sealed class LobbyClient : ILobbyClient
{
    private const int OperationTimeoutSeconds = 15;

    private static readonly TimeSpan OperationTimeout = TimeSpan.FromSeconds(OperationTimeoutSeconds);

    private LobbyCallbackHandler? _callbackHandler;
    private DuplexChannelFactory<ILobbyService>? _channelFactory;
    private readonly object _syncRoot = new();
    private readonly object _eventLock = new();
    private readonly Queue<PendingLobbyEvent> _pendingEvents = new();
    private EventHandler<LobbyMember>? _playerJoined;
    private EventHandler<Guid>? _playerLeft;
    private EventHandler<LobbyChatMessage>? _messageReceived;
    private EventHandler<LobbyPreparation>? _preparationChanged;
    private EventHandler<MatchBoardChanged>? _matchBoardUpdated;
    private ILobbyService? _channel;
    private bool _isDisposed;

    public LobbyClient()
    {
    }

    public event EventHandler<LobbyMember>? PlayerJoined
    {
        add
        {
            lock (_eventLock)
            {
                _playerJoined += value;
            }
        }
        remove
        {
            lock (_eventLock)
            {
                _playerJoined -= value;
            }
        }
    }

    public event EventHandler<Guid>? PlayerLeft
    {
        add
        {
            lock (_eventLock)
            {
                _playerLeft += value;
            }
        }
        remove
        {
            lock (_eventLock)
            {
                _playerLeft -= value;
            }
        }
    }
    
    public event EventHandler<LobbyChatMessage>? MessageReceived
    {
        add
        {
            lock (_eventLock)
            {
                _messageReceived += value;
            }
        }
        remove
        {
            lock (_eventLock)
            {
                _messageReceived -= value;
            }
        }
    }

    public event EventHandler<LobbyPreparation>? PreparationChanged
    {
        add
        {
            lock (_eventLock)
            {
                _preparationChanged += value;
            }
        }
        remove
        {
            lock (_eventLock)
            {
                _preparationChanged -= value;
            }
        }
    }

    public event EventHandler<MatchBoardChanged>? MatchBoardUpdated
    {
        add
        {
            lock (_eventLock)
            {
                _matchBoardUpdated += value;
            }
        }
        remove
        {
            lock (_eventLock)
            {
                _matchBoardUpdated -= value;
            }
        }
    }

    public string CreateLobby(bool isPublic, int maxPlayers, string sessionToken)
    {
        ClearPendingEvents();
        ILobbyService channel = GetOrCreateChannel();
        try
        {
            return channel.CreateLobby(isPublic, maxPlayers, sessionToken);
        }
        catch (Exception exception) when (IsTransportFailure(exception))
        {
            AbortChannel(channel);
            throw;
        }
    }

    public LobbyAdmission JoinLobby(string lobbyCode, string sessionToken)
    {
        ClearPendingEvents();
        ILobbyService channel = GetOrCreateChannel();
        try
        {
            return channel.JoinLobby(lobbyCode, sessionToken);
        }
        catch (Exception exception) when (IsTransportFailure(exception))
        {
            AbortChannel(channel);
            throw;
        }
    }

    public bool LeaveLobby(string lobbyCode, string sessionToken)
    {
        ILobbyService channel = GetOrCreateChannel();
        try
        {
            return channel.LeaveLobby(lobbyCode, sessionToken);
        }
        catch (Exception exception) when (IsTransportFailure(exception))
        {
            AbortChannel(channel);
            throw;
        }
    }

    public List<LobbySummary> GetPublicLobbies()
    {
        ILobbyService channel = GetOrCreateChannel();
        List<LobbySummary> lobbies;

        try
        {
            lobbies = channel.GetPublicLobbies();
        }
        catch (Exception exception) when (IsTransportFailure(exception))
        {
            AbortChannel(channel);
            throw;
        }

        return lobbies;
    }

    public LobbyPreparation GetPreparation(string lobbyCode, string sessionToken)
    {
        ILobbyService channel = GetOrCreateChannel();
        try
        {
            return channel.GetPreparation(lobbyCode, sessionToken);
        }
        catch (Exception exception) when (IsTransportFailure(exception))
        {
            AbortChannel(channel);
            throw;
        }
    }

    public MatchBoardSnapshot GetMatchBoard(string lobbyCode, string sessionToken)
    {
        return ReadOnIndependentChannel(channel => channel.GetMatchBoard(lobbyCode, sessionToken));
    }

    public MatchPrivateHand GetMatchPrivateHand(string lobbyCode, string sessionToken)
    {
        return ReadOnIndependentChannel(channel => channel.GetMatchPrivateHand(lobbyCode, sessionToken));
    }

    public MatchBoardSnapshot PlaceMatchCard(MatchPlaceRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return ReadOnIndependentChannel(channel => channel.PlaceMatchCard(request));
    }

    public MatchBoardSnapshot DismantleMatchTower(MatchDismantleRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return ReadOnIndependentChannel(channel => channel.DismantleMatchTower(request));
    }

    public MatchBoardSnapshot UseMatchMasterPlan(MatchMasterPlanRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return ReadOnIndependentChannel(channel => channel.UseMatchMasterPlan(request));
    }

    public LobbyPreparation? GetCurrentLobby(string sessionToken)
    {
        return ReadOnIndependentChannel(channel => channel.GetCurrentLobby(sessionToken));
    }

    // The normal duplex channel is occupied by callbacks. Reusing it from a
    // callback-driven navigation can trigger WCF's Single-concurrency deadlock guard.
    private static TResult ReadOnIndependentChannel<TResult>(Func<ILobbyService, TResult> read)
    {
        DuplexChannelFactory<ILobbyService>? factory = null;
        ICommunicationObject? communication = null;
        try
        {
            NetTcpBinding binding = new(SecurityMode.None)
            {
                OpenTimeout = OperationTimeout,
                SendTimeout = OperationTimeout
            };
            factory = new DuplexChannelFactory<ILobbyService>(
                new InstanceContext(new LobbyCallbackHandler()), binding,
                new EndpointAddress(ServiceEndpoints.LobbyServiceAddress));
            ILobbyService channel = factory.CreateChannel();
            communication = (ICommunicationObject)channel;
            return read(channel);
        }
        finally
        {
            if (communication is not null)
            {
                CloseSafely(communication);
            }
            if (factory is not null)
            {
                CloseSafely(factory);
            }
        }
    }

    public LobbyPreparation SetReady(string lobbyCode, bool isReady, string sessionToken)
    {
        ILobbyService channel = GetOrCreateChannel();
        try
        {
            return channel.SetReady(lobbyCode, isReady, sessionToken);
        }
        catch (Exception exception) when (IsTransportFailure(exception))
        {
            AbortChannel(channel);
            throw;
        }
    }

    public LobbyPreparation StartMatch(string lobbyCode, string sessionToken)
    {
        ILobbyService channel = GetOrCreateChannel();
        try
        {
            return channel.StartMatch(lobbyCode, sessionToken);
        }
        catch (Exception exception) when (IsTransportFailure(exception))
        {
            AbortChannel(channel);
            throw;
        }
    }

    public void ReplayPendingEvents()
    {
        lock (_eventLock)
        {
            while (_pendingEvents.Count > 0)
            {
                PendingLobbyEvent pending = _pendingEvents.Dequeue();

                if (pending.Joined is not null)
                {
                    _playerJoined?.Invoke(this, pending.Joined);
                }
                else if (pending.Preparation is not null)
                {
                    _preparationChanged?.Invoke(this, pending.Preparation);
                }
                else if (pending.Chat is not null)
                {
                    _messageReceived?.Invoke(this, pending.Chat);
                }
                else
                {
                    _playerLeft?.Invoke(this, pending.LeftPlayerId);
                }
            }
        }
    }

    public void Disconnect()
    {
        ILobbyService? oldChannel;
        DuplexChannelFactory<ILobbyService>? oldFactory;
        lock (_syncRoot)
        {
            oldChannel = _channel;
            oldFactory = _channelFactory;
            _channel = null;
            _channelFactory = null;
            _callbackHandler = null;
        }
        (oldChannel as ICommunicationObject)?.Abort();
        oldFactory?.Abort();
        ClearPendingEvents();
    }

    public void Dispose()
    {
        lock (_syncRoot)
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;
        }
        Disconnect();
    }

    private static bool IsTransportFailure(Exception exception)
    {
        bool isCommunicationFailure = exception is CommunicationException and not FaultException;
        bool isTimeout = exception is TimeoutException;

        return isCommunicationFailure || isTimeout;
    }

    private ILobbyService GetOrCreateChannel()
    {
        lock (_syncRoot)
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);
            if (_channel is not null && !IsUnusable((ICommunicationObject)_channel))
            {
                return _channel;
            }
            _channelFactory?.Abort();
            LobbyCallbackHandler handler = new();
            _callbackHandler = handler;
            handler.PlayerJoined += (sender, player) =>
            {
                if (ReferenceEquals(_callbackHandler, handler))
                {
                    OnPlayerJoined(sender, player);
                }
            };
            handler.PlayerLeft += (sender, playerId) =>
            {
                if (ReferenceEquals(_callbackHandler, handler))
                {
                    OnPlayerLeft(sender, playerId);
                }
            };
            handler.PreparationChanged += (sender, preparation) =>
            {
                if (ReferenceEquals(_callbackHandler, handler))
                {
                    OnPreparationChanged(sender, preparation);
                }
            };
            handler.MessageReceived += (sender, message) =>
            {
                if (ReferenceEquals(_callbackHandler, handler))
                {
                    OnChatReceived(sender, message);
                }
            };
            handler.MatchBoardUpdated += (sender, change) =>
            {
                if (ReferenceEquals(_callbackHandler, handler))
                {
                    OnMatchBoardUpdated(change);
                }
            };
            NetTcpBinding binding = new(SecurityMode.None)
            {
                OpenTimeout = OperationTimeout,
                SendTimeout = OperationTimeout
            };
            _channelFactory = new DuplexChannelFactory<ILobbyService>(
                new InstanceContext(handler), binding,
                new EndpointAddress(ServiceEndpoints.LobbyServiceAddress));
            _channel = _channelFactory.CreateChannel();
            return _channel;
        }
    }

    private void AbortChannel(ILobbyService channel)
    {
        ((ICommunicationObject)channel).Abort();

        lock (_syncRoot)
        {
            if (ReferenceEquals(_channel, channel))
            {
                _channel = null;
            }
        }
    }

    private static bool IsUnusable(ICommunicationObject communicationObject)
    {
        bool isUnusable = communicationObject.State is CommunicationState.Faulted
            or CommunicationState.Closing
            or CommunicationState.Closed;

        return isUnusable;
    }

    private static void CloseSafely(ICommunicationObject communicationObject)
    {
        try
        {
            communicationObject.Close();
        }
        catch (CommunicationException)
        {
            communicationObject.Abort();
        }
        catch (TimeoutException)
        {
            communicationObject.Abort();
        }
    }

    private void ClearPendingEvents()
    {
        lock (_eventLock)
        {
            _pendingEvents.Clear();
        }
    }

    // Events are raised inside the lock so replayed and live events keep the server's order;
    // subscribers must only marshal to the UI thread (Dispatcher.Post) and return.
    private void OnPlayerJoined(object? sender, LobbyMember player)
    {
        lock (_eventLock)
        {
            if (_playerJoined is null)
            {
                _pendingEvents.Enqueue(new PendingLobbyEvent(player, Guid.Empty, null));
            }
            else
            {
                _playerJoined.Invoke(this, player);
            }
        }
    }

    private void OnPlayerLeft(object? sender, Guid playerId)
    {
        lock (_eventLock)
        {
            if (_playerLeft is null)
            {
                _pendingEvents.Enqueue(new PendingLobbyEvent(null, playerId, null));
            }
            else
            {
                _playerLeft.Invoke(this, playerId);
            }
        }
    }
    private void OnPreparationChanged(object? sender, LobbyPreparation preparation)
    {
        lock (_eventLock)
        {
            if (_preparationChanged is null)
            {
                _pendingEvents.Enqueue(new PendingLobbyEvent(null, Guid.Empty, null, preparation));
            }
            else
            {
                _preparationChanged.Invoke(this, preparation);
            }
        }
    }

    private void OnMatchBoardUpdated(MatchBoardChanged change)
    {
        lock (_eventLock)
        {
            _matchBoardUpdated?.Invoke(this, change);
        }
    }

    private void OnChatReceived(object? sender, LobbyChatMessage message)
    {
        lock (_eventLock)
        {
            if (_messageReceived is null)
            {
                _pendingEvents.Enqueue(new PendingLobbyEvent(null, Guid.Empty, message));
            }
            else
            {
                _messageReceived.Invoke(this, message);
            }
        }
    }
}
