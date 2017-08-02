using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page3 : Page
    {
        public Page3()
        {
            this.InitializeComponent();

            var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            var showAnimation = compositor.CreateVector3KeyFrameAnimation();
            showAnimation.InsertKeyFrame(0.0f, new Vector3(0.0f, 1000.0f, 0.0f));
            showAnimation.InsertKeyFrame(1.0f, new Vector3(0.0f, 0.0f, 0.0f));
            showAnimation.Target = nameof(Visual.Offset);
            showAnimation.Duration = TimeSpan.FromSeconds(2.5f);

            ElementCompositionPreview.SetImplicitShowAnimation(this, showAnimation);
        }
    }
}
