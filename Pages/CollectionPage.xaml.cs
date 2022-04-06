using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace ImplicitAnimations.Pages
{
    /// <summary>
    /// Data holding class to represent the items displayed in the list
    /// </summary>
    public class CollectionItem
    {
        public string Title { get; set; }
        public string Image { get; set; }
    }

    /// <summary>
    /// A page with a list of items that can be invoked to animate to a detailed page.
    /// </summary>
    public sealed partial class CollectionPage : Page
    {
        private PageAnimationType m_pageAnimation = PageAnimationType.Complex;

        public CollectionPage()
        {
            this.InitializeComponent();

            // Create some fake data off the UI thread, and then force setting of the data source
            // on the UI thread.
            _ = this.GenerateData().ContinueWith((result) =>
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

        /// <summary>
        /// Handle a single item being clicked, and animate to the destination page
        /// </summary>
        private void CollectionList_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Get the clicked or keyboard involved item
            UIElement focusedElement = FocusManager.GetFocusedElement() as UIElement;

            // Transform position information so we can pass source position
            // to the destination page
            var position = focusedElement.TransformToVisual(this);
            var point = position.TransformPoint(new Point(0.0, 0.0));

            this.Frame.Navigate(typeof(ProductPage), new ProductNavigation
            {
                ImageUri = (e.ClickedItem as CollectionItem).Image, // Image that will be scaled & positioned
                Position = point, // Position that we want to animate *from*
                Animation = m_pageAnimation
            }, new SuppressNavigationTransitionInfo());
        }

        /// <summary>
        /// Creates an animation for *entering* this page. When navigating from a
        /// product page, this does nothing. But when returing to this page from
        /// another collection page, it'll animate in the appropriate direction.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            // We only have this if it's not from a product page.
            if (e.Parameter is NavigationParameter param)
            {
                // Default data
                this.HeaderText.Text = param.PageIdentifier;
                this.m_pageAnimation = param.AnimationType;

                // Only if we have a direction should we attempt to configure it
                if (param.Direction != PhysicalNavigationDirection.None)
                {
                    // Expressions for the composition animation
                    var collectionStart = string.Empty;
                    var collectionEnd = string.Empty;
                    var headerStart = string.Empty;
                    var headerEnd = string.Empty;

                    // Assumption of all of these is we want to animate *to* the
                    // position, not *from* the position. This makes it easier to ensure
                    // things settle in the right place as an animation completes.
                    // In these expressions, thats what "this.StartingValue" is. See
                    // https://docs.microsoft.com/en-us/uwp/api/Windows.UI.Composition.ExpressionAnimation?msclkid=32232c1ab5ec11ecb9b06016e094a4b2&view=winrt-22000
                    // 
                    // In the animations, the collection item is named "collection" (see calls to SetReferenceParameter)
                    switch (param.Direction)
                    {
                        case PhysicalNavigationDirection.Up:
                            // Start from our Height + starting position (E.g. put the top of our collection
                            // at the bottom, off the page)
                            collectionStart = "collection.Size.y + this.StartingValue";
                            collectionEnd = "this.StartingValue";

                            // Move the header by a position relative to the *collection* reference item
                            headerStart = "collection.Size.y + collection.Offset.Y + this.StartingValue";
                            headerEnd = "this.StartingValue";
                            break;

                        case PhysicalNavigationDirection.Down:
                            // Start with the item off the top of the screen
                            collectionStart = "-(collection.Size.y + this.StartingValue)";
                            collectionEnd = "this.StartingValue";

                            // Move the header from off the screen
                            headerStart = "-(collection.Size.y + this.StartingValue)";
                            headerEnd = "this.StartingValue";
                            break;
                    }

                    // Animation set on the collection itself
                    var collectionIntroAnimation = compositor.CreateScalarKeyFrameAnimation();
                    collectionIntroAnimation.Duration = Constants.DefaultAnimationDuration;
                    collectionIntroAnimation.SetReferenceParameter("collection", ElementCompositionPreview.GetElementVisual(this.CollectionList));
                    collectionIntroAnimation.InsertExpressionKeyFrame(0.0f, collectionStart);
                    collectionIntroAnimation.InsertExpressionKeyFrame(1.0f, collectionEnd);
                    collectionIntroAnimation.Target = "Offset.Y";

                    ElementCompositionPreview.SetImplicitShowAnimation(this.CollectionList, collectionIntroAnimation);

                    // Animation on the header
                    var headerIntroAnimation = compositor.CreateScalarKeyFrameAnimation();
                    headerIntroAnimation.Duration = Constants.DefaultAnimationDuration;
                    headerIntroAnimation.SetReferenceParameter("collection", ElementCompositionPreview.GetElementVisual(this.CollectionList));
                    headerIntroAnimation.InsertExpressionKeyFrame(0.0f, headerStart);
                    headerIntroAnimation.InsertExpressionKeyFrame(0.01f, headerStart);
                    headerIntroAnimation.InsertExpressionKeyFrame(1.0f, headerEnd);
                    headerIntroAnimation.Target = "Offset.Y";

                    ElementCompositionPreview.SetImplicitShowAnimation(this.Header, headerIntroAnimation);
                }
            }

            base.OnNavigatedTo(e);
        }
        /// <summary>
        /// Create an animation representing the dismissal of this page. This is the mirror
        /// of the OnNavigatingTo method, with the additional of a basic fade when the destination
        /// is a Product Page
        /// </summary>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            // Only have dismiss animation if we have a navigation page and a direction
            if (e.Parameter is NavigationParameter param && param.Direction != PhysicalNavigationDirection.None)
            {
                var collectionStart = string.Empty;
                var collectionEnd = string.Empty;
                var headerStart = string.Empty;
                var headerEnd = string.Empty;

                switch (param.Direction)
                {
                    case PhysicalNavigationDirection.Up:
                        // Animate things from their intial position *above* the
                        // viewport
                        collectionStart = "this.StartingValue";
                        collectionEnd = "-(collection.Size.y)";

                        headerStart = "this.StartingValue";
                        headerEnd = "-(collection.Size.y)";
                        break;

                    case PhysicalNavigationDirection.Down:
                        // Move things off the *bottom* of the viewport
                        collectionStart = "this.StartingValue";
                        collectionEnd = "collection.Size.y";

                        headerStart = "this.StartingValue";
                        headerEnd = "collection.Size.y + this.StartingValue";
                        break;
                }

                var collectionExitAnimation = compositor.CreateScalarKeyFrameAnimation();
                collectionExitAnimation.Duration = Constants.DefaultAnimationDuration;
                collectionExitAnimation.SetReferenceParameter("collection", ElementCompositionPreview.GetElementVisual(this.CollectionList));
                collectionExitAnimation.InsertExpressionKeyFrame(0.0f, collectionStart);
                collectionExitAnimation.InsertExpressionKeyFrame(1.0f, collectionEnd);
                collectionExitAnimation.Target = "Offset.Y";

                ElementCompositionPreview.SetImplicitHideAnimation(this.CollectionList, collectionExitAnimation);

                var headerExitAnimation = compositor.CreateScalarKeyFrameAnimation();
                headerExitAnimation.Duration = Constants.DefaultAnimationDuration;
                headerExitAnimation.SetReferenceParameter("collection", ElementCompositionPreview.GetElementVisual(this.Header));
                headerExitAnimation.InsertExpressionKeyFrame(0.0f, headerStart);
                headerExitAnimation.InsertExpressionKeyFrame(1.0f, headerEnd);
                headerExitAnimation.Target = "Offset.Y";
                ElementCompositionPreview.SetImplicitHideAnimation(this.Header, headerExitAnimation);

            }
            else if (e.Parameter is ProductNavigation)
            {
                var pageAnimation = compositor.CreateScalarKeyFrameAnimation();
                pageAnimation.Target = "Opacity";
                pageAnimation.Duration = Constants.DefaultAnimationDuration;
                pageAnimation.InsertKeyFrame(0.0f, 1.0f);
                // 0.1 is to hold the position for part of the animation to give
                // a visually staggered effect.
                pageAnimation.InsertKeyFrame(0.1f, 1.0f);
                pageAnimation.InsertKeyFrame(1.0f, 0.0f);

                ElementCompositionPreview.SetImplicitHideAnimation(this, pageAnimation);
            }

            base.OnNavigatingFrom(e);
        }
    }
}
