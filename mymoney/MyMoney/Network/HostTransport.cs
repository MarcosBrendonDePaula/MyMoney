using StardewModdingAPI;
using StardewValley;

namespace MyMoney.Network
{
    /// <summary>Crédito aplicado direto, usado quando o jogador local é o host.</summary>
    internal sealed class HostTransport : ITransferTransport
    {
        private readonly IMonitor monitor;

        public HostTransport(IMonitor monitor)
        {
            this.monitor = monitor;
        }

        /// <inheritdoc/>
        public void Credit(Farmer target, int amount)
        {
            Credit(this.monitor, target, amount);
        }

        /// <summary>
        /// Aplica o crédito na carteira individual do destino.
        /// </summary>
        /// <remarks>
        /// Estático porque o handler de mensagens do host reaproveita isto ao
        /// atender um <see cref="CreditRequest"/> vindo de um farmhand.
        /// </remarks>
        public static void Credit(IMonitor monitor, Farmer target, int amount)
        {
            if (target == null || amount <= 0)
                return;

            FarmerTeam team = Game1.player.team;
            int current = team.GetIndividualMoney(target);
            team.SetIndividualMoney(target, current + amount);

            monitor.Log($"Creditados {amount}g em {target.Name} (saldo {current} -> {current + amount}).", LogLevel.Trace);
        }
    }
}
