using ImplicitAnimations.Pages;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ImplicitAnimations
{
    /// <summary>
    /// Visually, Navigation has a physicality to it. Code doesn't care.
    /// We need to represent it in code to ensure we play the right animation.
    /// </summary>
    public enum PhysicalNavigationDirection
    {
        /// <summary>
        /// No-known animation directon
        /// </summary>
        None,

        /// <summary>
        /// We're going to an item that is visually 'below' where we are
        /// </summary>
        Down,

        /// <summary>
        /// We're going to an item that is visually 'above' where we are
        /// </summary>
        Up
    }

    /// <summary>
    /// Container class holding information that the navigation 'manager' can use
    /// to perform navigation, and derive what animations etc should be.
    /// </summary>
    public class NavigationParameter
    {
        public string PageIdentifier = "Unknown";
        public PhysicalNavigationDirection Direction = PhysicalNavigationDirection.None;
        public PageAnimationType AnimationType = PageAnimationType.Simple;
    }

    public sealed partial class MainPage : Page
    {
        private NavigationParameter m_previousPageParameter;

        public MainPage()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;

            // Navigation List
            this.NavView.MenuItemsSource = new List<string> {
                PageIdentifiers.CollectionPage1,
                PageIdentifiers.CollectionPage2,
                PageIdentifiers.TestPage
            };
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
            // Select the default navigation item as 'CollectionPage' & Navigate to it
            this.NavView.SelectedItem = PageIdentifiers.CollectionPage1;
            this.MainFrame.Navigate(typeof(Pages.CollectionPage));
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var pageName = args.InvokedItem as string;

            // Deduce which page is actually being navigated to
            switch (pageName)
            {
                case PageIdentifiers.TestPage:
                    this.MainFrame.Navigate(typeof(Pages.TestPage));
                    break;

                case PageIdentifiers.CollectionPage1:
                    this.MainFrame.Navigate(typeof(Pages.CollectionPage), new NavigationParameter {
                        PageIdentifier = pageName,
                        AnimationType = PageAnimationType.Complex
                    });
                    break;

                case PageIdentifiers.CollectionPage2:
                    this.MainFrame.Navigate(typeof(Pages.CollectionPage), new NavigationParameter
                    {
                        PageIdentifier = pageName,
                        AnimationType = PageAnimationType.Simple
                    });
                    break;

                case PageIdentifiers.ProductPage:
                    this.MainFrame.Navigate(typeof(Pages.PDPPage), new PDPNavigation { ImageUri = "https://placeimg.com/202/202/animals" });
                    break;
            }
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            // update the back button state based on the main frames backstack
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
            NavigationParameter previousParameter = m_previousPageParameter;
            NavigationParameter param = e.Parameter as NavigationParameter;
            m_previousPageParameter = param;
            
            if (param == null)
            {
                // Nothing we can do here
                return;
            }

            // Assume if there is no prevous page that there is no animation
            if (previousParameter == null)
            {
                param.Direction = PhysicalNavigationDirection.None;
            }
            // If we're going from collection page 2 to 1, that is physically going 'up'
            else if ((param.PageIdentifier == previousParameter.PageIdentifier)
                || (param.PageIdentifier == PageIdentifiers.CollectionPage2 && previousParameter.PageIdentifier == PageIdentifiers.CollectionPage1))
            {
                param.Direction = PhysicalNavigationDirection.Up;
            }
            // If we're going from collection page 1 to 2, that is logically down
            else if (param.PageIdentifier == PageIdentifiers.CollectionPage1 && previousParameter.PageIdentifier == PageIdentifiers.CollectionPage2)
            {
                param.Direction = PhysicalNavigationDirection.Down;
            }
        }
    }
}
