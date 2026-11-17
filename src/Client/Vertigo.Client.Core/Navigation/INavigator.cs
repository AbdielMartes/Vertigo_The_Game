namespace Vertigo.Client.Core.Navigation;

public interface INavigator
{
    NavigationPage CurrentPage { get; }
    event EventHandler<NavigationPage>? PageChanged;
    void NavigateTo(NavigationPage page);
}
