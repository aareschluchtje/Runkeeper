using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;


namespace Runkeeper.ViewModel
{
    public class Time : INotifyPropertyChanged
    {
        private DispatcherTimer timer;
        public string stopwatchTime { get; set; }
        private double startTime;
        public Time()
        {
            startTime = Now;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += TimerOnTick;
            stopwatchTime = "0:0:0:0";
        }

        public static double Now { get { return (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds; } }

        public static string MillisecondsToTime(double millis)
        {
            string timestr = "";
            int millesec = (int)millis%1000;
            millis /= 1000;
            int seconde = (int) millis%60;
            millis /= 60;
            int minuten = (int) millis%60;
            millis /= 60;
            int uren = (int)millis;
            timestr = uren+ ":" + minuten + ":" + seconde + ":" + millesec; 
            return timestr;
        }

        private void TimerOnTick(object sender, object o)
        {
            stopwatchTime = MillisecondsToTime(Now - startTime);
            NotifyPropertyChanged(nameof(stopwatchTime));
        }

        public void Start()
        {
            stopwatchTime = "0:0:0:0";
            startTime = Now;
            timer.Start();
            NotifyPropertyChanged(nameof(stopwatchTime));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Stop()
        {
            timer.Stop();
        }
    }
}
