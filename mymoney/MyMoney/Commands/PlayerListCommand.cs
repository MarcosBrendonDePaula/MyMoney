using System.Collections.Generic;
using System.Text;
using MyMoney.Services;
using StardewModdingAPI;
using StardewValley;

namespace MyMoney.Commands
{
    /// <summary>
    /// <c>player_list</c> — mostra os jogadores online e o nome exato de cada um.
    /// </summary>
    /// <remarks>
    /// Existe porque o erro mais comum ao pagar é errar o nome: quem joga vê o
    /// apelido no chat, que pode diferir do nome do fazendeiro. Aqui os dois
    /// aparecem lado a lado.
    /// </remarks>
    internal sealed class PlayerListCommand : IConsoleCommand
    {
        private readonly IPlayerLookup lookup;
        private readonly IMonitor monitor;

        public PlayerListCommand(IPlayerLookup lookup, IMonitor monitor)
        {
            this.lookup = lookup;
            this.monitor = monitor;
        }

        /// <inheritdoc/>
        public string Name => "player_list";

        /// <inheritdoc/>
        public string Description => Translations.CommandListDescription;

        /// <inheritdoc/>
        public void Execute(string[] args)
        {
            if (!Context.IsWorldReady)
            {
                this.monitor.Log(Translations.WorldNotReady, LogLevel.Warn);
                return;
            }

            IReadOnlyList<Farmer> players = this.lookup.GetOnlinePlayers();

            StringBuilder text = new();
            text.AppendLine(Translations.CommandListHeader);

            foreach (Farmer player in players)
            {
                bool isSelf = player.UniqueMultiplayerID == Game1.player.UniqueMultiplayerID;
                string name = player.Name;
                string display = player.displayName;

                text.Append("  - ").Append(name);

                // Só mostra o apelido quando ele difere do nome, para a lista
                // não ficar repetitiva.
                if (!string.IsNullOrEmpty(display) && display != name)
                    text.Append(" (").Append(display).Append(')');

                if (isSelf)
                    text.Append(" <- você");

                text.AppendLine();
            }

            this.monitor.Log(text.ToString().TrimEnd(), LogLevel.Info);
        }
    }
}
