using Avalonia.Controls;
using Avalonia.Threading;

namespace Vertigo.Client.UI.Views;

public partial class MatchView : UserControl
{
    public MatchView()
    {
        InitializeComponent();
        FollowFeed(lstRecentMoves, scrRecentMoves);
        FollowFeed(lstMatchChat, scrMatchChat);
    }

    private static void FollowFeed(ItemsControl items, ScrollViewer viewport)
    {
        bool scrollQueued = false;
        items.Items.CollectionChanged += (_, _) => QueueScroll();
        viewport.SizeChanged += (_, _) => QueueScroll();

        void QueueScroll()
        {
            if (!scrollQueued)
            {
                scrollQueued = true;
                Dispatcher.UIThread.Post(() =>
                {
                    scrollQueued = false;
                    viewport.ScrollToEnd();
                }, DispatcherPriority.Loaded);
            }
        }
    }
}
