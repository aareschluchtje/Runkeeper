using System;
using System.Collections.Generic;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Runkeeper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
         
            RouteScreen.IsSelected = true;
            Myframe.Navigate(typeof(MapPage));
            PageTitle.Text = "Route";
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            RunView.IsPaneOpen = !RunView.IsPaneOpen;
        }

        private void RunList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RouteScreen.IsSelected)
            {
                Myframe.Navigate(typeof(MapPage));
                PageTitle.Text = "Route";
            }
            if (Grafiek.IsSelected)
            {
                Myframe.Navigate(typeof(DataPage));
                PageTitle.Text = "graphics";
            }
            if (Groups.IsSelected)
            {
                Myframe.Navigate(typeof(GroupsPage));
                PageTitle.Text = "add-Groups";
            }
            if (Settings.IsSelected)
            {
                Myframe.Navigate(typeof(SettingsPage));
                PageTitle.Text = "Settings";
            }
            if (Help.IsSelected)
            {
                Myframe.Navigate(typeof(HelpPage));
                PageTitle.Text = "Help";
            }
        }

        private void MySplitviewPane_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {

        }

    
        private void Grid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {

        }
    }
}
