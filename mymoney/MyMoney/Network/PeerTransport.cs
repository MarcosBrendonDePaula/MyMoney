using StardewModdingAPI;
using StardewValley;

namespace MyMoney.Network
{
    /// <summary>
    /// Crédito pedido ao host, usado quando o jogador local é um farmhand.
    /// </summary>
    /// <remarks>
    /// O mod antigo escrevia na carteira do outro jogador de qualquer máquina.
    /// Isso depende de a sincronização do campo aceitar escrita de qualquer
    /// peer — foi o tipo de detalhe que fez o mod "parar de funcionar" numa
    /// atualização do jogo. Pedir ao host, que é a autoridade do save, é
    /// estável entre versões.
    /// </remarks>
    internal sealed class PeerTransport : ITransferTransport
    {
        private readonly IModHelper helper;
        private readonly string modId;

        public PeerTransport(IModHelper helper, string modId)
        {
            this.helper = helper;
            this.modId = modId;
        }

        /// <inheritdoc/>
        public void Credit(Farmer target, int amount)
        {
            if (target == null || amount <= 0)
                return;

            var request = new CreditRequest
            {
                TargetPlayerId = target.UniqueMultiplayerID,
                Amount = amount,
                SenderName = Game1.player.Name
            };

            this.helper.Multiplayer.SendMessage(
                message: request,
                messageType: CreditRequest.MessageType,
                modIDs: new[] { this.modId }
            );
        }
    }
}
