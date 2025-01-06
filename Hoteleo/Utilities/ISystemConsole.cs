namespace Hoteleo.Utilities
{
    internal interface ISystemConsole
    {
        string ReadLine();

        void WriteLine(string text);
    }

    internal class SystemConsole : ISystemConsole
    {
        public string ReadLine() => Console.ReadLine();

        public void WriteLine(string text) => Console.WriteLine(text);
    }
}
