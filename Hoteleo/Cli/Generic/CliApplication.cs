using Hoteleo.Utilities;

namespace Hoteleo.Cli.Generic
{
    internal class CliApplication
    {
        private readonly ISystemConsole _systemConsole;
        private readonly IEnumerable<ICliApplicationCommand> commands;

        public CliApplication(ISystemConsole systemConsole, IEnumerable<ICliApplicationCommand> commands)
        {
            _systemConsole = systemConsole;
            this.commands = commands;
        }

        public int Run(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var invocation = _systemConsole.ReadLine();
                var applicableCommands = commands.Where(c => c.CanRun(invocation)).ToArray();

                if (applicableCommands.Length == 0)
                {
                    _systemConsole.WriteLine("Unrecognized invocation.");
                    continue;
                }

                if (applicableCommands.Length > 1)
                {
                    _systemConsole.WriteLine("Ambiguous invocation.");
                    continue;
                }

                CommandResult result;

                try
                {
                    result = applicableCommands.Single().Run(invocation);
                    _systemConsole.WriteLine(result.Output);
                }
                catch
                {
                    return ExitCode.UnhandledException;
                }

                if (result.ShouldExit && result.IsError)
                {
                    return ExitCode.CommandError;
                }

                if (result.ShouldExit)
                {
                    return ExitCode.Success;
                }
            }

            _systemConsole.WriteLine("Application aborted.");
            return ExitCode.Aborted;
        }
    }
}
