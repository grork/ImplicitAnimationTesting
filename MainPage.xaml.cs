using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

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
        public LogicalNavigationDirection Direction = LogicalNavigationDirection.None;
        public string Parameter;
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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
            if(MainFrame.CanGoBack)
            {
                MainFrame.GoBack();
            }
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            this.NavView.SelectedItem = this.NavView.MenuItems[1];
            this.MainFrame.Navigate(typeof(Pages.CollectionPage));
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var pageName = args.InvokedItem as string;

            switch(pageName)
            {
                case "TestPage":
                    this.MainFrame.Navigate(typeof(Pages.TestPage));
                    break;

                case "CollectionPage":
                case "CollectionPage1":
                    this.MainFrame.Navigate(typeof(Pages.CollectionPage), new NavigationParameter { Parameter = pageName });
                    break;

                case "PDPPage":
                    this.MainFrame.Navigate(typeof(Pages.PDPPage));
                    break;
            }
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            var nav = SystemNavigationManager.GetForCurrentView();
            if(MainFrame.CanGoBack)
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
            if(param == null)
            {
                // Nothing we can do here
                return;
            }

            if(previousParameter == null)
            {
                param.Direction = LogicalNavigationDirection.Up;
            }
            else if ((param.Parameter == previousParameter.Parameter)
                || (param.Parameter == "CollectionPage1" && previousParameter.Parameter == "CollectionPage"))
            {
                param.Direction = LogicalNavigationDirection.Up;
            }
            else if (param.Parameter == "CollectionPage" && previousParameter.Parameter == "CollectionPage1")
            {
                param.Direction = LogicalNavigationDirection.Down;
            }
        }
    }
}
