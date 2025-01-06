using Hoteleo.Utilities;

namespace Hoteleo.Cli.Generic
{
    internal class CliApplication
    {
        private readonly ISystemConsole _systemConsole;
        private readonly IEnumerable<ICliApplicationCommand> commands;

        public CliApplication(ISystemConsole systemConsole, IEnumerable<ICliApplicationCommand> _commands)
        {
            _systemConsole = systemConsole;
            _commands = _commands;
        }

        public int Run(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var command = _systemConsole.ReadLine();
                var applicableCommands = commands.Where(c => c.CanRun(command)).ToArray();

                if (applicableCommands.Length == 0)
                {
                    _systemConsole.WriteLine("Unrecognized command.");
                    continue;
                }

                if (applicableCommands.Length > 1)
                {
                    _systemConsole.WriteLine("Ambiguous command.");
                    continue;
                }

                CommandResult result;

                try
                {
                    result = applicableCommands.Single().Run(command);
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
