using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicPlayer.Commands;

public static class TrackEndsCommands
{
    public static readonly RoutedUICommand PlayPrev = new RoutedUICommand("Play previous", "PlayPrev", typeof(TrackEndsCommands));
    public static readonly RoutedUICommand PlayNext = new RoutedUICommand("Play next", "PlayNext", typeof(TrackEndsCommands));
    public static readonly RoutedUICommand StopThere = new RoutedUICommand("Stop there", "StopThere", typeof(TrackEndsCommands));
    public static readonly RoutedUICommand PlayAgain = new RoutedUICommand("Play again", "PlayAgain", typeof(TrackEndsCommands));
}
