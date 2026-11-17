using Vertigo.Client.Core.Navigation;

namespace Vertigo.Client.UnitTests.Navigation;

public sealed class NavigatorTests
{
    [Fact]
    public void NavigateTo_DifferentPage_RaisesOneEvent()
    {
        Navigator navigator = new();
        List<NavigationPage> notifications = new();
        navigator.PageChanged += (_, page) => notifications.Add(page);

        navigator.NavigateTo(NavigationPage.Home);

        Assert.Equal(NavigationPage.Home, navigator.CurrentPage);
        Assert.Equal(new[] { NavigationPage.Home }, notifications);
    }

    [Fact]
    public void NavigateTo_SamePage_DoesNotRepeatEvent()
    {
        Navigator navigator = new();
        int notificationCount = 0;
        navigator.PageChanged += (_, _) => notificationCount++;

        navigator.NavigateTo(NavigationPage.Home);
        navigator.NavigateTo(NavigationPage.Home);

        Assert.Equal(1, notificationCount);
    }
}
