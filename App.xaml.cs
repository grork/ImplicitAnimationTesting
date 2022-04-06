using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ImplicitAnimations
{
    /// <summary>
    /// Identifiers to represent the different pages in a structured way.
    /// </summary>
    internal static class PageIdentifiers
    {
        internal const string CollectionPage1 = "collection-page1";
        internal const string CollectionPage2 = "collection-page2";
        internal const string TestPage = "test-page";
        internal const string ProductPage = "product-page";
    }

    public enum PageAnimationType
    {
        Complex,
        Simple
    }

    public static class Constants
    {
        public static readonly TimeSpan DefaultAnimationDuration = TimeSpan.FromSeconds(0.5f);
    }

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

    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                Window.Current.Content = rootFrame = new Frame();
            }

            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }

            Window.Current.Activate();
        }
    }
}
