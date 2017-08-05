using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
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
        public CollectionPage()
        {
            this.InitializeComponent();

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
    }
}
