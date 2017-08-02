using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
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
    public sealed partial class Page1 : Page
    {
        public Page1()
        {
            this.InitializeComponent();
        }

        private void AddItem(object sender, RoutedEventArgs e)
        {
            Rectangle rect = new Rectangle();
            rect.Width = rect.Height = 300;
            rect.Fill = new SolidColorBrush(Colors.Orange);

            this.ContentContainer.Children.Add(rect);
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
