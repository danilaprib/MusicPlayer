using Microsoft.Win32;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks.Dataflow;
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
//using System.Windows.Shapes.Path;

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        int CURRENT_TRACK_INDEX = 0;

        System.Timers.Timer _timer;
        DateTime _timerStartedAt;

        string _currentPosition;

        public string CurrentPosition
        {
            get { return _currentPosition; } 
            set
            {
                _currentPosition = value;
                OnPropertyChanged();
            }
        }

        string[] tracks;

        bool isLooped;
        bool isPaused = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string val = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(val));
            //Position.Text = CurrentPosition;
        }

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            isLooped = false;
            CURRENT_TRACK_INDEX = 0;

            this.Dir_LostFocus(this, null);

        }

        private void LoopBtn_Click(object sender, RoutedEventArgs e)
        {
            isLooped = !isLooped;
            if (isLooped)
            {
                LoopBtn.Content = "loop ON";
                LoopBtn.Background = new SolidColorBrush(Colors.Blue);
            }
            else
            {
                LoopBtn.Content = "loop OFF";
                LoopBtn.Background = new SolidColorBrush(Colors.Red);
            }
        }



        // app crashes after pressing stop and after that pressing play/pause
        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            player.Stop();
            Status.Visibility = Visibility.Visible;
            Status.Content = "(Stopped)";
            isPaused = true;


            _timer.Stop();
            //_timer.Dispose();
        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            player.Play();
            Status.Visibility = Visibility.Hidden;
            Status.Content = string.Empty;
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            player.Pause();
            isPaused = true;
            Status.Visibility = Visibility.Visible;
            Status.Content = "(Paused)";
        }


        private void Dir_LostFocus(object sender, RoutedEventArgs e)
        {
            CURRENT_TRACK_INDEX = 0;

            var dir = Dir.Text;
            if (!Directory.Exists(dir))
            {
                return;
            }
            TrackNames.Children.Clear();
            TrackIndexes.Children.Clear();

            tracks = Directory.GetFiles(dir, "*.mp3");



            for(int i = 0; i < tracks.Length; i++)
            {
                Button btn = new Button();
                var style = TryFindResource("TrackNameStyle");
                if (style != null)
                {
                    btn.Style = (Style)style;
                }
                btn.Click += TrackBtn_Click;

                string trackName = tracks[i].Remove(0, dir.Length + 1);
                btn.Content = trackName;

                TrackNames.Children.Add(btn);

                Label index = new Label();

                index.FontSize = 18;
                var temp = i + 1;
                index.Content = temp.ToString();
                index.VerticalAlignment = VerticalAlignment.Center;
                index.FontStyle = FontStyles.Italic;

                TrackIndexes.Children.Add(index);
            }
        }

        private void TrackBtn_Click(Object sender, RoutedEventArgs e)
        {
            _timer = new System.Timers.Timer();
            _timer.Interval = 1000;
            _timer.AutoReset = true;
            _timer.Elapsed += timer_Elapsed;

            Status.Content = string.Empty;

            var btn = (Button)sender;
            var trackname = btn.Content as string;
            
            var fullPath = System.IO.Path.Combine(Dir.Text, trackname);


            CURRENT_TRACK_INDEX = TrackNames.Children.IndexOf(btn);

            Debug.WriteLine("CURRENT_TRACK_INDEX: " + CURRENT_TRACK_INDEX);


            if (trackname != null)
            {
                CurrentTrack.Text = trackname;
                player.Stop();
                player.Source = new Uri(fullPath);
                player.Play();
            }


            _timer.Start();
            _timerStartedAt = DateTime.Now;
        }

        private void timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            var curPos = e.SignalTime - _timerStartedAt;
            CurrentPosition = curPos.ToString(@"hh\:mm\:ss");
            Debug.WriteLine("POSITION: " + CurrentPosition);
        }

        private void ReplayBtn_Click(object sender, RoutedEventArgs e)
        {
            Status.Content = string.Empty;
            player.Stop();
            player.Play();
        }

        private void PrevBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CURRENT_TRACK_INDEX == 0)
            {
                // to jump to end
                CURRENT_TRACK_INDEX = TrackIndexes.Children.Count;
            }
            CURRENT_TRACK_INDEX--;


            //var track = (DockPanel)Tracklist.Children[CURRENT_TRACK_INDEX];

            var btn = (Button)TrackNames.Children[CURRENT_TRACK_INDEX];
            var trackname = btn.Content as string;
            var fullPath = System.IO.Path.Combine(Dir.Text, trackname);

            if (trackname != null)
            {
                CurrentTrack.Text = trackname;
                player.Stop();
                player.Source = new Uri(fullPath);
                player.Play();
                PlayPauseBtn.Background = new SolidColorBrush(Colors.Blue);
            }
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CURRENT_TRACK_INDEX == TrackIndexes.Children.Count - 1)
            {
                // to jump to beginning
                CURRENT_TRACK_INDEX = -1;
            }
            CURRENT_TRACK_INDEX++;

            var btn = (Button)TrackNames.Children[CURRENT_TRACK_INDEX];
            var trackname = btn.Content as string;
            var fullPath = System.IO.Path.Combine(Dir.Text, trackname);

            if (trackname != null)
            {
                CurrentTrack.Text = trackname;
                player.Stop();
                player.Source = new Uri(fullPath);
                player.Play();
                PlayPauseBtn.Background = new SolidColorBrush(Colors.Blue);
            }
        }

        private void player_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (isLooped)
            {
                _timer = new System.Timers.Timer();
                _timer.Interval = 1000;
                _timer.AutoReset = true;
                _timer.Elapsed += timer_Elapsed;

                player.Stop();
                player.Play();

                _timer.Start();
                _timerStartedAt = DateTime.Now;
            }
            else
            {
                this.NextBtn_Click(this, null);
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
                Volume.Background = new SolidColorBrush(Colors.Red);
                MuteBtn.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Volume.Background = new SolidColorBrush(Colors.Blue);
                MuteBtn.Background = new SolidColorBrush(Colors.Blue);
            }
        }

        private void Speed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var balanceSlider = sender as Slider;

            var vol = balanceSlider.Value;

            player.SpeedRatio = vol;
        }
        private void ResetSpeedBtn_Click(object sender, RoutedEventArgs e)
        {
            Speed.Value = 1;
        }

        private void PlayPauseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (player.CanPause && !isPaused)
            {
                player.Pause();
                _timer.Stop();
                isPaused = !isPaused;
                PlayPauseBtn.Background = new SolidColorBrush(Colors.Red);
            }
            else if (isPaused)
            {
                player.Play();
                _timer.Start();
                isPaused = !isPaused;
                Status.Content = string.Empty;
                PlayPauseBtn.Background = new SolidColorBrush(Colors.Blue);
            }
        }

        private void AboutItem_Click(object sender, RoutedEventArgs e)
        {
            //var aboutWindow = new Window();
            //aboutWindow.Close?
            var message = "You can press Enter to close this message box.\nSpecify directory in the Path textbox and press anywhere to lose the focus from the textbox. This will load all .mp3 files from that directory\nPress on tracknames to select a track and start " +
                "playing it.\n\nDaniil Prybyshchuk 2025";
            var result = MessageBox.Show(message, "About", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.None);
        }

        private void OpenFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var curDir = Dir.Text;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select file to open...";
            openFileDialog.Filter = "Audio file(s) | *.mp3";
            openFileDialog.DefaultDirectory = curDir;
            openFileDialog.InitialDirectory = curDir;
            var result = openFileDialog.ShowDialog();

            if (result != true)
                return;

            var selectedFilename = openFileDialog.FileName;
            string? trackName = System.IO.Path.GetFileName(selectedFilename);

            var dir = Directory.GetParent(selectedFilename);
            Dir.Text = dir.ToString();
            Dir_LostFocus(this, null);


            // TODO: should play the selected track

            //for (int i = 0; i < tracks.Length; i++)
            //{
            //    if (trackName == tracks[i].Name)
            //    {
            //        player.Play();
            //    }

            //}
        }

        private void MoveToPrevDirBtn_Click(object sender, RoutedEventArgs e)
        {
            string dir = Dir.Text;
            var prevDir = Directory.GetParent(dir);

            if (prevDir != null)
            {
                Dir.Text = prevDir.ToString();
                Dir_LostFocus(this, null);
            }
        }


        //MediaCommands.IncreaseBass
    }
}