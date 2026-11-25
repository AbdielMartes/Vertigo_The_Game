using System;
using Vertigo.Client.Core.Navigation;

namespace Vertigo.Client.UI.ViewModels.Utilities;

public sealed class ProfileNavigation(INavigator navigation)
{
    private (Guid PlayerId, NavigationPage Source)? _pending;

    public void Open(Guid? playerId)
    {
        NavigationPage source = navigation.CurrentPage;
        if (playerId is Guid id && id != Guid.Empty && source is NavigationPage.Social or NavigationPage.Ranking)
        {
            _pending = (id, source);
            navigation.NavigateTo(NavigationPage.Profile);
        }
    }

    public (Guid PlayerId, NavigationPage Source)? Take()
    {
        (Guid PlayerId, NavigationPage Source)? target = _pending;
        _pending = null;
        return target;
    }
}
