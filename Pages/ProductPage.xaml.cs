using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace ImplicitAnimations.Pages
{
    /// <summary>
    /// Data class for parameter information that controls:
    /// - What image to animate
    /// - Where to animate it from
    /// - If we should play a nice animation
    /// </summary>
    public class ProductNavigation
    {
        /// <summary>
        /// Image that we're animating / displaying on our product page
        /// </summary>
        public string ImageUri;

        /// <summary>
        /// Source coordinate of the animation
        /// </summary>
        public Point Position;

        /// <summary>
        /// Animation type
        /// </summary>
        public PageAnimationType Animation = PageAnimationType.Complex;
    }

    public sealed partial class ProductPage : Page
    {
        public ProductPage()
        {
            this.InitializeComponent();

            var data = new List<int>(10);
            for (int i = 1; i <= 10; i++)
            {
                data.Add(i);
            }

            this.ItemList.ItemsSource = data;
        }

        private void ItemList_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            // Simplistic stripe effect behind the liste items
            args.ItemContainer.Background = (((int)args.Item) % 2 == 0) ? new SolidColorBrush(Colors.LightGray) : new SolidColorBrush(Colors.White);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var param = e.Parameter as ProductNavigation;
            var url = param.ImageUri;
            var source = new BitmapImage(new Uri(url));

            // Set the images on the destination rendering points
            this.Animal.Source = source;
            this.BackDrop.Source = source;

            // Run the complex animation if, well, it's a complex type.
            if (param.Animation == PageAnimationType.Complex)
            {
                this.RunComplexAnimation(param);
            }

            base.OnNavigatedTo(e);
        }

        private void RunComplexAnimation(ProductNavigation param)
        {
            var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            #region Collection/List Animation
            // Transform from the bottom of the viewport to final position
            var collectionStart = "collection.Size.y + this.StartingValue";
            var collectionEnd = "this.StartingValue";

            var listTranslateIn = compositor.CreateScalarKeyFrameAnimation();
            listTranslateIn.Duration = Constants.DefaultAnimationDuration;
            listTranslateIn.SetReferenceParameter("collection", ElementCompositionPreview.GetElementVisual(this.ItemList));
            listTranslateIn.InsertExpressionKeyFrame(0.0f, collectionStart);
            listTranslateIn.InsertExpressionKeyFrame(1.0f, collectionEnd);
            listTranslateIn.Target = "Offset.Y";

            // Fade it in also, to mimize the jarring effect
            var listFadeIn = compositor.CreateScalarKeyFrameAnimation();
            listFadeIn.Duration = Constants.DefaultAnimationDuration;
            listFadeIn.Target = "Opacity";
            listFadeIn.InsertKeyFrame(0.0f, 0.0f);
            listFadeIn.InsertKeyFrame(0.5f, 0.0f);
            listFadeIn.InsertKeyFrame(1.0f, 1.0f);

            // Group the animations together
            var listEntrance = compositor.CreateAnimationGroup();
            listEntrance.Add(listTranslateIn);
            listEntrance.Add(listFadeIn);

            ElementCompositionPreview.SetImplicitShowAnimation(this.ItemList, listEntrance);
            #endregion Collection/List Animation

            #region Backdrop Animation
            // Animates the backdrop behind the text/image from the originating location,
            // while also 'croping' from the sequare to destination shape
            var backdropEntrance = compositor.CreateAnimationGroup();

            // Translate it frmo the *source* position
            var backdropTranslateY = compositor.CreateScalarKeyFrameAnimation();
            backdropTranslateY.Duration = Constants.DefaultAnimationDuration;
            backdropTranslateY.SetScalarParameter("beginY", (float)param.Position.Y);
            backdropTranslateY.InsertExpressionKeyFrame(0.0f, "beginY");
            backdropTranslateY.InsertExpressionKeyFrame(1.0f, "this.StartingValue");
            backdropTranslateY.Target = "Offset.Y";

            var backdropTranslateX = compositor.CreateScalarKeyFrameAnimation();
            backdropTranslateX.Duration = Constants.DefaultAnimationDuration;
            backdropTranslateX.SetScalarParameter("beginX", (float)param.Position.X);
            backdropTranslateX.InsertExpressionKeyFrame(0.0f, "beginX");
            backdropTranslateX.InsertExpressionKeyFrame(1.0f, "this.StartingValue");
            backdropTranslateX.Target = "Offset.X";

            // Scale it to match the final width
            var backdropScale = compositor.CreateVector3KeyFrameAnimation();

            backdropScale.SetReferenceParameter("backdrop", ElementCompositionPreview.GetElementVisual(this.BackdropContainer));
            backdropScale.InsertExpressionKeyFrame(0.0f, "Vector3(202/backdrop.Size.X, 202 / backdrop.Size.Y, 0.0)");
            backdropScale.InsertKeyFrame(1.0f, new Vector3(1.0f));
            backdropScale.Target = nameof(Visual.Scale);
            backdropScale.Duration = Constants.DefaultAnimationDuration;

            backdropEntrance.Add(backdropTranslateY);
            backdropEntrance.Add(backdropTranslateX);
            backdropEntrance.Add(backdropScale);

            ElementCompositionPreview.SetImplicitShowAnimation(this.BackdropContainer, backdropEntrance);

            // Clip the backdrop so it changes from square to rectangle
            var backdropBottomClip = compositor.CreateScalarKeyFrameAnimation();
            var backdropVisual = ElementCompositionPreview.GetElementVisual(this.BackdropContainer);
            backdropBottomClip.Target = "BottomInset";
            var clippy = compositor.CreateInsetClip();
            backdropVisual.Clip = clippy;
            backdropBottomClip.Duration = Constants.DefaultAnimationDuration;
            backdropBottomClip.SetReferenceParameter("backdrop", backdropVisual);
            backdropBottomClip.InsertExpressionKeyFrame(0.0f, "0");
            backdropBottomClip.InsertExpressionKeyFrame(1.0f, "(backdrop.Size.Y - 250)");
            clippy.StartAnimation("BottomInset", backdropBottomClip);
            #endregion Backdrop Animation

            #region Image Animation
            var imageEntrance = compositor.CreateAnimationGroup();

            // Opacity
            var imageOpacity = compositor.CreateScalarKeyFrameAnimation();
            imageOpacity.Target = "Opacity";
            imageOpacity.Duration = Constants.DefaultAnimationDuration;
            imageOpacity.InsertKeyFrame(0.0f, 0.0f);
            imageOpacity.InsertKeyFrame(0.5f, 0.0f); // Start the fade part-way through the transition
            imageOpacity.InsertKeyFrame(1.0f, 1.0f);

            // Transform to final position
            var imageTranslate = compositor.CreateVector3KeyFrameAnimation();
            imageTranslate.Duration = Constants.DefaultAnimationDuration;
            imageTranslate.InsertExpressionKeyFrame(0.0f, "Vector3(500, 500, -this.StartingValue.Z)");
            imageTranslate.InsertExpressionKeyFrame(1.0f, "this.StartingValue");
            imageTranslate.Target = "Offset";

            imageEntrance.Add(imageOpacity);
            imageEntrance.Add(imageTranslate);

            ElementCompositionPreview.SetImplicitShowAnimation(this.Animal, imageEntrance);
            #endregion Image Animation

            #region Text Animation
            // Text should start to fade in halfway through the animation
            var textOpacity = compositor.CreateScalarKeyFrameAnimation();
            textOpacity.Target = "Opacity";
            textOpacity.Duration = Constants.DefaultAnimationDuration;
            textOpacity.InsertKeyFrame(0.0f, 0.0f);
            textOpacity.InsertKeyFrame(0.6f, 0.0f);
            textOpacity.InsertKeyFrame(1.0f, 1.0f);

            ElementCompositionPreview.SetImplicitShowAnimation(this.TextContainer, textOpacity);
            #endregion
        }
    }
}
