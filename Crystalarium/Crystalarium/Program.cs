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
        }
    }
}
