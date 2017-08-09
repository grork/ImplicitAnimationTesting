using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ImplicitAnimations.Pages
{
    public class CollectionItem
    {
        public string Title { get; set; }
        public string Image { get; set; }
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CollectionPage : Page
    {
        public static readonly TimeSpan animationDuration = TimeSpan.FromSeconds(0.5f);

        public CollectionPage()
        {
            this.InitializeComponent();

            Canvas.SetZIndex(this, 10);

            this.GenerateData().ContinueWith((result) =>
            {
                this.CollectionList.ItemsSource = result.Result;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public async Task<IList<CollectionItem>> GenerateData()
        {
            return await Task.Run<IList<CollectionItem>>(() =>
            {
                List<CollectionItem> items = new List<CollectionItem>(500);

                for (int i = 0; i < 500; i++)
                {
                    items.Add(new CollectionItem
                    {
                        Title = "I am item " + i,
                        Image = "https://placeimg.com/202/202/animals?foo=" + i
                    });
                }

                return items;
            });
        }

        private void CollectionList_ItemClick(object sender, ItemClickEventArgs e)
        {
            UIElement focusedElement = FocusManager.GetFocusedElement() as UIElement;

            var position = focusedElement.TransformToVisual(this);
            var point = position.TransformPoint(new Point(0.0, 0.0));
            this.Frame.Navigate(typeof(PDPPage), new PDPNavigation { ImageUri = (e.ClickedItem as CollectionItem).Image, Position = point });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            var param = e.Parameter as NavigationParameter;

            if(param != null)
            {
                this.HeaderText.Text = param.Parameter;
            }

            if (param != null && param.Direction != LogicalNavigationDirection.None)
            {
                string collectionStart = String.Empty;
                string collectionEnd = String.Empty;
                string headerStart = String.Empty;
                string headerEnd = String.Empty;

                switch(param.Direction)
                {
                    case LogicalNavigationDirection.Up:
                        collectionStart = "A.Size.y + this.StartingValue";
                        collectionEnd = "this.StartingValue";

                        headerStart = "A.Size.y + A.Offset.Y + this.StartingValue";
                        headerEnd = "this.StartingValue";
                        break;

                    case LogicalNavigationDirection.Down:
                        collectionStart = "-(A.Size.y + this.StartingValue)";
                        collectionEnd = "this.StartingValue"; 

                        headerStart = "-(A.Size.y + this.StartingValue)";
                        headerEnd = "this.StartingValue";
                        break;
                }

                var collectionIntroAnimation = compositor.CreateScalarKeyFrameAnimation();
                collectionIntroAnimation.Duration = animationDuration;
                collectionIntroAnimation.SetReferenceParameter("A", ElementCompositionPreview.GetElementVisual(this.CollectionList));
                collectionIntroAnimation.InsertExpressionKeyFrame(0.0f, collectionStart);
                collectionIntroAnimation.InsertExpressionKeyFrame(1.0f, collectionEnd);
                collectionIntroAnimation.Target = "Offset.Y";

                ElementCompositionPreview.SetImplicitShowAnimation(this.CollectionList, collectionIntroAnimation);

                var headerIntroAnimation = compositor.CreateScalarKeyFrameAnimation();
                headerIntroAnimation.Duration = animationDuration;
                headerIntroAnimation.SetReferenceParameter("A", ElementCompositionPreview.GetElementVisual(this.CollectionList));
                headerIntroAnimation.InsertExpressionKeyFrame(0.0f, headerStart);
                headerIntroAnimation.InsertExpressionKeyFrame(0.01f, headerStart);
                headerIntroAnimation.InsertExpressionKeyFrame(1.0f, headerEnd);
                headerIntroAnimation.Target = "Offset.Y";

                ElementCompositionPreview.SetImplicitShowAnimation(this.Header, headerIntroAnimation);
            }

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            var param = e.Parameter as NavigationParameter;

            if (param != null && param.Direction != LogicalNavigationDirection.None)
            {
                string collectionStart = String.Empty;
                string collectionEnd = String.Empty;
                string headerStart = String.Empty;
                string headerEnd = String.Empty;

                switch (param.Direction)
                {
                    case LogicalNavigationDirection.Up:
                        collectionStart = "this.StartingValue";
                        collectionEnd = "-(A.Size.y)";

                        headerStart = "this.StartingValue";
                        headerEnd = "-(A.Size.y)";
                        Canvas.SetZIndex(this.Header, 1);
                        break;

                    case LogicalNavigationDirection.Down:
                        collectionStart = "this.StartingValue";
                        collectionEnd = "A.Size.y";

                        headerStart = "this.StartingValue";
                        headerEnd = "A.Size.y + this.StartingValue";
                        break;
                }

                var collectionExitAnimation = compositor.CreateScalarKeyFrameAnimation();
                collectionExitAnimation.Duration = animationDuration;
                collectionExitAnimation.SetReferenceParameter("A", ElementCompositionPreview.GetElementVisual(this.CollectionList));
                collectionExitAnimation.InsertExpressionKeyFrame(0.0f, collectionStart);
                collectionExitAnimation.InsertExpressionKeyFrame(1.0f, collectionEnd);
                collectionExitAnimation.Target = "Offset.Y";

                ElementCompositionPreview.SetImplicitHideAnimation(this.CollectionList, collectionExitAnimation);

                var headerExitAnimation = compositor.CreateScalarKeyFrameAnimation();
                headerExitAnimation.Duration = animationDuration;
                headerExitAnimation.SetReferenceParameter("A", ElementCompositionPreview.GetElementVisual(this.Header));
                headerExitAnimation.InsertExpressionKeyFrame(0.0f, headerStart);
                headerExitAnimation.InsertExpressionKeyFrame(1.0f, headerEnd);
                headerExitAnimation.Target = "Offset.Y";
                ElementCompositionPreview.SetImplicitHideAnimation(this.Header, headerExitAnimation);

            }
            else if (e.Parameter is PDPNavigation)
            {
                var pageAnimation = compositor.CreateScalarKeyFrameAnimation();
                pageAnimation.Target = "Opacity";
                pageAnimation.Duration = animationDuration;
                pageAnimation.InsertKeyFrame(0.0f, 1.0f);

                pageAnimation.InsertKeyFrame(0.1f, 1.0f);
                pageAnimation.InsertKeyFrame(1.0f, 0.0f);

                ElementCompositionPreview.SetImplicitHideAnimation(this, pageAnimation);
            }
            base.OnNavigatingFrom(e);
        }
    }
}
