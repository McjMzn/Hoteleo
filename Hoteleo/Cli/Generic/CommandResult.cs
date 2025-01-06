namespace Hoteleo.Cli.Generic
{
    internal readonly record struct CommandResult(string Output, bool IsError, bool ShouldExit);
}
