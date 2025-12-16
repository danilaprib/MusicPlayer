using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class MainWindow : Window
    {
        const string AUDIO_FORMAT_PATTERN = "^.*\\.(mp3)$";
        int CURRENT_TRACK_INDEX = 0;
        //Dictionary<int, string> _tracklist = new Dictionary<int, string>();

        string[] tracks;
        public MainWindow()
        {
            InitializeComponent();

            
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            player.Stop();
        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            player.Play();
            PausedLabel.Visibility = Visibility.Hidden;
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            player.Pause();
            PausedLabel.Visibility = Visibility.Visible;
        }

        private void player_MediaOpened(object sender, RoutedEventArgs e)
        {

        }

        private void Dir_LostFocus(object sender, RoutedEventArgs e)
        {
            var dir = Dir.Text;
            if (!Directory.Exists(dir))
            {
                return;
            }

            tracks = Directory.GetFiles(dir, "*.mp3");
            //var correctTracknames = new List<string>();
            //foreach (var trackname in tracknames)
            //{
            //    if (Regex.Match(trackname, FILE_REGEX).Success)
            //    {
            //        correctTracknames.Add(trackname);
            //    }
            //}


            Tracklist.Children.Clear();
            for(int i = 0; i < tracks.Length; i++)
            {
                //_tracklist.Add(i, tracks[i]);

                DockPanel panel = new DockPanel();
                panel.LastChildFill = false;

                TextBlock index = new TextBlock();
                index.FontSize = 18;
                index.Text = i.ToString();
                index.VerticalAlignment = VerticalAlignment.Center;

                Button button = new Button();
                var style = TryFindResource("TrackNameStyle");
                if (style != null)
                {
                    button.Style = (Style)style;
                }
                    
                button.Click += TrackBtn_Click; 
                button.Content = tracks[i];

                // why does docking work this way ?
                DockPanel.SetDock(button, Dock.Left);
                panel.Children.Add(button);


                DockPanel.SetDock(button, Dock.Right);
                panel.Children.Add(index);

                Tracklist.Children.Add(panel);
            }

            CURRENT_TRACK_INDEX = 0;
            var track = (DockPanel)Tracklist.Children[CURRENT_TRACK_INDEX];
            //track.Children[0];

            var btn = (Button)track.Children[0];
            var trackname = btn.Content as string;

            if (trackname != null)
            {
                CurrentTrack.Content = trackname;
                player.Source = new Uri(trackname);

            }
        }

        private void TrackBtn_Click(Object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var trackname = btn.Content as string;
            CURRENT_TRACK_INDEX = Tracklist.Children.IndexOf(btn);
            if (trackname != null)
            {
                CurrentTrack.Content = trackname;
                player.Stop();
                player.Source = new Uri(trackname);
                player.Play();
            }

            //player.Balance = -1; USE THIS FOR MIXER 
        }

        private void ReplayBtn_Click(object sender, RoutedEventArgs e)
        {
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

            if (trackname != null)
            {
                CurrentTrack.Content = trackname;
                player.Stop();
                player.Source = new Uri(trackname);
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

            if (trackname != null)
            {
                CurrentTrack.Content = trackname;
                player.Stop();
                player.Source = new Uri(trackname);
                player.Play();
            }
        }


        private void LoopBtn_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("LOOP button pressed");
        }

        private void player_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.Next_Click(this, null);
        }
    }
}