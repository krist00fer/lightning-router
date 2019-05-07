using System;
using Krist00fer.ConsoleUtils;

namespace Krist00fer.LightningRouter
{
    public class GuitarConsoleSprite : ConsoleSprite
    {
        public string Brand { get; set; }
        public string Color { get; set; }

        public string Label
        {
            get
            {
                return $"{Brand}";
            }
        }
        public override void Draw()
        {
            Console.SetCursorPosition((int)Position.Left, (int)Position.Top);

            var defaultColor = Console.ForegroundColor;
            switch (Color)
            {
                case "White":
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case "Green":
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
            }

            Console.Write(Label);
            Console.ForegroundColor = defaultColor;
        }

        public override void Erase()
        {
            Console.SetCursorPosition((int)Position.Left, (int)Position.Top);
            Console.WriteLine(new string(' ', Label.Length));
        }
    }
}
