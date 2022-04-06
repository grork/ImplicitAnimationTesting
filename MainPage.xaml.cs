using ImplicitAnimations.Pages;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ImplicitAnimations
{
    public enum LogicalNavigationDirection
    {
        None,
        Down,
        Up
    }

    public class NavigationParameter
    {
        public string PageIdentifier = "Unknown";
        public LogicalNavigationDirection Direction = LogicalNavigationDirection.None;
        public PageAnimationType AnimationType = PageAnimationType.Simple;
    }

    public sealed partial class MainPage : Page
    {
        private NavigationParameter m_previousParameter;

        public MainPage()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
        }

        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                MainFrame.GoBack();
            }
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // Select the default navigation item as 'CollectionPage'
            this.NavView.SelectedItem = this.NavView.MenuItems.First((o) =>
            {
                return ((o as NavigationViewItem).Content as string) == "CollectionPage";
            });

            this.MainFrame.Navigate(typeof(Pages.CollectionPage));
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var pageName = args.InvokedItem as string;

            switch (pageName)
            {
                case "TestPage":
                    this.MainFrame.Navigate(typeof(Pages.TestPage));
                    break;

                case "CollectionPage":
                    this.MainFrame.Navigate(typeof(Pages.CollectionPage), new NavigationParameter
                    {
                        PageIdentifier = pageName,
                        AnimationType = PageAnimationType.Complex
                    });
                    break;

                case "CollectionPage1":
                    this.MainFrame.Navigate(typeof(Pages.CollectionPage), new NavigationParameter
                    {
                        PageIdentifier = pageName,
                        AnimationType = PageAnimationType.Simple
                    });
                    break;

                case "PDPPage":
                    this.MainFrame.Navigate(typeof(Pages.PDPPage), new PDPNavigation { ImageUri = "https://placeimg.com/202/202/animals" });
                    break;
            }
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            var nav = SystemNavigationManager.GetForCurrentView();
            if (MainFrame.CanGoBack)
            {
                nav.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            else
            {
                nav.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
        }

        private void MainFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            NavigationParameter previousParameter = m_previousParameter;
            NavigationParameter param = e.Parameter as NavigationParameter;
            m_previousParameter = param;
            if (param == null)
            {
                // Nothing we can do here
                return;
            }

            if (previousParameter == null)
            {
                param.Direction = LogicalNavigationDirection.Up;
            }
            else if ((param.PageIdentifier == previousParameter.PageIdentifier)
                || (param.PageIdentifier == "CollectionPage1" && previousParameter.PageIdentifier == "CollectionPage"))
            {
                param.Direction = LogicalNavigationDirection.Up;
            }
            else if (param.PageIdentifier == "CollectionPage" && previousParameter.PageIdentifier == "CollectionPage1")
            {
                param.Direction = LogicalNavigationDirection.Down;
            }
        }
    }
}
