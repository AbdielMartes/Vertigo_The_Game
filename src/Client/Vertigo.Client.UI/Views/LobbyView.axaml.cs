using Avalonia.Controls;
using Avalonia.Threading;

namespace Vertigo.Client.UI.Views;

public partial class LobbyView : UserControl
{
    public LobbyView()
    {
        InitializeComponent();
        lstLobbyChat.Items.CollectionChanged += (_, _) => Dispatcher.UIThread.Post(ScrollToLastMessage,
            DispatcherPriority.Loaded);
    }

    private void ScrollToLastMessage()
    {
        if (lstLobbyChat.ItemCount > 0)
        {
            lstLobbyChat.ScrollIntoView(lstLobbyChat.ItemCount - 1);
        }
    }
}
