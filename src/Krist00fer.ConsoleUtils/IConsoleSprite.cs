using System;

namespace Krist00fer.ConsoleUtils
{
    public interface IConsoleSprite
    {
        void Tick(TimeSpan delta);
        void Draw();
    }
}
