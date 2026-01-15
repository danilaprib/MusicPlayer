using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicPlayer.Commands;

public static class ControlCommands
{
    public static readonly RoutedUICommand VolUp = new RoutedUICommand("Increase volume", "VolUp", typeof(ControlCommands));
    public static readonly RoutedUICommand VolDown = new RoutedUICommand("Decrease volume", "VolDown", typeof(ControlCommands));
}
