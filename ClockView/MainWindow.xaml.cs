using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace ClockView
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Timer timer, timerCD;
        private DateTime CDtimeBegin;

        public MainWindow()
        {
            InitializeComponent();
            timer = new Timer();
            timerCD = new Timer();
            timerCD.Interval = 1000;
            timerCD.Enabled = true;
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            //DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            ////dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            //dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            //dispatcherTimer.Start();

            //var keys = Observable.FromEventPattern<EventArgs>(dispatcherTimer, "Tick");
            //keys.Subscribe(x => label1.Content = DateTime.Now.Second);

            #region Timer

            timer.Interval = 1000;
            timer.Enabled = true;
            //timer.Elapsed+=new ElapsedEventHandler(timer_Elapsed);

            int i = 0;
            DateTime dateTime = DateTime.Now;
            //var keys = Observable.FromEventPattern<ElapsedEventArgs>(timer, "Elapsed").Throttle(new TimeSpan(0, 0, 1));
            var keys = Observable.FromEventPattern<ElapsedEventArgs>(timer, "Elapsed");

            timer.Start();

            IDisposable subscription = keys.ObserveOnDispatcher().Subscribe(x =>
            {
                //label1.Content = i.ToString();
                //i++;

                TimeSpan timeSpan = DateTime.Now.Subtract(dateTime) - pauseTime;
                //TimeSpan timeSpan = x.EventArgs.SignalTime.Subtract(dateTime);

                //label1.Content = string.Format("{0:#}", timeSpan.TotalSeconds);
                label1.Content = string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                //if (timeSpan.Seconds > 12)
                //{
                //    timer.Stop();
                //    label1.Content = "0";
                //}
                if (pauseEnable)
                {
                    timer.Stop();
                    pauseBeginTime = DateTime.Now;
                }
            });
            #endregion
        }


        //keys.oberverDispatcher
        //    IObservable<ElapsedEventArgs> observable= Observable.FromEventPattern<ElapsedEventArgs>(timer, "Elapsed");
        //ObserveOn().Subscribe()
        //    Scheduler

        //IObservable<long> observable = Observable.Interval(new TimeSpan(0, 0, 10));
        //observable.ObserveOnDispatcher(). 

        private bool pauseEnable = false;

        private DateTime pauseBeginTime = new DateTime();

        private TimeSpan pauseTime = new TimeSpan();


        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            pauseEnable = !pauseEnable;
            if (pauseEnable)
            {
                //timer.Stop();
                //pauseBeginTime = DateTime.Now;
            }
            else
            {
                timer.Start();
                //CDtimeBegin = DateTime.Now;
                pauseTime = pauseTime + DateTime.Now.Subtract(pauseBeginTime);
            }

        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {

            pauseTime = TimeSpan.Zero;
            label1.Content = "00:00:00";
            pauseEnable = false;
            timer.Close();
        }



        private bool CDpauseEnable = false;
        private IDisposable iDisposable;
        private DateTime CDpauseBeginTime;
        /// <summary>
        /// 倒计时长度
        /// </summary>
        private TimeSpan timeSpanCD;

        private void ReRun_Click(object sender, RoutedEventArgs e)
        {
            
            #region TimerCD
            BuildTimeSpanCD();
            var keys = Observable.FromEventPattern<ElapsedEventArgs>(timerCD, "Elapsed");
            CDpauseEnable = false;
            timerCD.Start();
            if (iDisposable != null)
            {
                iDisposable.Dispose();
            }
            iDisposable = keys.ObserveOnDispatcher().Subscribe(x =>
            {
                timeSpanCD = timeSpanCD.Subtract(new TimeSpan(0, 0, 0, 1));
                label2.Content = string.Format("{0:00}:{1:00}:{2:00}",
                     timeSpanCD.Hours, timeSpanCD.Minutes, timeSpanCD.Seconds);

                if (timeSpanCD <= TimeSpan.Zero)
                {
                    iDisposable.Dispose();
                    label2.Content = "00:00:00";
                    MessageBox.Show("时间到！");
                   
                }

            });
        }
            #endregion

        private void RePause_Click(object sender, RoutedEventArgs e)
        {
            CDpauseEnable = !CDpauseEnable;
            if (CDpauseEnable == false)
            {
                timerCD.Start();
            }
            else
            {
                timerCD.Stop();
                //iDisposable.Dispose();

            }
        }

        private void ReStop_Click(object sender, RoutedEventArgs e)
        {
            label2.Content = "00:00:00";

            //timerCD.Stop();
            //BuildTimeSpanCD();
            //CDpauseEnable = false;
            iDisposable.Dispose();
        }

        private void countDown_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (countDown.Text != "")
            {
                BuildTimeSpanCD();
            }
        }

        private void BuildTimeSpanCD()
        {
            timeSpanCD = new TimeSpan(0, 0, int.Parse(countDown.Text), 0);
            label2.Content = string.Format("{0:00}:{1:00}:{2:00}", timeSpanCD.Hours, timeSpanCD.Minutes, timeSpanCD.Seconds);
        }
    }
}
