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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
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
                    this.MainFrame.Navigate(typeof(Pages.CollectionPage));
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
    }
}
