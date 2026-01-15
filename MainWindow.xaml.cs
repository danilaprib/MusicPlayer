using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

//using System.Windows.Shapes.Path;

namespace MusicPlayer
{

    // left-right arrow keys move 5 sec forward and back the track (you can specify the number)
    // up-down keys bring the volume up and down

    // IT CRASHES WHEN REACHING END OF QUEUE


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // its better to have OnPropertyChanged for current index and just execute different methods 
        // when index changes
        // just button methods change the index
        // and one method that loads the media for the current index

        int CURRENT_INDEX = 0;

        bool IS_PAUSED = false;

        System.Timers.Timer TIMER_POSITION;

        int POSITION = 0; // this is for seeker

        public MainWindow()
        {
            InitializeComponent();

            ScanBtn_Click(this, null);
        }


        private void ScanBtn_Click(object sender, RoutedEventArgs e)
        {
            lvTracklist.Items.Clear();
            var dir = Dir.Text;
            if (Directory.Exists(dir))
            {
                var files = Directory.GetFiles(dir, "*mp3");

                for (int i = 0; i < files.Length; i++)
                {
                    Player.Source = new Uri(files[i]);
                    var trackName = System.IO.Path.GetFileName(files[i]);

                    lvTracklist.Items.Add(new TrackItem((i + 1), trackName, "0:00:00"));
                }
            }

            lvTracklist.SelectedItem = null;
        }

        private void lvTracklist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvTracklist.SelectedItem != null)
            {
                var trackItem = (TrackItem)lvTracklist.SelectedItem;
                CURRENT_INDEX = trackItem.TrackIndex - 1;
                var fullPath = Path.Combine(Dir.Text, trackItem.TrackName);
                Player.Source = new Uri(fullPath);

                TIMER_POSITION = new System.Timers.Timer();
                TIMER_POSITION.AutoReset = true;
                TIMER_POSITION.Interval = 1000;
                TIMER_POSITION.Enabled = false;
                TIMER_POSITION.Elapsed += OnTimerPositionElapsed(TIMER_POSITION, null);
                TIMER_POSITION.Start();

                Player.Play();
            }
            lvTracklist.SelectedItem = null;
        }

        private ElapsedEventHandler OnTimerPositionElapsed(object sender, EventArgs e)
        {
            Debug.WriteLine("Seeker value: " + Seeker.Value.ToString());
            ++Seeker.Value;
            Debug.WriteLine("Seeker value: " + Seeker.Value.ToString());
            return null;
        }

        private void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (rbPlayPrev.IsChecked == true)
            {
                PrevBtn_Click(this, null);
            }
            else if (rbPlayNext.IsChecked == true)
            {
                // use method here to load the next track when property changes
                NextBtn_Click(this, null);
            }
            else if (rbStopThere.IsChecked == true)
            {
                Player.Stop();
            }
            else if (rbPlayAgain.IsChecked == true)
            {
                Player.Stop();
                Player.Play();
            }
        }
        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            var curTrack = Path.GetFileName(Player.Source.ToString());
            CurrentTrack.Text = curTrack;

            var duration = Player.NaturalDuration;
            Seeker.Maximum = duration.TimeSpan.TotalSeconds;

            Duration.Text = duration.ToString();
            //var dur = Player.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss");
        }

        private void BalanceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = (Slider)sender;
            var balance = slider.Value;
            Player.Balance = balance;
        }

        private void BalanceResetBtn_Click(object sender, RoutedEventArgs e)
        {
            BalanceSlider.Value = 0;
            Player.Balance = 0;
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CURRENT_INDEX == lvTracklist.Items.Count - 1)
            {
                CURRENT_INDEX = -1;
            }

            CURRENT_INDEX += 1;

            LoadTrack(CURRENT_INDEX);
        }

        private void LoadTrack(int index)
        {
            var trackItem = (TrackItem)lvTracklist.Items[index];

            var fullPath = Path.Combine(Dir.Text, trackItem.TrackName);

            Player.Source = new Uri(fullPath);
            Player.Play();
        }

        private void PrevBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CURRENT_INDEX == 0)
            {
                CURRENT_INDEX = lvTracklist.Items.Count;
            }

            CURRENT_INDEX -= 1;

            LoadTrack(CURRENT_INDEX);
        }

        private void PlayPauseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!IS_PAUSED)
            {
                Player.Pause();
                PlayPauseBtn.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Player.Play();
                PlayPauseBtn.Background = new SolidColorBrush(Colors.Blue);
            }

            IS_PAUSED = !IS_PAUSED;
        }

        private void MuteBtn_Click(object sender, RoutedEventArgs e)
        {
            Player.IsMuted = !Player.IsMuted;
            if (Player.IsMuted)
            {
                MuteBtn.Background = new SolidColorBrush(Colors.Red);
                VolumeSlider.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                MuteBtn.Background = new SolidColorBrush(Colors.Blue);
                VolumeSlider.Background = new SolidColorBrush(Colors.Blue);
            }
        }

        private void OpenTrackBtn_Click(object sender, RoutedEventArgs e)
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
            string? trackName = Path.GetFileName(selectedFilename);

            var dir = Directory.GetParent(selectedFilename);
            Dir.Text = dir.ToString();
            ScanBtn_Click(this, null);

            //var index = lvTracklist.Items.IndexOf(trackName);

            //(TrackItem)lvTracklist.Items[CURRENT_INDEX];

            for (int i = 0; i < lvTracklist.Items.Count; i++)
            {
                var trackItem = (TrackItem)lvTracklist.Items[i];
                if (trackItem.TrackName == trackName)
                {
                    CURRENT_INDEX = i;
                    var fullPath = Path.Combine(Dir.Text, trackName);
                    Player.Source = new Uri(fullPath);
                    Player.Play();
                    break;
                }
            }
        }

        private void ReplayBtn_Click(object sender, RoutedEventArgs e)
        {
            Player.Stop();
            Player.Play();
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            Player.Stop();
            IS_PAUSED = true;
        }

        private void MovePrevDirBtn_Click(object sender, RoutedEventArgs e)
        {
            // need to set cursor to end of Path

            string dir = Dir.Text;
            var prevDir = Directory.GetParent(dir);

            if (prevDir != null)
            {
                Dir.Text = prevDir.ToString();
                ScanBtn_Click(this, null);
            }
        }
        private void Seeker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Player.CanPause)
                Player.Pause();
            
            var value = (double)e.NewValue;
            TimeSpan span = TimeSpan.FromSeconds(value);
            Player.Position = span;

            Player.Play();
        }

        private void JumpToStartOfQueueBtn_Click(object sender, RoutedEventArgs e)
        {
            CURRENT_INDEX = 0;

            var trackItem = (TrackItem)lvTracklist.Items[CURRENT_INDEX];

            var fullPath = Path.Combine(Dir.Text, trackItem.TrackName);

            Player.Source = new Uri(fullPath);
            Player.Play();
        }

        private void JumpToEndOfQueueBtn_Click(object sender, RoutedEventArgs e)
        {
            CURRENT_INDEX = lvTracklist.Items.Count - 1;

            var trackItem = (TrackItem)lvTracklist.Items[CURRENT_INDEX];

            var fullPath = Path.Combine(Dir.Text, trackItem.TrackName);

            Player.Source = new Uri(fullPath);
            Player.Play();
        }

        private void LowerVolumeRepeatBtn_Click(object sender, RoutedEventArgs e)
        {
            VolumeSlider.Value -= 0.01;
        }

        private void IncreaseVolumeRepeatBtn_Click(object sender, RoutedEventArgs e)
        {
            VolumeSlider.Value += 0.01;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double volume = (double)e.NewValue;
            double temp = volume * 100;
            double roundedVolume = Math.Round(temp);

            if (tbVolume != null)
                tbVolume.Text = roundedVolume.ToString();

            Player.Volume = volume;
        }

        // vol up command
        private void VolUpCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            bool volDownIsNotPressed = !Keyboard.IsKeyDown(Key.Down) && !Keyboard.IsKeyDown(Key.S);
            e.CanExecute = volDownIsNotPressed;
        }

        private void VolUpCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            IncreaseVolumeRepeatBtn_Click(this, null);
        }

        // vol dowwn command
        private void VolDownCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            bool volUpIsNotPressed = !Keyboard.IsKeyDown(Key.Up) && !Keyboard.IsKeyDown(Key.W);
            e.CanExecute = volUpIsNotPressed;
        }

        private void VolDownCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            LowerVolumeRepeatBtn_Click(this, null);
        }

        // play prev
        private void PlayPrevCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void PlayPrevCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            rbPlayPrev.IsChecked = true;
        }

        // play next
        private void PlayNextCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void PlayNextCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            rbPlayNext.IsChecked = true;
        }


        // stop there
        private void StopThereCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void StopThereCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            rbStopThere.IsChecked = true;
        }


        // play again
        private void PlayAgainCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void PlayAgainCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            rbPlayAgain.IsChecked = true;
        }

        // TOOLBAR COMMANDS

        // open track

        private void OpenTrackCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenTrackCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            OpenTrackBtn_Click(this, null);
        }

        // scan 

        private void ScanCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ScanCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            ScanBtn_Click(this, null);
        }

        // prev dir

        private void PrevDirCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void PrevDirCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            MovePrevDirBtn_Click(this, null);
        }


        //private void Dir_Drop(object sender, DragEventArgs e)
        //{
        //    if (e.Data.GetDataPresent(DataFormats.FileDrop))
        //    {
        //        // Note that you can have more than one file.
        //        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

        //        foreach (var file in files)
        //        {
        //            Console.WriteLine(file.ToString());
        //        }

        //    }
        //}
    }
}
