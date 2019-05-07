using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Krist00fer.ConsoleUtils
{
    public class ConsoleScreen
    {
        List<string> _background;

        public int Width { get; set; } = 95;
        public int Hight { get; set; } = 30;

        public List<ConsoleSprite> Sprites { get; set; } = new List<ConsoleSprite>();

        public void LoadBackground(string backgroundFilePath)
        {
            var lines = File.ReadAllLines(backgroundFilePath);

            _background = (from l in lines
                           select l.PadRight(Width)).ToList();

            for (int i = _background.Count; i < Hight; i++)
            {
                _background.Add(new string(' ', Width));
            }
        }

        public void Init()
        {
            Console.CursorVisible = false;
            Console.SetWindowSize(Width, Hight);
            Console.SetBufferSize(Width, Hight);
            Console.Clear();
            DrawBackground();
        }

        public void NextFrame()
        {
            var delay = TimeSpan.FromSeconds(0.10);

            Draw();
            Thread.Sleep(delay);
            Erase();
            Tick(delay);
        }

        public virtual void Erase()
        {
            foreach (var sprite in Sprites)
            {
                sprite.Erase();
            }
        }

        public virtual void Tick(TimeSpan delta)
        {
            foreach (var sprite in Sprites)
            {
                sprite.Tick(delta);
            }
        }

        public virtual void Draw()
        {
            foreach (var sprite in Sprites)
            {
                sprite.Draw();
            }
        }

        public virtual void DrawBackground()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;

            Console.SetCursorPosition(0, 0);

            foreach (var line in _background)
            {
                Console.WriteLine(line);
            }

            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
