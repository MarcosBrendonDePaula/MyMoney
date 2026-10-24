using MyMoney.Core;
using StardewModdingAPI;
using StardewValley;

namespace MyMoney.Network
{
    /// <summary>
    /// Crédito aplicado direto, usado quando o jogador local é o host. É o
    /// único lugar do mod que escreve na carteira de outro jogador.
    /// </summary>
    /// <remarks>
    /// Também é quem avisa o destino, porque os dois caminhos passam por aqui:
    /// o host pagando alguém e o host atendendo o pedido de um farmhand. Pôr o
    /// aviso em cada chamador significaria esquecê-lo em um deles.
    /// </remarks>
    internal sealed class HostTransport : ITransferTransport
    {
        private readonly IMonitor monitor;
        private readonly IModHelper helper;
        private readonly string modId;
        private readonly ITransferReporter reporter;

        public HostTransport(IMonitor monitor, IModHelper helper, string modId, ITransferReporter reporter)
        {
            this.monitor = monitor;
            this.helper = helper;
            this.modId = modId;
            this.reporter = reporter;
        }

        /// <inheritdoc/>
        public void Credit(Farmer target, int amount)
        {
            this.CreditFrom(Game1.player.Name, target, amount);
        }

        /// <summary>
        /// Credita <paramref name="amount"/> em <paramref name="target"/> e
        /// avisa quem recebeu.
        /// </summary>
        /// <param name="senderName">Nome de quem pagou, para a mensagem do destino.</param>
        public void CreditFrom(string senderName, Farmer target, int amount)
        {
            if (target == null || amount <= 0)
                return;

            this.Apply(target, amount);
            this.Notify(senderName, target, amount);
        }

        /// <summary>
        /// Devolve um valor à carteira de quem pagou, sem avisar ninguém: o
        /// estorno tem mensagem própria, e quem recebe de volta não "recebeu
        /// dinheiro de" alguém.
        /// </summary>
        /// <remarks>
        /// Serve ao caso em que quem pagou saiu do jogo antes do estorno: a
        /// carteira individual vive no time, que é do host, então a escrita
        /// vale mesmo com o jogador desconectado — o saldo certo está lá
        /// quando ele voltar.
        /// </remarks>
        public void CreditBack(Farmer payer, int amount)
        {
            if (payer == null || amount <= 0)
                return;

            this.Apply(payer, amount);
        }

        /// <summary>Escreve na carteira individual do destino.</summary>
        private void Apply(Farmer target, int amount)
        {
            FarmerTeam team = target.team;
            int current = team.GetIndividualMoney(target);
            team.SetIndividualMoney(target, current + amount);

            this.monitor.Log($"Creditados {amount}g em {target.Name} (saldo {current} -> {current + amount}).", LogLevel.Trace);
        }

        /// <summary>Conta ao destino que o dinheiro chegou.</summary>
        private void Notify(string senderName, Farmer target, int amount)
        {
            // O destino pode ser o próprio host (um farmhand pagando quem
            // hospeda): aí não há mensagem a enviar, é só mostrar no HUD.
            if (target.UniqueMultiplayerID == Game1.player.UniqueMultiplayerID)
            {
                this.reporter.Report(TransferResult.Received(senderName, amount));
                return;
            }

            this.helper.Multiplayer.SendMessage(
                message: new CreditReceipt { Amount = amount, SenderName = senderName },
                messageType: CreditReceipt.MessageType,
                modIDs: new[] { this.modId },
                playerIDs: new[] { target.UniqueMultiplayerID }
            );
        }
    }
}
