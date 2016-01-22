using Runkeeper.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Runkeeper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HistoryRoutePage : Page
    {
        public HistoryRoutePage()
        {
            this.InitializeComponent();
            data.DataContext = App.instance.transfer.data;
            orderbyDistance();
        }

        public void orderbyDistance()
        {
            var allBooks = from Route in App.instance.transfer.data.walkedRoutes select Route;
            var order = from Route in App.instance.transfer.data.walkedRoutes
                orderby Route.totalDistance descending
                select Route;
            PrintRoute("distance order: ", allBooks);

        }

        static void PrintRoute<Route>(string title, IEnumerable<Route> Routes)
        {
            Debug.WriteLine("{0}:", title);
            foreach (Route routes in Routes)
            {
                Debug.WriteLine(routes.ToString());
            }
        }
    }
}
