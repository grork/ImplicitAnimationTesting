# ImplicitAnimationTesting
A sample project showing some *moderately* complex [Composition Animations](https://docs.microsoft.com/en-us/windows/uwp/composition/using-the-visual-layer-with-xaml ""). These are straight to the platform API, and not using the Community Toolkit [Wrapper](https://docs.microsoft.com/en-us/windows/communitytoolkit/animations/implicitanimations ""), so are relatively low level and verbose.

## Why a sample project?
At the time of this project was authored, there was very little documentation or samples for composition animations. Even more rare were animations that helped perform *page* transitions. At the time composition animations were introduced, connected animations were also delivered they didn’t support complex connected animations only a [BasicConnectedAnimationConfiguration](https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.media.animation.basicconnectedanimationconfiguration?view=winrt-22000 ""), which didn’t allow for control or coordination with other animations.

At the time my $job was looking to employ these technologies, so I spent some time creating an approximation.

## Why share it?
It was a learning experience, and in my (light) research, still seems like it might be useful to someone since the community toolkit hides some of the details. The XAML Gallery [example](https://github.com/microsoft/Xaml-Controls-Gallery/blob/master/XamlControlsGallery/ConnectedAnimationPages/CollectionPage.xaml.cs "") is dependent on using the build in connected animations – which now get close to the desired effect – but if you need something more complex, this may help in that understanding.

## Running
You need:
- Visual Studio 2019 or higher
- Windows 10 2004, 19041 or higher
- Animations turned on in Windows Settings

Open it, and go!

## Contents
There is a left navigation containing 3 pages — two collection pages, and a test page. Additionally navigation from the collection page (clicking on a tile) will navigate to a product details page.

### Animations
- Collection-to-Collection translates up/down (Depending on the direction), as well as a staggered animation of the header
- Collection-to-Product Page will animate the image of the tile to top-left *from it’s originating position*, and also scale up a backdrop behind the the header while performing a clip

https://user-images.githubusercontent.com/1811056/162288618-940a6548-a7be-4d8d-bfdf-e1afca4e5613.mp4

### Notes
- Holding Control when causing an animation to start will slow the animation down 10x. The default length is 0.5s, so will be extended to 5s
- To ensure that the page transitions play it is *required* that you pass a `new SuppressNavigationTransiationInfo()` to the `Frame.Navigate` method. Without this, animations will stall/run for the wrong duration/just won’t work
