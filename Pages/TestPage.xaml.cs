using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ImplicitAnimations.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TestPage : Page
    {
        public TestPage()
        {
            this.InitializeComponent();
            Canvas.SetZIndex(this, 10);
        }

        private void AddItem(object sender, RoutedEventArgs e)
        {
            var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            var showAnimation = compositor.CreateScalarKeyFrameAnimation();
            showAnimation.InsertKeyFrame(0.0f, 0.0f);
            showAnimation.InsertKeyFrame(1.0f, 1.0f);
            showAnimation.Target = nameof(Visual.Opacity);
            showAnimation.Duration = TimeSpan.FromSeconds(2.5f);

            var hideAnimation = compositor.CreateVector3KeyFrameAnimation();
            hideAnimation.InsertKeyFrame(0.0f, new Vector3(1.0f));
            hideAnimation.InsertKeyFrame(1.0f, new Vector3(0.0f));
            hideAnimation.Target = nameof(Visual.Scale);
            hideAnimation.Duration = TimeSpan.FromSeconds(2.5f);

            Rectangle rect = new Rectangle();
            rect.Width = rect.Height = 300;
            rect.Fill = new SolidColorBrush(Colors.Orange);

            Visual f = ElementCompositionPreview.GetElementVisual(rect);
            f.CenterPoint = new Vector3(150.0f, 150.0f, 0.0f);

            ElementCompositionPreview.SetImplicitShowAnimation(rect, showAnimation);
            ElementCompositionPreview.SetImplicitHideAnimation(rect, hideAnimation);

            this.ContentContainer.Children.Add(rect);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.AddItem(null, null);
        }

        private void RemoveItem(object sender, RoutedEventArgs e)
        {
            if (this.ContentContainer.Children.Count < 1)
            {
                return;
            }

            this.ContentContainer.Children.Remove(this.ContentContainer.Children.Last());
        }
    }
}
