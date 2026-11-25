using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Sessions;

public sealed class SessionCoordinator : ISessionCoordinator
{
    private readonly IAuthClient _authClient;
    private readonly PlayerSession _playerSession;

    public SessionCoordinator(IAuthClient authClient, PlayerSession playerSession)
    {
        _authClient = authClient;
        _playerSession = playerSession;
    }

    public ClientResult<UserSession> Login(string username, string password)
    {
        ClientResult<UserSession> result = _authClient.Login(new LoginCredentials
        {
            Username = username,
            Password = password
        });
        ApplySession(result);
        return result;
    }

    public ClientResult<UserSession> ResumeSession(string token)
    {
        ClientResult<UserSession> result = _authClient.ResumeSession(token);
        ApplySession(result);
        return result;
    }

    public ClientResult<UserSession> CreateGuestSession()
    {
        ClientResult<UserSession> result = _authClient.CreateGuestSession();
        ApplySession(result);
        return result;
    }

    public ClientResult<bool> Logout()
    {
        SessionSnapshot? session = _playerSession.GetActiveSession();
        ClientResult<bool> result = ClientResult<bool>.Success(true);
        if (session is not null)
        {
            result = _authClient.Logout(session.Token);
            if (result.IsSuccess && result.Value)
            {
                _playerSession.Clear(session);
            }
        }

        return result;
    }

    private void ApplySession(ClientResult<UserSession> result)
    {
        if (result.IsSuccess && result.Value is UserSession session)
        {
            _playerSession.SetSession(new SessionSnapshot
            {
                PlayerId = session.PlayerId,
                Username = session.Username,
                AvatarId = session.AvatarId,
                Token = session.Token,
                ExpiresAt = session.ExpiresAt,
                IsGuest = session.IsGuest
            });
        }
    }
}
