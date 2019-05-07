using NLua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Krist00fer.CollectionUtils;
using Krist00fer.ConsoleUtils;

namespace Krist00fer.LightningRouter
{
    class Program
    {
        private static ConsoleScreen _screen;
        private static Queue<RoutedMessage> _messageQueue;
        private static bool _addNewMessage = true;
        private static bool _removeMessage = false;
        private static int _flashRouter = 0;
        private static string _lastRouteValue = "";

        private static LuaFunction _routeMessageFunction;

        const string RoutinFileName = "Routing.lua";

        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Please provide inputFilePath, routingDirectory and routingFile");
                return;
            }

            _screen = new ConsoleScreen();
            _screen.LoadBackground("background.txt");
            _screen.Init();


            var rnd = new Random();
            var inputFilePath = args[0];
            var routingDirectory = args[1];
            var routingFile = args[2];

            var routingPath = Path.Combine(routingDirectory, routingFile);

            var messageList = (from l in File.ReadAllLines(inputFilePath)
                               where l.Trim().Length < 8
                               select new RoutedMessage { Content = l, Color = RandomColor(rnd) }).ToList();
            messageList.Shuffle(rnd);
            _messageQueue = new Queue<RoutedMessage>(messageList);


            using (var watcher = new FileSystemWatcher())
            using (var lua = new Lua())
            {
                // Setup Lua environment
                lua.DoFile(routingPath);
                _routeMessageFunction = lua["RouteMessage"] as LuaFunction;

                // Setup FileSysteWatcher to monitor routing file
                watcher.Path = routingDirectory;
                watcher.Filter = routingFile;
                watcher.Changed += (sender, e) =>
                {
                    lua.DoFile(e.FullPath);
                    _routeMessageFunction = lua["RouteMessage"] as LuaFunction;
                };
                watcher.EnableRaisingEvents = true;

                while (true)
                {
                    _screen.NextFrame();

                    AddAndRemoveMessagesAsNeeded();
                    FlashRouterWhenMessagesArrive();
                }
            }
        }

        private static string OnRouteMessage(string brand, string color)
        {
            return "X";

            #region XYZ
            //var result = _routeMessageFunction.Call(brand, color);
            //return (string)result[0];
            #endregion
        }

        #region Helper Methods
        private static void AddAndRemoveMessagesAsNeeded()
        {
            if (_addNewMessage)
            {
                AddNewMessageToPipeline();
                _addNewMessage = false;
            }

            if (_removeMessage)
            {
                _screen.Sprites.RemoveAt(0);
                _removeMessage = false;
            }
        }

        private static void FlashRouterWhenMessagesArrive()
        {
            if (_flashRouter == 10)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.SetCursorPosition(33, 11);
                Console.Write($"   {_lastRouteValue}    ");
                Console.SetCursorPosition(33, 15);
                Console.Write($"   {_lastRouteValue}    ");
                Console.BackgroundColor = ConsoleColor.Black;
            }
            else if (_flashRouter == 1)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(33, 11);
                Console.Write("        ");
                Console.SetCursorPosition(33, 15);
                Console.Write("        ");
            }
            if (_flashRouter > 0)
            {
                _flashRouter--;
            }
        }


        private static void AddNewMessageToPipeline()
        {
            var message = _messageQueue.Dequeue();

            var sprite = new GuitarConsoleSprite { Brand = message.Content, Color = message.Color, Position = new ConsoleVector { Left = 0, Top = 13 } };
            sprite.Route.Enqueue(new Waypoint { Position = new ConsoleVector { Left = 33, Top = 13 }, WaypointName = "Router" });
            sprite.Route.Enqueue(new Waypoint { Position = new ConsoleVector { Left = 45, Top = 13 }, WaypointName = "Fork" });

            sprite.WaypointReached += (sender, e) =>
            {
                if (e.Waypoint.WaypointName == "Router")
                {
                    _flashRouter = 10;
                    
                    var s = sender as GuitarConsoleSprite;
                    var r = OnRouteMessage(s.Brand, s.Color);
                    _lastRouteValue = r;

                    if (r == "A")
                    {
                        s.Route.Enqueue(new Waypoint { Position = new ConsoleVector { Left = 55, Top = 4 }, WaypointName = "UpperBend" });
                        s.Route.Enqueue(new Waypoint { Position = new ConsoleVector { Left = 80, Top = 4 }, WaypointName = "End" });
                    }
                    else if (r == "B")
                    {
                        s.Route.Enqueue(new Waypoint { Position = new ConsoleVector { Left = 80, Top = 13 }, WaypointName = "End" });
                    }
                    else if (r == "C")
                    {
                        s.Route.Enqueue(new Waypoint { Position = new ConsoleVector { Left = 55, Top = 22 }, WaypointName = "LowerBend" });
                        s.Route.Enqueue(new Waypoint { Position = new ConsoleVector { Left = 80, Top = 22 }, WaypointName = "End" });
                    }
                    else
                    {
                        _removeMessage = true;
                    }

                    _addNewMessage = true;
                }
                else if (e.Waypoint.WaypointName == "End")
                {
                    _removeMessage = true;
                }
            };
            _screen.Sprites.Add(sprite);
        }

        private static string RandomColor(Random rnd)
        {
            switch (rnd.Next(3))
            {
                case 0:
                    return "White";
                case 1:
                    return "Green";
                default:
                    return "Pink";
            }
        }
        #endregion
    }
}
