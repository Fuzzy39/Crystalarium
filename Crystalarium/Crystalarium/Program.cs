using Crystalarium.Main;
using System;

namespace Crystalarium
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new CrystalGame();
            game.Run();
        } //I GO TO KOREA < who put this here? billy? :/
    }
}
