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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ImplicitAnimations.Pages
{
    public enum PageAnimationType
    {
        Complex,
        Simple
    }

    public class PDPNavigation
    {
        public string ImageUri;
        public Point Position;
        public PageAnimationType Animation = PageAnimationType.Complex;
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PDPPage : Page
    {
        public PDPPage()
        {
            this.InitializeComponent();

            var data = new List<int>(10);
            for (int i = 1; i <= 10; i++)
            {
                data.Add(i);
            }

            this.PDPList.ItemsSource = data;
            Canvas.SetZIndex(this, 11);
        }

        private void PDPList_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            args.ItemContainer.Background = (((int)args.Item) % 2 == 0) ? new SolidColorBrush(Colors.LightGray) : new SolidColorBrush(Colors.White);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PDPNavigation param = e.Parameter as PDPNavigation;

            string url = param.ImageUri;

            var source = new BitmapImage(new Uri(url));
            this.Animal.Source = source;
            this.BackDrop.Source = source;

            switch (param.Animation)
            {
                case PageAnimationType.Complex:
                    this.RunComplexAnimation(param);
                    break;
            }

            base.OnNavigatedTo(e);
        }

        private void RunComplexAnimation(PDPNavigation param)
        {
            var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            var collectionStart = "A.Size.y + this.StartingValue";
            var collectionEnd = "this.StartingValue";

            // Transform from the bottom
            var listTranslateIn = compositor.CreateScalarKeyFrameAnimation();
            listTranslateIn.Duration = CollectionPage.animationDuration;
            listTranslateIn.SetReferenceParameter("A", ElementCompositionPreview.GetElementVisual(this.PDPList));
            listTranslateIn.InsertExpressionKeyFrame(0.0f, collectionStart);
            listTranslateIn.InsertExpressionKeyFrame(1.0f, collectionEnd);
            listTranslateIn.Target = "Offset.Y";

            // Opacity
            var listFadeIn = compositor.CreateScalarKeyFrameAnimation();
            listFadeIn.Duration = CollectionPage.animationDuration;
            listFadeIn.Target = "Opacity";
            listFadeIn.InsertKeyFrame(0.0f, 0.0f);
            listFadeIn.InsertKeyFrame(0.5f, 0.0f);
            listFadeIn.InsertKeyFrame(1.0f, 1.0f);

            var listEntrance = compositor.CreateAnimationGroup();
            listEntrance.Add(listTranslateIn);
            listEntrance.Add(listFadeIn);

            ElementCompositionPreview.SetImplicitShowAnimation(this.PDPList, listEntrance);

            // Backdrop
            var backdropEntrance = compositor.CreateAnimationGroup();

            // translate
            var backdropTranslateY = compositor.CreateScalarKeyFrameAnimation();
            backdropTranslateY.Duration = CollectionPage.animationDuration;
            backdropTranslateY.SetReferenceParameter("A", ElementCompositionPreview.GetElementVisual(this.BackdropContainer));
            backdropTranslateY.SetScalarParameter("beginY", (float)param.Position.Y);
            backdropTranslateY.InsertExpressionKeyFrame(0.0f, "beginY");
            backdropTranslateY.InsertExpressionKeyFrame(1.0f, "this.StartingValue");
            backdropTranslateY.Target = "Offset.Y";

            var backdropTranslateX = compositor.CreateScalarKeyFrameAnimation();
            backdropTranslateX.Duration = CollectionPage.animationDuration;
            backdropTranslateX.SetReferenceParameter("A", ElementCompositionPreview.GetElementVisual(this.BackdropContainer));
            backdropTranslateX.SetScalarParameter("beginX", (float)param.Position.X);
            backdropTranslateX.InsertExpressionKeyFrame(0.0f, "beginX");
            backdropTranslateX.InsertExpressionKeyFrame(1.0f, "this.StartingValue");
            backdropTranslateX.Target = "Offset.X";

            // Scale
            var backdropScale = compositor.CreateVector3KeyFrameAnimation();

            backdropScale.SetReferenceParameter("A", ElementCompositionPreview.GetElementVisual(this.BackdropContainer));
            backdropScale.InsertExpressionKeyFrame(0.0f, "Vector3(202/A.Size.X, 202 / A.Size.Y, 0.0)");
            backdropScale.InsertKeyFrame(1.0f, new Vector3(1.0f));
            backdropScale.Target = nameof(Visual.Scale);
            backdropScale.Duration = CollectionPage.animationDuration;

            backdropEntrance.Add(backdropTranslateY);
            backdropEntrance.Add(backdropTranslateX);
            backdropEntrance.Add(backdropScale);

            ElementCompositionPreview.SetImplicitShowAnimation(this.BackdropContainer, backdropEntrance);

            // Clip
            var backdropBottomClip = compositor.CreateScalarKeyFrameAnimation();
            var backdropVisual = ElementCompositionPreview.GetElementVisual(this.BackdropContainer);
            backdropBottomClip.Target = "BottomInset";
            var clippy = compositor.CreateInsetClip();
            backdropVisual.Clip = clippy;
            backdropBottomClip.Duration = CollectionPage.animationDuration;
            backdropBottomClip.SetReferenceParameter("A", backdropVisual);
            backdropBottomClip.InsertExpressionKeyFrame(0.0f, "0");
            backdropBottomClip.InsertExpressionKeyFrame(1.0f, "(A.Size.Y - 250)");
            clippy.StartAnimation("BottomInset", backdropBottomClip);

            // Image
            var imageEntrance = compositor.CreateAnimationGroup();

            // Opacity
            var imageOpacity = compositor.CreateScalarKeyFrameAnimation();
            imageOpacity.Target = "Opacity";
            imageOpacity.Duration = CollectionPage.animationDuration;
            imageOpacity.InsertKeyFrame(0.0f, 0.0f);
            imageOpacity.InsertKeyFrame(0.5f, 0.0f);
            imageOpacity.InsertKeyFrame(1.0f, 1.0f);

            // Transform
            var imageTranslate = compositor.CreateVector3KeyFrameAnimation();
            imageTranslate.Duration = CollectionPage.animationDuration;
            //imageTranslate.SetReferenceParameter("A", ElementCompositionPreview.GetElementVisual(this.BackdropContainer));
            imageTranslate.InsertExpressionKeyFrame(0.0f, "Vector3(500, 500, -this.StartingValue.Z)");
            imageTranslate.InsertExpressionKeyFrame(1.0f, "this.StartingValue");
            imageTranslate.Target = "Offset";

            imageEntrance.Add(imageOpacity);
            imageEntrance.Add(imageTranslate);

            ElementCompositionPreview.SetImplicitShowAnimation(this.Animal, imageEntrance);

            // Text
            var textOpacity = compositor.CreateScalarKeyFrameAnimation();
            textOpacity.Target = "Opacity";
            textOpacity.Duration = CollectionPage.animationDuration;
            textOpacity.InsertKeyFrame(0.0f, 0.0f);
            textOpacity.InsertKeyFrame(0.6f, 0.0f);
            textOpacity.InsertKeyFrame(1.0f, 1.0f);

            ElementCompositionPreview.SetImplicitShowAnimation(this.TextContainer, textOpacity);
        }
    }
}
