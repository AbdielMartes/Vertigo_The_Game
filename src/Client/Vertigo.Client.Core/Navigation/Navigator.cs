namespace Vertigo.Client.Core.Navigation;

public sealed class Navigator : INavigator
{
    private readonly object _syncRoot = new();
    private NavigationPage _currentPage = NavigationPage.Access;

    public event EventHandler<NavigationPage>? PageChanged;

    public NavigationPage CurrentPage
    {
        get
        {
            lock (_syncRoot)
            {
                return _currentPage;
            }
        }
    }

    public void NavigateTo(NavigationPage page)
    {
        bool changed;
        lock (_syncRoot)
        {
            changed = _currentPage != page;
            _currentPage = page;
        }

        if (changed)
        {
            PageChanged?.Invoke(this, page);
        }
    }
}
