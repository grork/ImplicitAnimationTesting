using ImplicitAnimations.Pages;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using muxc = Microsoft.UI.Xaml.Controls;

namespace ImplicitAnimations
{
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
        }

        private void NavView_SelectionChanged(muxc.NavigationView sender, muxc.NavigationViewSelectionChangedEventArgs args)
        {
            var pageName = args.SelectedItem as string;
            this.NavigateToItem(pageName);
        }

        private void NavigateToItem(string pageName)
        {
            // Deduce which page is actually being navigated to
            switch (pageName)
            {
                case PageIdentifiers.TestPage:
                    this.MainFrame.Navigate(typeof(Pages.TestPage));
                    break;

                case PageIdentifiers.CollectionPage1:
                    this.MainFrame.Navigate(typeof(Pages.CollectionPage), new NavigationParameter {
                        PageIdentifier = pageName
                    }, new SuppressNavigationTransitionInfo());
                    break;

                case PageIdentifiers.CollectionPage2:
                    this.MainFrame.Navigate(typeof(Pages.CollectionPage), new NavigationParameter
                    {
                        PageIdentifier = pageName
                    }, new SuppressNavigationTransitionInfo());
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

            NavView.SelectedItem = param.PageIdentifier;

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
