using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ImplicitAnimations
{
    internal static class PageIdentifiers
    {
        internal const string CollectionPage1 = "collection-page1";
        internal const string CollectionPage2 = "collection-page2";
        internal const string TestPage = "test-page";
        internal const string ProductPage = "product-page";
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
