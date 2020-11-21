using System;
using MonoTek.Core;
namespace MonoTek.Run.GL
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = GameClient.Instance;
            game.Run();
        }
    }
}
