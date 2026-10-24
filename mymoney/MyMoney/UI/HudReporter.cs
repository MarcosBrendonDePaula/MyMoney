using MyMoney.Core;
using StardewValley;

namespace MyMoney.UI
{
    /// <summary>
    /// Mostra o resultado no HUD do jogo. Usado pela UI, onde o jogador não
    /// está olhando o console do SMAPI.
    /// </summary>
    internal sealed class HudReporter : ITransferReporter
    {
        /// <inheritdoc/>
        public void Report(TransferResult result)
        {
            string text = MessageFormatter.Describe(result);

            // O tipo da mensagem muda o ícone e o som: moeda no sucesso,
            // erro quando não deu.
            int type = result.IsSuccess
                ? HUDMessage.achievement_type
                : HUDMessage.error_type;

            Game1.addHUDMessage(new HUDMessage(text, type));
        }
    }
}
