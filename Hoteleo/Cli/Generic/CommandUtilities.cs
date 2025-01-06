namespace Hoteleo.Cli.Generic
{
    internal static class CommandUtilities
    {
        public static string[] ExtractArguments(string command)
        {
            var argsStartIndex = command.IndexOf('(') + 1;
            var argsEndIndex = command.IndexOf(")");

            return
                command
                .Substring(argsStartIndex, argsEndIndex - argsStartIndex)
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .ToArray();
        }
    }
}
