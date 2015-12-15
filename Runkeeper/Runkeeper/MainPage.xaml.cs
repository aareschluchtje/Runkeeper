using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
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
        Frame frame;
        public MainPage(Frame frame)
        {
            this.InitializeComponent();
            this.frame = frame;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

            SystemNavigationManager.GetForCurrentView().BackRequested += (s, e) =>
            {
                if (frame.CanGoBack)
                {
                    frame.GoBack();
                    e.Handled = true;
                    RouteScreen.IsSelected = true;
                }
            };
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            RunView.IsPaneOpen = !RunView.IsPaneOpen;
        }

        private void RunList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RouteScreen.IsSelected)
            {
                frame.Navigate(typeof(MapPage));
                PageTitle.Text = "Route";
            }
            if (Grafiek.IsSelected)
            {
                frame.Navigate(typeof(DataPage));
                PageTitle.Text = "graphics";
            }
            if (Groups.IsSelected)
            {
                frame.Navigate(typeof(GroupsPage));
                PageTitle.Text = "add-Groups";
            }
            if (Settings.IsSelected)
            {
                frame.Navigate(typeof(SettingsPage));
                PageTitle.Text = "Settings";
            }
            if (Help.IsSelected)
            {
                frame.Navigate(typeof(HelpPage));
                PageTitle.Text = "Help";
            }
        }

        private void MySplitviewPane_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (e.Cumulative.Translation.X < -50)
            {
                RunView.IsPaneOpen = false;
            }
        }

    
        private void Grid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (e.Cumulative.Translation.X > 50)
            {
                RunView.IsPaneOpen = true;
            }
        }
    }
}
