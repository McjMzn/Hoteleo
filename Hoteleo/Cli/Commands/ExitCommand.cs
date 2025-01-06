using Hoteleo.Cli.Generic;

namespace Hoteleo.Cli.Commands
{
    internal class ExitCommand : ICliApplicationCommand
    {
        public bool CanRun(string invocation) => invocation == string.Empty;

        public CommandResult Run(string invocation) => new CommandResult(string.Empty, false, true);
    }
}
