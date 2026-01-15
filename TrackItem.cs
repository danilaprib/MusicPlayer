using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer
{

    public class TrackItem
    {
        public int TrackIndex { get; set; }
        public string TrackName { get; set; }
        public string TrackDuration{ get; set; }

        public TrackItem(int trackIndex, string trackName, string trackDuration)
        {
            this.TrackIndex = trackIndex;
            this.TrackName = trackName;
            this.TrackDuration = trackDuration;
        }
    }

}
