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
        static bool s_firstNav = true;

        public CollectionPage()
        {
            this.InitializeComponent();

            Canvas.SetZIndex(this, 10);

            this.GenerateData().ContinueWith((result) =>
            {
                this.CollectionList.ItemsSource = result.Result;
            }, TaskScheduler.FromCurrentSynchronizationContext());

            var animationDuration = TimeSpan.FromSeconds(1.0f);

            var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            if(!s_firstNav)
            {
                var collectionIntroAnimation = compositor.CreateScalarKeyFrameAnimation();
                collectionIntroAnimation.Duration = animationDuration;
                collectionIntroAnimation.SetReferenceParameter("A", ElementCompositionPreview.GetElementVisual(this.CollectionList));
                collectionIntroAnimation.InsertExpressionKeyFrame(0.0f, "A.Size.y + this.StartingValue");
                collectionIntroAnimation.InsertExpressionKeyFrame(1.0f, "this.StartingValue");
                collectionIntroAnimation.Target = "Offset.Y";

                ElementCompositionPreview.SetImplicitShowAnimation(this.CollectionList, collectionIntroAnimation);

                var headerIntroAnimation = compositor.CreateScalarKeyFrameAnimation();
                headerIntroAnimation.Duration = animationDuration;
                headerIntroAnimation.SetReferenceParameter("A", ElementCompositionPreview.GetElementVisual(this.CollectionList));
                headerIntroAnimation.InsertExpressionKeyFrame(0.0f, "A.Size.y + A.Offset.Y + this.StartingValue");
                headerIntroAnimation.InsertExpressionKeyFrame(0.01f, "A.Size.y + A.Offset.Y + this.StartingValue");
                headerIntroAnimation.InsertExpressionKeyFrame(1.0f, "this.StartingValue");
                headerIntroAnimation.Target = "Offset.Y";

                ElementCompositionPreview.SetImplicitShowAnimation(this.Header, headerIntroAnimation);
            }

            var collectionExitAnimation = compositor.CreateScalarKeyFrameAnimation();
            collectionExitAnimation.Duration = animationDuration;
            collectionExitAnimation.SetReferenceParameter("A", ElementCompositionPreview.GetElementVisual(this.CollectionList));
            collectionExitAnimation.InsertExpressionKeyFrame(0.0f, "this.StartingValue");
            collectionExitAnimation.InsertExpressionKeyFrame(1.0f, "-(A.Size.y)");
            collectionExitAnimation.Target = "Offset.Y";

            ElementCompositionPreview.SetImplicitHideAnimation(this.CollectionList, collectionExitAnimation);

            var headerExitAnimation = compositor.CreateScalarKeyFrameAnimation();
            headerExitAnimation.Duration = animationDuration;
            headerExitAnimation.SetReferenceParameter("A", ElementCompositionPreview.GetElementVisual(this.Header));
            headerExitAnimation.InsertExpressionKeyFrame(0.0f, "this.StartingValue");
            headerExitAnimation.InsertExpressionKeyFrame(1.0f, "-(A.Size.y)");
            headerExitAnimation.Target = "Offset.Y";
            ElementCompositionPreview.SetImplicitHideAnimation(this.Header, headerExitAnimation);

            s_firstNav = false;
        }

        public async Task<IList<CollectionItem>> GenerateData()
        {
            return await Task.Run<IList<CollectionItem>>(() =>
            {
                List<CollectionItem> items = new List<CollectionItem>(500);

                for(int i = 0; i < 500; i++)
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
            this.Frame.Navigate(typeof(PDPPage), (e.ClickedItem as CollectionItem).Image);
        }
    }
}
