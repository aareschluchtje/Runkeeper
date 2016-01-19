using Runkeeper.ViewModel;
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
    public sealed partial class RunningPage : Page
    {
        Time time;
        public RunningPage()
        {
            this.InitializeComponent();
            time = new Time();
            this.DataContext = time;
            // lol.Text = time.stopwatch.Elapsed.Seconds.ToString();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
                time.timer.Stop();
            time.ResetStopWatch();
        }

        private void START_Click(object sender, RoutedEventArgs e)
        {
            time.timer.Start();
        }
    }
}
