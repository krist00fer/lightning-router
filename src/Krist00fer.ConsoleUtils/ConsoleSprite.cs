using System;
using System.Collections.Generic;

namespace Krist00fer.ConsoleUtils
{
    public abstract class ConsoleSprite : IConsoleSprite
    {
        public ConsoleVector Position { get; set; } = new ConsoleVector { Left = 1, Top = 1 };
        public Queue<Waypoint> Route { get; set; } = new Queue<Waypoint>();
        public double Speed { get; set; } = 12;

        public EventHandler<WaypointReachedEventArgs> WaypointReached;

        public virtual void OnWaypointReached(object sender, WaypointReachedEventArgs e)
        {
            if (WaypointReached != null)
            {
                WaypointReached(sender, e);
            }
        }

        public virtual void Tick(TimeSpan delta)
        {
            if (Route.Count > 0)
            {
                var nextWaypoint = Route.Peek();

                var waypointVector = new ConsoleVector
                {
                    Left = nextWaypoint.Position.Left - Position.Left,
                    Top = nextWaypoint.Position.Top - Position.Top
                };

                var distanceToWaypoint = Math.Sqrt(Math.Pow(waypointVector.Left, 2) + Math.Pow(waypointVector.Top, 2));

                var normalizedHeading = new ConsoleVector
                {
                    Left = waypointVector.Left / distanceToWaypoint,
                    Top = waypointVector.Top / distanceToWaypoint
                };

                var movement = new ConsoleVector
                {
                    Left = normalizedHeading.Left * delta.TotalSeconds * Speed,
                    Top = normalizedHeading.Top * delta.TotalSeconds * Speed
                };

                Position = new ConsoleVector
                {
                    Left = Math.Abs(movement.Left) > Math.Abs(waypointVector.Left) ? nextWaypoint.Position.Left : Position.Left + movement.Left,
                    Top = Math.Abs(movement.Top) > Math.Abs(waypointVector.Top) ? nextWaypoint.Position.Top : Position.Top + movement.Top
                };

                if (Position.Left == nextWaypoint.Position.Left && Position.Top == nextWaypoint.Position.Top)
                {
                    OnWaypointReached(this, new WaypointReachedEventArgs { Waypoint = nextWaypoint });
                    Route.Dequeue();
                }
            }
        }

        public abstract void Draw();

        public abstract void Erase();
    }
}
