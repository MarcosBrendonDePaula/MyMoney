using System.Collections.Generic;
using StardewModdingAPI;

namespace MyMoney.Commands
{
    /// <summary>
    /// Registra todos os comandos no console do SMAPI.
    /// </summary>
    /// <remarks>
    /// Com isto, adicionar um comando novo é criar a classe e incluí-la na
    /// lista do <c>ModEntry</c> — o Entry não cresce a cada comando.
    /// </remarks>
    internal sealed class CommandRegistry
    {
        private readonly ICommandHelper commands;
        private readonly IMonitor monitor;

        public CommandRegistry(ICommandHelper commands, IMonitor monitor)
        {
            this.commands = commands;
            this.monitor = monitor;
        }

        public void RegisterAll(IEnumerable<IConsoleCommand> all)
        {
            foreach (IConsoleCommand command in all)
            {
                this.commands.Add(command.Name, command.Description, (_, args) =>
                {
                    try
                    {
                        command.Execute(args);
                    }
                    catch (System.Exception ex)
                    {
                        // Um comando nunca deve derrubar o jogo. O mod antigo
                        // lançava IndexOutOfRange ao ser chamado sem argumentos.
                        this.monitor.Log($"Erro ao executar '{command.Name}': {ex}", LogLevel.Error);
                    }
                });
            }
        }
    }
}
