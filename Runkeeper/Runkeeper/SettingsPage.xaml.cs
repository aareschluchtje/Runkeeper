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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Runkeeper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            SetOn.IsOn = App.instance.transfer.data.zoomCenter;
            SetOld.IsOn = App.instance.transfer.data.drawOld;
        }

        private void ToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
        {
            if (SetOn.IsOn)
            {
                App.instance.transfer.data.setZoomCenter(true);
            }
            else
            {
                App.instance.transfer.data.setZoomCenter(false);
            }
        }

        private void SetOld_Toggled(object sender, RoutedEventArgs e)
        {
            if (SetOld.IsOn)
            {
                App.instance.transfer.data.setDrawOld(true);
            }
            else
            {
                App.instance.transfer.data.setDrawOld(false);
            }
        }
    }
}
