using System;

namespace Krist00fer.ConsoleUtils
{
    public class WaypointReachedEventArgs : EventArgs
    {
        public Waypoint Waypoint { get; set; }
    }
}
