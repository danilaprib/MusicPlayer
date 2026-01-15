using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicPlayer.Commands;

public static class ToolBarCommands
{
    public static readonly RoutedUICommand OpenTrack = new RoutedUICommand("Open track", "OpenTrack", typeof(ToolBarCommands), new InputGestureCollection() { new KeyGesture(Key.O, ModifierKeys.Control) });
    public static readonly RoutedUICommand Scan = new RoutedUICommand("Scan Path", "Scan", typeof(ToolBarCommands), new InputGestureCollection() { new KeyGesture(Key.F, ModifierKeys.Control) });
    public static readonly RoutedUICommand PrevDir = new RoutedUICommand("Previous directory", "PrevDir", typeof(ToolBarCommands), new InputGestureCollection() { new KeyGesture(Key.P, ModifierKeys.Control)});
}
