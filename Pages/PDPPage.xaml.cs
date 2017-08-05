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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ImplicitAnimations.Pages
{
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
        }

        private void PDPList_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            args.ItemContainer.Background = (((int)args.Item) % 2 == 0) ? new SolidColorBrush(Colors.LightGray) : new SolidColorBrush(Colors.White);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string url = e.Parameter as String ?? "https://placeimg.com/202/202/animals";

            var source = new BitmapImage(new Uri(url));
            this.Animal.Source = source;
            this.BackDrop.Source = source;
            base.OnNavigatedTo(e);
        }
    }
}
