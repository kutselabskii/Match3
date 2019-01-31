using System;

namespace MatchThree
{
#if WINDOWS || LINUX
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new MatchThreeGame())
                game.Run();
        }
    }
#endif
}
