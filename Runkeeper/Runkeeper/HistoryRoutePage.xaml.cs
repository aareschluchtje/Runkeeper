using Runkeeper.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Runkeeper.Annotations;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Runkeeper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HistoryRoutePage : Page, INotifyPropertyChanged
    {
        public HistoryRoutePage()
        {
            this.InitializeComponent();
            this.DataContext = App.instance.transfer.data;
        }

        public void orderbyDistance()
        {
            var order = from route in App.instance.transfer.data.walkedRoutes
                        orderby route.totalDistance descending
                        select route;
            App.instance.transfer.data.walkedRoutes = new ObservableCollection<Route>(order);
            NotifyPropertyChanged(nameof(App.instance.transfer.data.walkedRoutes));
            data.ItemsSource = App.instance.transfer.data.walkedRoutes;
            PrintRoute<Route>("distance order: ", App.instance.transfer.data.walkedRoutes);
            data.DataContext = App.instance.transfer.data;
        }

        public void orderbyTime()
        { 
            var order = from route in App.instance.transfer.data.walkedRoutes
                        orderby route.date.Second descending
                        select route;
            App.instance.transfer.data.walkedRoutes = new ObservableCollection<Route>(order);
            NotifyPropertyChanged(nameof(App.instance.transfer.data.walkedRoutes));
            data.ItemsSource = App.instance.transfer.data.walkedRoutes;
            PrintRoute<Route>("distance order: ", App.instance.transfer.data.walkedRoutes);
            data.DataContext = App.instance.transfer.data;
        }

        static void PrintRoute<T>(string title, IEnumerable<T> Routes)
        {
            Debug.WriteLine("{0}:", title);
            foreach (T routes in Routes)
            {
                Debug.WriteLine(routes.ToString());
            }
        }

        private void SortDistance_OnClick(object sender, RoutedEventArgs e)
        {
            orderbyDistance();
        }

        private void SortDate_OnClick(object sender, RoutedEventArgs e)
        {
            orderbyTime();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
