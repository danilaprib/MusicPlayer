using System.Text;
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
    }
}