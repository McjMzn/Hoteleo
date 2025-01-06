namespace Hoteleo.Cli.Generic
{
    internal interface ICliApplicationCommand
    {
        bool CanRun(string command);

        CommandResult Run(string command);
    }
}
