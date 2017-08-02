using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            this.NavView.SelectedItem = this.NavView.MenuItems[0];
            this.MainFrame.Navigate(typeof(Pages.Page1));
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var pageName = args.InvokedItem as string;

            switch(pageName)
            {
                case "Page1":
                    this.MainFrame.Navigate(typeof(Pages.Page1));
                    break;

                case "Page2":
                    this.MainFrame.Navigate(typeof(Pages.Page2));
                    break;

                case "Page3":
                    this.MainFrame.Navigate(typeof(Pages.Page3));
                    break;
            }
        }
    }
}
