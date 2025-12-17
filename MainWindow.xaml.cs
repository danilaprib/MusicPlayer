using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        int CURRENT_TRACK_INDEX;

        string _currentPosition;

        public string CurrentPosition
        {
            get => _currentPosition;
            set
            {
                _currentPosition = value;
                PropertyChanged?.Invoke(this, null);
            }
        }

        string[] tracks;

        public event PropertyChangedEventHandler? PropertyChanged;

        bool isLooped;

        //System.Timers.Timer _timer;

        public MainWindow()
        {
            InitializeComponent();
            isLooped = false;
            CURRENT_TRACK_INDEX = 0;

            this.Dir_LostFocus(this, null);



            //using (_timer = new System.Timers.Timer(1000));
            //_timer.AutoReset = true;
            //_timer.Enabled = true;

        }

        private void LoopBtn_Click(object sender, RoutedEventArgs e)
        {
            isLooped = !isLooped;
            if (isLooped)
            {
                LoopBtn.Content = "loop ON";
            }
            else
            {
                LoopBtn.Content = "loop OFF";
            }
            Debug.WriteLine("LOOP button pressed");
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            player.Stop();
            Status.Visibility = Visibility.Visible;
            Status.Content = "(Stopped)";

            PauseBtn.IsEnabled = false;
        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            player.Play();
            Status.Visibility = Visibility.Hidden;
            Status.Content = string.Empty;


            PauseBtn.IsEnabled = true;
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            player.Pause();
            Status.Visibility = Visibility.Visible;
            Status.Content = "(Paused)";

            var pos = player.Position.ToString(@"hh\:mm\:ss");
            Debug.WriteLine($"position:  {pos}");
        }


        private void Dir_LostFocus(object sender, RoutedEventArgs e)
        {
            var dir = Dir.Text;
            if (!Directory.Exists(dir))
            {
                return;
            }
            Tracklist.Children.Clear();
            tracks = Directory.GetFiles(dir, "*.mp3");

            for(int i = 0; i < tracks.Length; i++)
            {
                DockPanel panel = new DockPanel();

                TextBlock index = new TextBlock();
                index.FontSize = 18;
                index.Text = i.ToString();
                index.VerticalAlignment = VerticalAlignment.Center;
                index.FontStyle = FontStyles.Italic;

                Button button = new Button();
                var style = TryFindResource("TrackNameStyle");
                if (style != null)
                {
                    button.Style = (Style)style;
                }
                    
                button.Click += TrackBtn_Click;

                string trackName = tracks[i].Remove(0, dir.Length + 1);
                button.Content = trackName;

                DockPanel.SetDock(button, Dock.Left);
                panel.Children.Add(button);


                DockPanel.SetDock(button, Dock.Right);
                panel.Children.Add(index);

                Tracklist.Children.Add(panel);
            }
        }

        private void TrackBtn_Click(Object sender, RoutedEventArgs e)
        {
            Status.Content = string.Empty;
            PauseBtn.IsEnabled = true;

            var btn = (Button)sender;
            var trackname = btn.Content as string;
            
            var fullPath = System.IO.Path.Combine(Dir.Text, trackname);
            CURRENT_TRACK_INDEX = Tracklist.Children.IndexOf(btn); // because btn is inside dock panel it returns -1
            // better use 2 separate stack panels (1 for indexes 1 for track names) and dock these stackpanels to a dockpanel
            if (trackname != null)
            {
                CurrentTrack.Text = trackname;
                player.Stop();
                player.Source = new Uri(fullPath);
                player.Play();
            }
            //Duration.Text = player.NaturalDuration.ToString();
            //Position.Text = player.Position.ToString();

            //var timer = new System.Timers.Timer(1000);
            //timer.
            //timer.Elapsed += OnTimedEvent;
            //timer.AutoReset = true;
            //timer.Enabled = true;

            //_timer = timer;
            //_timer.Enabled = true;
            //_timer.Start();


            //posThread.Start();


            //Debug.WriteLine("timer: " + _timer.);
        }



        //void OnTimedEvent(Object sender, ElapsedEventArgs e)
        //{
        //    CurrentPosition = player.Position.ToString(@"hh\:mm\:ss");
        //    Debug.WriteLine("position:  " + player.Position.ToString(@"hh\:mm\:ss"));
        //    Debug.WriteLine("timer: " + e.SignalTime);
        //}

        private void ReplayBtn_Click(object sender, RoutedEventArgs e)
        {
            Status.Content = string.Empty;
            PauseBtn.IsEnabled = true;
            player.Stop();
            player.Play();
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (CURRENT_TRACK_INDEX == 0)
            {
                return;
            }
            CURRENT_TRACK_INDEX--;

            var track = (DockPanel)Tracklist.Children[CURRENT_TRACK_INDEX];

            var btn = (Button)track.Children[0];
            var trackname = btn.Content as string;
            var fullPath = System.IO.Path.Combine(Dir.Text, trackname);

            if (trackname != null)
            {
                CurrentTrack.Text = trackname;
                player.Stop();
                player.Source = new Uri(fullPath);
                player.Play();
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (CURRENT_TRACK_INDEX == Tracklist.Children.Count)
            {
                return;
            }
            CURRENT_TRACK_INDEX++;

            var track = (DockPanel)Tracklist.Children[CURRENT_TRACK_INDEX];

            var btn = (Button)track.Children[0];
            var trackname = btn.Content as string;
            var fullPath = System.IO.Path.Combine(Dir.Text, trackname);

            if (trackname != null)
            {
                CurrentTrack.Text = trackname;
                player.Stop();
                player.Source = new Uri(fullPath);
                player.Play();
            }
        }

        private void player_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (isLooped)
            {
                player.Stop();
                player.Play();
            }
            else
            {
                this.Next_Click(this, null);
            }
        }

        private void player_MediaOpened(object sender, RoutedEventArgs e)
        {
            string duration = player.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss");
            Duration.Text = duration;
        }

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var volSlider = sender as Slider;
            var vol = (volSlider.Value);

            player.Volume = vol;
            //CurrentVolume.Text = (vol * 100).ToString();
            //var vol = 
            //string msg = String.Format("Current value: {0}", e.NewValue);
            //this.textBlock1.Text = msg;
        }

        private void Balance_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var balanceSlider = sender as Slider;

            var vol = (balanceSlider.Value);

            player.Balance = vol;
        }

        private void ResetBalanceBtn_Click(object sender, RoutedEventArgs e)
        {
            Balance.Value = 0;
        }

        private void MuteBtn_Click(object sender, RoutedEventArgs e)
        {
            player.IsMuted = !player.IsMuted;
            if (player.IsMuted)
            {
                Color color = Color.FromRgb(255, 0, 0);
                Volume.Background = new SolidColorBrush(color);
            }
            else
            {
                Color color = Color.FromRgb(0, 0, 255);
                Volume.Background = new SolidColorBrush(color);
            }
        }

        private void Speed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var balanceSlider = sender as Slider;

            var vol = balanceSlider.Value;

            //player.Pause();
            player.SpeedRatio = vol;
            //player.Play();
        }
        private void ResetSpeedBtn_Click(object sender, RoutedEventArgs e)
        {
            Speed.Value = 1;
        }
    }
}