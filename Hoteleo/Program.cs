using Hoteleo.Cli;

namespace Hoteleo
{
    internal class Program
    {
        static int Main(string[] args)
        {
            var launcher = new HoteleoApplicationLauncher(args);
         
            return launcher.Launch();
        }
    }
}
