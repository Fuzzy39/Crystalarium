using System;

namespace Crystalarium
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Crystalarium())
                game.Run();
        } //I GO TO KOREA < who put this here? billy? :/
    }
}
